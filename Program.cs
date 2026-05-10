using System;
using System.IO;
using System.Windows.Forms;
using LotTraceApp.Models;
using LotTraceApp.Repositories;
using LotTraceApp.Services;
using LotTraceApp.Utils;

namespace LotTraceApp
{
    internal static class Program
    {
        // 仕様書 7.7 の出力先例
        private const string ExportDirectory = @"C:\FA-Server\DB_TRACE\Export";

        [STAThread]
        private static void Main(string[] args)
        {
            // 実行ファイルと同じフォルダにある INI ファイルを想定
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string iniPath = Path.Combine(baseDir, "LotTraceApp.ini");

            IniFile ini;
            try
            {
                ini = new IniFile(iniPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("INI ファイルの読み込みに失敗しました。\r\n" +
                                "Path: " + iniPath + "\r\n" +
                                "Error: " + ex.Message,
                                "起動エラー",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                return;
            }

            // 接続文字列を INI から取得
            string connLiquid = ini.GetValue("ConnectionStrings", "Mes31");
            string connBottle = ini.GetValue("ConnectionStrings", "Mes33");

            if (string.IsNullOrEmpty(connLiquid))
            {
                MessageBox.Show("INI ファイルに接続文字列 Mes31 が定義されていません。",
                                "起動エラー",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                return;
            }
            if (string.IsNullOrEmpty(connBottle))
            {
                MessageBox.Show("INI ファイルに接続文字列 Mes33 が定義されていません。",
                                "起動エラー",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                return;
            }

            var liquidRepo = new LotTraceRepository(connLiquid);
            var customerItemRepo = new CustomerItemMasterRepository(ini);
            var liquidService = new LotTraceService(liquidRepo, customerItemRepo);

            var resultRepo = new ResultRepositories(connLiquid);
            var resultService = new ResultService(resultRepo, customerItemRepo);
            
            var bottleRepo = new BottleTraceRepository(connBottle);
            var bottleService = new BottleTraceService(bottleRepo);

            // コマンドライン引数がある場合は、液設備側で検索→CSV出力のみを行う
            if (args != null && args.Length > 0)
            {
                RunCommandLine(args, liquidService);
                return;
            }

            // 通常起動（GUI）
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm(liquidService, bottleService, resultService));
        }

        /// <summary>
        /// コマンドライン起動用：液設備のロットトレースを実行し、CSV を出力して終了
        /// </summary>
        private static void RunCommandLine(string[] args, LotTraceService liquidService)
        {
            var p = new TraceSearchParameters();
            int mode = 1; // 1,2,3 を想定（この例ではすべて CSV 出力して終了）

            // 簡易パラメータ解析
            foreach (string raw in args)
            {
                if (string.IsNullOrEmpty(raw)) continue;

                if (raw.Equals("-F", StringComparison.OrdinalIgnoreCase))
                {
                    p.Direction = TraceDirection.Forward;
                }
                else if (raw.Equals("-B", StringComparison.OrdinalIgnoreCase))
                {
                    p.Direction = TraceDirection.Backward;
                }
                else if (raw.StartsWith("-S:", StringComparison.OrdinalIgnoreCase))
                {
                    p.ProductionOrderNumber = raw.Substring(3);
                }
                else if (raw.StartsWith("-N:", StringComparison.OrdinalIgnoreCase))
                {
                    p.ItemName = raw.Substring(3);
                }
                else if (raw.StartsWith("-C:", StringComparison.OrdinalIgnoreCase))
                {
                    p.ItemCode = raw.Substring(3);
                }
                else if (raw.StartsWith("-L:", StringComparison.OrdinalIgnoreCase))
                {
                    p.LotNumber = raw.Substring(3);
                }
                else if (raw.StartsWith("-M:", StringComparison.OrdinalIgnoreCase))
                {
                    int m;
                    if (int.TryParse(raw.Substring(3), out m))
                    {
                        mode = m;
                    }
                }
            }

            // 検索実行
            TraceResult result = liquidService.ExecuteTrace(p);

            // 出力ディレクトリ作成
            try
            {
                if (!Directory.Exists(ExportDirectory))
                {
                    Directory.CreateDirectory(ExportDirectory);
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("出力フォルダ作成に失敗しました: " + ex.Message);
                return;
            }

            string fileName = "Lottr" + DateTime.Now.ToString("yyMMddHHmmss") + ".csv";
            string path = Path.Combine(ExportDirectory, fileName);

            try
            {
                ExportHelper.ExportTraceResultToCsv(result, path);
                Console.WriteLine("CSV exported: " + path);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("CSV 出力に失敗しました: " + ex.Message);
            }

            // mode(1/2/3)に応じた GUI 表示などは、この簡易版では実装していない
        }
    }
}