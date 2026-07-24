using LotTraceApp.Models;
using LotTraceApp.Repositories;
using LotTraceApp.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LotTraceApp.Services
{
    public class ResultService
    {
        private readonly ResultRepositories _repo;
        private readonly DisplayNameProvider _dis;

        // CSV（表示名変換）
        private static readonly string csvPath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config", "SingleControlProcessTable.csv");

        // FilterTable（表示名変換）
        private static readonly string filterCsvPath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config", "FilterTable.csv");

        private readonly DisplayNameProvider _filterDis;

        // ★ 工程フィルター履歴（TSV保存）
        private readonly FilterHistoryStore _filterHistoryStore;

        // ★ ドロップダウン用 擬似ID
        public const string FilterId = "__FILTER__";

        public ResultService(ResultRepositories repo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));

            // CSVはここで1回読み込み
            _dis = new DisplayNameProvider(csvPath);

            _filterDis = new DisplayNameProvider(filterCsvPath);
            _filterHistoryStore = new FilterHistoryStore(BuildFilterHistoryPath());

        }

        public bool TryGetDisplayName(string originalName, out string? displayName)
        {
            displayName = null;

            if (string.IsNullOrWhiteSpace(originalName))
                return false;

            
            if (_dis.Map.TryGetValue(originalName, out var value) && !string.IsNullOrWhiteSpace(value))
            {
                displayName = value;
                return true;
            }

            return false;
        }

        public List<ProcessItem> GetAvailableProcesses(string productionOrderNumber, string itemCode, string lotNumber)
        {
            var all = new List<ProcessItem>
            {
                new ProcessItem { Id = "1", Name = "CIP制御工程" },
                new ProcessItem { Id = "2", Name = "N2押し制御工程" },
                new ProcessItem { Id = "3", Name = "エア抜き制御工程" },
                new ProcessItem { Id = "4", Name = "液上制御工程" },
                new ProcessItem { Id = "6", Name = "撹拌制御工程" },
                new ProcessItem { Id = "7", Name = "仕込制御工程" },
                new ProcessItem { Id = "C", Name = "循環制御工程(圧力制御有)" },
                new ProcessItem { Id = "D", Name = "循環制御工程(圧力制御無)" },
                new ProcessItem { Id = "E", Name = "循環制御工程(流量制御有)" },
                new ProcessItem { Id = "F", Name = "初液抜き制御工程" },
                new ProcessItem { Id = "G", Name = "抜出制御工程" },
                new ProcessItem { Id = "H", Name = "廃液制御工程" },
                new ProcessItem { Id = "I", Name = "蓋ロック制御工程" },
            };

            var available = new HashSet<string>(
                _repo.GetAvailableProcessIds(productionOrderNumber, itemCode, lotNumber),
                StringComparer.OrdinalIgnoreCase);

            return all.Where(p => p.Id is not null && available.Contains(p.Id)).ToList();
        }

        public DataTable GetProcessView(string productionOrderNumber, string itemCode, string lotNumber, string processId)
        {
            // 制御に該当するデータだけDBから取得（切り捨て）
            var raw = _repo.GetSingleControlHistory(productionOrderNumber, itemCode, lotNumber, processId);

            // 指図/実績 横持ち変換（CSVにある列だけ表示）
            return ToOrderAndActualHorizontal(raw);
        }

        // ★ フィルター（FilterTable）表示用
        public DataTable GetFilterView(string productionOrderNumber, string itemCode, string lotNumber)
        {
            var raw = _repo.GetFilterHistory(productionOrderNumber, itemCode, lotNumber);

            // FilterTableはCSV列定義が無い想定なので「全列表示版」で横持ち化
            return ToOrderAndActualHorizontal_AllColumns(raw);
        }
        // ★ FilterTable に該当データがあるか（ドロップダウンに「フィルタ」を出す判定用）
        public bool HasFilterData(string productionOrderNumber, string itemCode, string lotNumber)
        {
            return _repo.HasFilterHistory(itemCode, lotNumber);
        }

        public void ApplyDisplayNamesToGrid(DataGridView dgv)
        {
            if (dgv == null)
                return;

            foreach (DataGridViewColumn col in dgv.Columns)
            {
                string key = !string.IsNullOrWhiteSpace(col.DataPropertyName)
                    ? col.DataPropertyName
                    : col.Name;

                
                if (TryGetDisplayName(key, out var display))
                {
                    col.HeaderText = display; // CSVにあるものだけ変更
                }
            }
        }

        public DataTable ToOrderAndActualHorizontal(DataTable src)
        {
            var dst = new DataTable();
            dst.Columns.Add("項目", typeof(string));

            if (src == null || src.Rows.Count == 0 || src.Columns.Count == 0)
                return dst;

            if (!src.Columns.Contains("DataCategory"))
                return dst;

            // ★ 交互マージしない：DB返却順のまま（指図/実績のみ採用）
            // ★ DB返却順（出現パターン）は維持し、実績だけ StartDate 昇順にする
            var baseList = src.Rows
                .Cast<DataRow>()
                .Select((r, idx) => new { Row = r, Index = idx })
                .Where(x =>
                {
                    var cat = Convert.ToString(x.Row["DataCategory"]);
                    return cat == "指図" || cat == "実績";
                })
                .ToList();

            if (baseList.Count == 0)
                return dst;

            // 実績だけ抽出して StartDate 昇順（同値/Nullは元順で安定化）
            var sortedActuals = baseList
                .Where(x => string.Equals(Convert.ToString(x.Row["DataCategory"]), "実績", StringComparison.OrdinalIgnoreCase))
                .OrderBy(x => GetStartDateForSort(x.Row))
                .ThenBy(x => x.Index)
                .Select(x => x.Row)
                .ToList();

            // “元の並び”を維持したまま、実績スロットだけ順に差し込む
            int ai = 0;
            var merged = new List<DataRow>(baseList.Count);

            foreach (var x in baseList)
            {
                var cat = Convert.ToString(x.Row["DataCategory"]);

                if (string.Equals(cat, "実績", StringComparison.OrdinalIgnoreCase))
                {
                    merged.Add(ai < sortedActuals.Count ? sortedActuals[ai++] : x.Row);
                }
                else
                {
                    // 指図は完全にそのまま
                    merged.Add(x.Row);
                }
            }

            if (merged.Count == 0)
                return dst;

            // --- 列（指図1/実績1/指図2/実績2...）を merged の順で作る ---
            var colNames = new List<string>();
            int orderNo = 0;
            int actualNo = 0;

            foreach (var r in merged)
            {
                string? cat = Convert.ToString(r["DataCategory"]);
                string header;

                if (cat == "指図")
                {
                    orderNo++;
                    header = "指図" + orderNo;
                }
                else // "実績"
                {
                    actualNo++;
                    header = "実績" + actualNo;
                }

                dst.Columns.Add(header, typeof(string));
                colNames.Add(header);
            }

            // --- 表示対象の項目（列）を決める：CSVにある列だけ＋開始/終了 ---
            var itemColumns = new List<DataColumn>();

            void AddIfExists(string colName)
            {
                if (src.Columns.Contains(colName))
                {
                    var c = src.Columns[colName]!;
                    if (!itemColumns.Contains(c)) itemColumns.Add(c);
                }
            }

            AddIfExists("StartDate");
            AddIfExists("EndDate");

            foreach (DataColumn c in src.Columns)
            {
                if (string.Equals(c.ColumnName, "MasterKey", StringComparison.OrdinalIgnoreCase)) continue;
                if (string.Equals(c.ColumnName, "ForeignKey", StringComparison.OrdinalIgnoreCase)) continue;
                if (string.Equals(c.ColumnName, "DataCategory", StringComparison.OrdinalIgnoreCase)) continue;
                if (string.Equals(c.ColumnName, "StartDate", StringComparison.OrdinalIgnoreCase)) continue;
                if (string.Equals(c.ColumnName, "EndDate", StringComparison.OrdinalIgnoreCase)) continue;

                
                if (_dis.Map.TryGetValue(c.ColumnName, out var display) && !string.IsNullOrWhiteSpace(display))
                {
                    itemColumns.Add(c);
                }
            }

            // --- 横持ち変換 ---
            foreach (DataColumn itemCol in itemColumns)
            {
                string itemName;
                if (string.Equals(itemCol.ColumnName, "StartDate", StringComparison.OrdinalIgnoreCase))
                    itemName = "開始日時";
                else if (string.Equals(itemCol.ColumnName, "EndDate", StringComparison.OrdinalIgnoreCase))
                    itemName = "終了日時";
                else
                    itemName = _dis.Get(itemCol.ColumnName);

                var newRow = dst.NewRow();
                newRow["項目"] = itemName;

                int colIndex = 0;
                foreach (var r in merged)
                {
                    if (colIndex >= colNames.Count) break;

                    string header = colNames[colIndex++];
                    object v = r[itemCol];

                    if (v == DBNull.Value) newRow[header] = "";
                    else if (v is DateTime dt) newRow[header] = dt.ToString("yyyy/MM/dd HH:mm:ss");
                    else newRow[header] = Convert.ToString(v);
                }

                dst.Rows.Add(newRow);
            }

            return dst;
        }
        private static DateTime GetStartDateForSort(DataRow r)
        {
            if (r == null || r.Table == null || !r.Table.Columns.Contains("StartDate"))
                return DateTime.MaxValue;

            object v = r["StartDate"];
            if (v == null || v == DBNull.Value)
                return DateTime.MaxValue;

            if (v is DateTime dt)
                return dt;

            DateTime parsed;
            if (DateTime.TryParse(Convert.ToString(v), out parsed))
                return parsed;

            return DateTime.MaxValue;
        }

        // ★ 全列表示版（FilterTable向け）
        // ★ 全列表示版（FilterTable向け）※値も入れる版
        public DataTable ToOrderAndActualHorizontal_AllColumns(DataTable src)
        {
            var dst = new DataTable();
            dst.Columns.Add("項目", typeof(string));



            ///本郷のやつ
            ///MasterKeyを見て、Columnsの構成を決める

            //dst.Columns.Add("セット１");
            //dst.Columns.Add("セット２");

            ///

            
            ///縦のデータセットを作る
            
            ///




            //List<string> filterDataSet = new List<string>();

            //foreach (DataRow row in src.Rows)
            //{

            //    string value01 = row["FilterSetNumber01"].ToString();
            //    string value02 = row["FilterSetNumber02"].ToString();

            //    filterDataSet.Add(value01);
               
                
            //}

            /////

            //dst.Rows.Add("フィルターセットNo.", filterDataSet[0]);

            //dst.Rows.Add("フィルター品目コード", filterDataSet[1]);



            ///ここまで本郷のやつ



            if (src == null || src.Rows.Count == 0 || src.Columns.Count == 0)
                return dst;

            if (!src.Columns.Contains("DataCategory"))
                return dst;

            // 指図/実績だけ使う
            var baseList = src.Rows
                .Cast<DataRow>()
                .Select((r, idx) => new { Row = r, Index = idx })
                .Where(x =>
                {
                    var cat = Convert.ToString(x.Row["DataCategory"]);
                    return string.Equals(cat, "指図", StringComparison.OrdinalIgnoreCase)
                        || string.Equals(cat, "実績", StringComparison.OrdinalIgnoreCase);
                })
                .ToList();

            if (baseList.Count == 0)
                return dst;

            // FilterTableは日時が無い想定：SQL返却順を尊重してそのまま
            var merged = baseList.OrderBy(x => x.Index).Select(x => x.Row).ToList();

            // --- 列（指図1/実績1/…）を merged の順で作る ---
            var colNames = new List<string>();
            int orderNo = 0, actualNo = 0;

            foreach (var r in merged)
            {
                string? cat = Convert.ToString(r["DataCategory"]);
                string header;

                if (string.Equals(cat, "指図", StringComparison.OrdinalIgnoreCase))
                    header = "指図" + (++orderNo);
                else
                    header = "実績" + (++actualNo);

                dst.Columns.Add(header, typeof(string));
                colNames.Add(header);
            }

            // --- 表示対象の項目（列）を決める：全列（ただしキー系は除外） ---
            var itemColumns = new List<DataColumn>();
            foreach (DataColumn c in src.Columns)
            {
                if (c.ColumnName.Equals("MasterKey", StringComparison.OrdinalIgnoreCase)) continue;
                if (c.ColumnName.Equals("ForeignKey", StringComparison.OrdinalIgnoreCase)) continue;
                if (c.ColumnName.Equals("DataCategory", StringComparison.OrdinalIgnoreCase)) continue;

                itemColumns.Add(c);
            }

            // --- 横持ち変換（★ここで値を詰める） ---
            foreach (var itemCol in itemColumns)
            {
                var newRow = dst.NewRow();

                // 表示名：CSVにあれば使い、無ければ列名そのまま
                
                if (_filterDis != null && _filterDis.Map != null
                    && _filterDis.Map.TryGetValue(itemCol.ColumnName, out var disp)
                    && !string.IsNullOrWhiteSpace(disp))
                {
                    newRow["項目"] = disp;
                }
                else
                {
                    newRow["項目"] = itemCol.ColumnName;
                }

                int colIndex = 0;
                foreach (var r in merged)
                {
                    if (colIndex >= colNames.Count) break;

                    string header = colNames[colIndex++];
                    object v = r[itemCol];

                    if (v == DBNull.Value) newRow[header] = "";
                    else if (v is DateTime dt) newRow[header] = dt.ToString("yyyy/MM/dd HH:mm:ss");
                    else newRow[header] = Convert.ToString(v);
                }

                dst.Rows.Add(newRow);
            }

            return dst;
        }

        // ==========================
        // ★ 工程フィルター履歴（Result用）
        // ==========================
        private static string BuildFilterHistoryPath()
        {
            var dir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "LotTraceApp");
            return Path.Combine(dir, "ResultFilterHistory.tsv");
        }

        private static string BuildContextKey(string productionOrderNumber, string itemCode, string lotNumber)
        {
            return (productionOrderNumber ?? "") + "|" + (itemCode ?? "") + "|" + (lotNumber ?? "");
        }

        public void RememberFilterHistory(string productionOrderNumber, string itemCode, string lotNumber, string processId)
        {
            if (string.IsNullOrWhiteSpace(processId))
                return;

            // ★ 擬似IDは履歴に入れない
            if (string.Equals(processId, FilterId, StringComparison.OrdinalIgnoreCase))
                return;

            string ctx = BuildContextKey(productionOrderNumber, itemCode, lotNumber);
            processId = processId.Trim();

            var all = _filterHistoryStore.LoadAll();

            var hit = all.FirstOrDefault(x => x is not null 
                        && string.Equals( x.ContextKey, ctx, StringComparison.OrdinalIgnoreCase)
                        && string.Equals(x.ProcessId, processId,StringComparison.OrdinalIgnoreCase));

            if (hit == null)
            {
                hit = new FilterHistoryEntry
                {
                    ContextKey = ctx,
                    ProcessId = processId,
                    UseCount = 0,
                    LastUsedUtc = DateTime.UtcNow
                };
                all.Add(hit);
            }

            hit.UseCount += 1;
            hit.LastUsedUtc = DateTime.UtcNow;

            // 1ロット（Context）あたり上限30件
            all = all
                .GroupBy(x => x.ContextKey, StringComparer.OrdinalIgnoreCase)
                .SelectMany(g => g.OrderByDescending(x => x.LastUsedUtc).Take(30))
                .ToList();

            _filterHistoryStore.SaveAll(all);
        }

        // ★ cmbTarget の DataSource にそのまま渡せる（フィルター→区切り→履歴→区切り→通常）
        public DataTable BuildProcessDropdownTable(string? productionOrderNumber, string? itemCode, string? lotNumber)
        {

            var dt = new DataTable();
            dt.Columns.Add("Id");
            dt.Columns.Add("Name");

            if (string.IsNullOrWhiteSpace(productionOrderNumber)
                || string.IsNullOrWhiteSpace(itemCode)
                || string.IsNullOrWhiteSpace(lotNumber))
            {
                return dt;
            }   

            // 制御工程（利用可能なものだけ）
            var available = GetAvailableProcesses(productionOrderNumber, itemCode, lotNumber);
            foreach (var p in available)
            {
                var dr = dt.NewRow();
                dr["Id"] = p.Id;
                dr["Name"] = p.Name;
                dt.Rows.Add(dr);
            }

            // ★ FilterTable に該当がある場合のみ「フィルタ」を末尾に出す
            if (HasFilterData(productionOrderNumber, itemCode, lotNumber))
            {
                var dr = dt.NewRow();
                dr["Id"] = FilterId;     // "__FILTER__"
                dr["Name"] = "フィルタ";
                dt.Rows.Add(dr);
            }


            return dt;
        }
        public string? ResolveProcessIdByMasterKey(
        string productionOrderNumber,string itemCode,string lotNumber,string masterKey)
        {
            if (string.IsNullOrWhiteSpace(masterKey))
                return null;

            masterKey = masterKey.Trim();

            // 利用可能な制御工程だけに絞る（最大でも十数件）
            var available = GetAvailableProcesses(productionOrderNumber, itemCode, lotNumber);

            foreach (var p in available)
            {
                // ここは「横持ち変換」前の生データ（MasterKey列が残っている想定）
                var raw = _repo.GetSingleControlHistory(productionOrderNumber, itemCode, lotNumber, p.Id);
                if (raw == null || raw.Rows.Count == 0) continue;

                if (!raw.Columns.Contains("MasterKey")) continue;

                bool hit = raw.Rows.Cast<DataRow>()
                    .Any(r =>
                    {
                        var mk = r["MasterKey"] == DBNull.Value ? null : Convert.ToString(r["MasterKey"]);
                        return string.Equals(mk, masterKey, StringComparison.OrdinalIgnoreCase);
                    });

                if (hit)
                    return p.Id; // ★この工程IDが該当
            }

            return null;
        }
        public sealed class ProcessItem
        {
            public string? Id { get; set; }
            public string? Name { get; set; }  // 表示名
        }

        // ==========================
        // 履歴保存（TSV）
        // ==========================
        private sealed class FilterHistoryEntry
        {
            public string? ContextKey { get; set; }  // 指図|品目コード|ロット
            public string? ProcessId { get; set; }    // 工程ID
            public int UseCount { get; set; }
            public DateTime LastUsedUtc { get; set; }
        }

        private sealed class FilterHistoryStore
        {
            private readonly string _filePath;

            public FilterHistoryStore(string filePath)
            {
                _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
                Directory.CreateDirectory(Path.GetDirectoryName(_filePath)!);
            }

            public List<FilterHistoryEntry> LoadAll()
            {
                if (!File.Exists(_filePath))
                    return new List<FilterHistoryEntry>();

                var lines = File.ReadAllLines(_filePath, Encoding.UTF8);
                var list = new List<FilterHistoryEntry>();

                foreach (var line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    if (line.StartsWith("#")) continue;

                    // TSV: ContextKey \t ProcessId \t UseCount \t LastUsedUtc(ISO)
                    var p = line.Split('\t');
                    if (p.Length < 4) continue;

                    int useCount;
                    if (!int.TryParse(p[2], out useCount)) useCount = 0;

                    DateTime lastUtc;
                    if (!DateTime.TryParse(
                            p[3],
                            CultureInfo.InvariantCulture,
                            DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                            out lastUtc))
                    {
                        lastUtc = DateTime.UtcNow;
                    }

                    list.Add(new FilterHistoryEntry
                    {
                        ContextKey = p[0],
                        ProcessId = p[1],
                        UseCount = useCount,
                        LastUsedUtc = lastUtc
                    });
                }

                return list;
            }
          
            public void SaveAll(List<FilterHistoryEntry> entries)
            {
                entries = entries ?? new List<FilterHistoryEntry>();

                var sb = new StringBuilder();
                sb.AppendLine("#ContextKey\tProcessId\tUseCount\tLastUsedUtc(ISO)");

                foreach (var e in entries)
                {
                    if (e == null) continue;
                    if (string.IsNullOrWhiteSpace(e.ContextKey)) continue;
                    if (string.IsNullOrWhiteSpace(e.ProcessId)) continue;

                    sb.Append(e.ContextKey).Append('\t')
                      .Append(e.ProcessId).Append('\t')
                      .Append(e.UseCount).Append('\t')
                      .Append(e.LastUsedUtc.ToUniversalTime().ToString("o", CultureInfo.InvariantCulture))
                      .AppendLine();
                }

                File.WriteAllText(_filePath, sb.ToString(), Encoding.UTF8);
            }

        }
    }

}
