using System;
using System.Collections.Generic;
using System.Linq;
using static LotTraceApp.Services.LotTraceService;
namespace LotTraceApp.Models
{
    /// <summary>
    /// トレース検索の方向
    /// </summary>
    public enum TraceDirection
    {
        Forward,
        Backward
    }

    /// <summary>
    /// 子ノード探索元
    /// </summary>
    public enum TraceSourceTable
    {
        Unknown = 0,
        SingleControlProcessTable = 1,
        MaterialTableA = 2,
        MaterialTableB = 3
    }

    public enum TraceEdgeDirection
    {
        Unknown = 0,
        ParentToChild = 1,
        ChildToParent = 2
    }

    /// <summary>
    /// MaterialTable_A の系統
    /// </summary>
    public enum MaterialAInputType
    {
        None = 0,
        Drumcan = 1,
        ManualInput = 2
    }

    /// <summary>
    /// コマンドライン / 画面共通の検索条件
    /// </summary>
    public class TraceSearchParameters
    {
        public string ProductionOrderNumber { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }

        public List<string> ResolvedItemCodes { get; set; }

        public string LotNumber { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public TraceDirection Direction { get; set; } = TraceDirection.Forward;
    }

    /// <summary>
    /// 生産結果ノード
    /// </summary>
    /// <summary>
    /// 生産結果ノード
    /// </summary>
    public class ProductionResultNode
    {
        // 表示用
        public string ProductionOrderNumber { get; set; }
        public string LotNumber { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public DateTime? StartDate { get; set; }
        public string StartDateLabel { get; set; }
        public DateTime? EndDate { get; set; }
        public string ManufacturingProcessName { get; set; }
        public string ManufacturingTankName { get; set; }
        public float? Weight { get; set; }

        // 識別補助用
        public string ControlMasterKey { get; set; }

        /// <summary>
        /// 画面互換のため当面残す
        /// 旧ロジックで使用している親LotNo
        /// </summary>
        public string ParentKey { get; set; }

        // 補助情報
        public int Depth { get; set; }
        public string NodeType { get; set; }
        public string ParentMasterKey { get; set; }

        /// <summary>
        /// 系統情報
        /// "A" / "B"
        /// </summary>
        public string RouteSystem { get; set; }

        /// <summary>
        /// A系統の入力枠番号。
        /// B系統では固定値的に扱う場合があるが、
        /// Nodeそのものの実体識別というよりは接続文脈の補助情報として使う。
        /// </summary>
        public int? InputSlotNo { get; set; }

        /// <summary>
        /// A系統の入力種別。
        /// "Drumcan" / "ManualInput"
        /// </summary>
        public string InputSourceType { get; set; }

        /// <summary>
        /// Lotなし等でそのノードを終端扱いにするための補助フラグ。
        /// </summary>
        public bool IsTraceTerminal { get; set; }

        /// <summary>
        /// 色々合ったKey
        /// 現状トレースフォワードとトレースバックで使い方が違うので注意
        ///
        ///トレースバックNodeの識別子として使う。
        ///トレースフォワードも後々同じ調整をする
        /// </summary>
        public string NodeIdentityKey
        {
            get
            {
                string routeSystem = string.IsNullOrWhiteSpace(RouteSystem)
                    ? ""
                    : RouteSystem.Trim().ToUpperInvariant();

                string masterKey = string.IsNullOrWhiteSpace(ControlMasterKey)
                    ? ""
                    : ControlMasterKey.Trim();

                string itemCode = string.IsNullOrWhiteSpace(ItemCode)
                    ? ""
                    : ItemCode.Trim().ToUpperInvariant();

                string lotNumber = string.IsNullOrWhiteSpace(LotNumber)
                    ? ""
                    : LotNumber.Trim().ToUpperInvariant();

                string inputSourceType = string.IsNullOrWhiteSpace(InputSourceType)
                    ? ""
                    : InputSourceType.Trim().ToUpperInvariant();

                string slotPart = InputSlotNo.HasValue
                    ? InputSlotNo.Value.ToString()
                    : "";

                // ------------------------------------------------------------
                // 1. B系統
                // ------------------------------------------------------------
                if (string.Equals(routeSystem, "B", StringComparison.OrdinalIgnoreCase))
                {
                    if (!string.IsNullOrWhiteSpace(masterKey))
                        return "N|B|" + masterKey;

                    // fallback（異常系）
                    return "ERR|B|" + itemCode + "|" + lotNumber;
                }

                // ------------------------------------------------------------
                // 2. A系統
                // ------------------------------------------------------------
                if (string.Equals(routeSystem, "A", StringComparison.OrdinalIgnoreCase))
                {
                    if (!string.IsNullOrWhiteSpace(masterKey))
                        return "N|A|" + masterKey + "|S" + (string.IsNullOrWhiteSpace(slotPart) ? "?" : slotPart);

                    // fallback（異常系）
                    return string.Join("|",
                        "ERR",
                        "A",
                        itemCode,
                        lotNumber,
                        inputSourceType,
                        string.IsNullOrWhiteSpace(slotPart) ? "?" : slotPart);
                }

                // ------------------------------------------------------------
                // 3. 不明系
                // ------------------------------------------------------------
                if (!string.IsNullOrWhiteSpace(masterKey))
                    return "N|U|" + masterKey;

                // 完全異常（でも落とさない）
                return string.Join("|",
                    "ERR",
                    "U",
                    itemCode,
                    lotNumber,
                    inputSourceType,
                    string.IsNullOrWhiteSpace(slotPart) ? "?" : slotPart);
            }
        }

        // ★ 構造
        public List<ProductionResultNode> ParentNodes { get; private set; }
        public List<ProductionResultNode> ChildNodes { get; private set; }

        // ★ 接続情報
        public List<ProductionResultLink> ParentLinks { get; private set; }
        public List<ProductionResultLink> ChildLinks { get; private set; }

        public ProductionResultNode()
        {
            ParentNodes = new List<ProductionResultNode>();
            ChildNodes = new List<ProductionResultNode>();
            ParentLinks = new List<ProductionResultLink>();
            ChildLinks = new List<ProductionResultLink>();
        }
    }

    /// <summary>
    /// ノード間の接続情報。
    ///
    /// 現在の設計方針では、
    /// 「Nodeそのものの実体識別」と
    /// 「どの文脈で親子接続されたか」は分けて扱う。
    ///
    /// - ProductionResultNode
    ///     ノード実体そのものを表す
    /// - ProductionResultLink
    ///     ノード同士の接続文脈を表す
    ///
    /// たとえば A/B 系統差、A系統の投入種別、投入枠番号、
    /// 親判定に使ったLotなどは、ノード実体そのものではなく
    /// 「接続の意味」に属する情報として本クラスで保持する。
    ///
    /// そのため、
    /// 同じ Node 同士の組み合わせでも、
    /// 接続元テーブルや枠番号が異なれば別 Link になり得る。
    /// </summary>
    public class ProductionResultLink
    {
        /// <summary>
        /// 親ノード。
        /// </summary>
        public ProductionResultNode ParentNode { get; set; }

        /// <summary>
        /// 子ノード。
        /// </summary>
        public ProductionResultNode ChildNode { get; set; }

        public TraceEdgeDirection EdgeDirection { get; set; }

        /// <summary>
        /// 親判定に使った親LotNo。
        ///
        /// A/B の接続判定時に、
        /// 「どのLotを使ってこの接続が成立したか」を残すための値。
        /// ノード実体識別子ではなく、接続文脈の一部。
        /// </summary>
        public string ParentLotNumber { get; set; }

        /// <summary>
        /// トレースグラフ上の辺の方向。
        ///
        /// TraceDirection が「検索の向き」を表すのに対して、
        /// こちらは Node 間を結ぶ Edge 自体の向きを表す。
        ///
        /// 例:
        /// - ParentToChild
        ///     工程・構造上の正方向（上流 → 下流）
        /// - ChildToParent
        ///     必要に応じて逆向き表現を明示したい場合に使用
        ///
        /// 現在の基本方針では、内部DAGは ParentToChild
        /// （上流 → 下流）で保持する想定。
        /// </summary>
        

        /// <summary>
        /// 接続元テーブル。
        ///
        /// 例:
        /// - MaterialTableA
        /// - MaterialTableB
        /// - SingleControlProcessTable
        ///
        /// 同じ親子ノードでも SourceTable が異なれば、
        /// 別の接続として扱う余地がある。
        /// </summary>
        public TraceSourceTable SourceTable { get; set; }

        /// <summary>
        /// MaterialTable_A 用の入力種別。
        /// - Drumcan
        /// - ManualInput
        ///
        /// MaterialTable_B や SCP では None。
        /// これもノード実体識別ではなく、接続文脈の一部。
        /// </summary>
        public MaterialAInputType MaterialAInputType { get; set; }

        /// <summary>
        /// 接続枠番号。
        ///
        /// - A系統: MaterialTable_A の XX 枠番号
        /// - B系統: 現状は固定枠番号として扱うことがある
        ///
        /// A系統では同一 MasterKey でも SlotNo が異なれば
        /// 表示上・接続上は別文脈として扱うことがあるため、
        /// Link 側で保持する。
        /// </summary>
        public int SlotNo { get; set; }

        /// <summary>
        /// 接続の重複排除・経路識別用キー。
        ///
        /// これは Node の一意識別子ではなく、
        /// あくまで「この接続をどう識別するか」のためのキー。
        ///
        /// 想定用途:
        /// - 同一接続の重複追加防止
        /// - Path 構築時の経路識別
        /// - 経路探索時のリンク一意性判定
        ///
        /// 親Node・子Node に加えて、
        /// SourceTable / MaterialAInputType / SlotNo / ParentLotNumber など
        /// 接続文脈を含めて構成される想定。
        /// </summary>
        public string LinkIdentityKey { get; set; }
    }

    public enum TraceDisplayColumnKind
    {
        Start = 0,
        Middle = 1,
        End = 2
    }

    public enum TraceDisplayRowKind
    {
        Normal = 0
    }

    public class TraceDisplayBuildOptions
    {
        public bool SuppressDuplicateStartCells { get; set; } = true;
        public bool SuppressDuplicateMiddleCells { get; set; } = true;
        public bool SuppressDuplicateEndCells { get; set; } = true;
        public bool SortBeforeSuppress { get; set; } = true;
    }

    /// <summary>
    /// 画面表示用セルデータ。
    ///
    /// 本クラスは「ノード実体」ではなく、
    /// あくまで「特定の経路文脈における表示単位」を表す。
    ///
    /// 設計上の役割分担：
    ///
    /// - ProductionResultNode
    ///     ノードの実体（データそのもの）
    ///
    /// - ProductionResultLink
    ///     ノード間の接続文脈
    ///
    /// - TraceDisplayCell（本クラス）
    ///     表示時の「位置」「経路文脈」「抑制制御」
    ///
    /// 同一Nodeでも、経路が異なれば別Cellとして複数存在する。
    /// </summary>
    public class TraceDisplayCell
    {

        /// <summary>
        /// ★フォワード専用
        /// このセルが属するLotグループキー。
        /// </summary>
        public string LotGroupKey { get; set; }

        /// <summary>
        /// このセルが代表枝かどうか。
        /// </summary>
        public bool IsRepresentativeInLotGroup { get; set; }

        /// <summary>
        /// 下流枝署名。
        /// </summary>
        public string BranchSignature { get; set; }

        /// <summary>
        /// 元ノード実体。
        /// </summary>
        public ProductionResultNode Node { get; set; }

        /// <summary>
        /// MasterKey（表示・補助用）。
        /// </summary>
        public string MasterKey { get; set; }

        /// <summary>
        /// 表示用ノードキー。
        ///
        /// 重要：
        /// これはノード統合キーではない。
        ///
        /// BuildNodeOnlyKey() ベースで生成され、
        /// 「表示上このセルをどう識別するか」のためのキー。
        ///
        /// ノードの一意性はサービス側の GetNodeMergeKey() に依存する。
        /// </summary>
        public string NodeKey { get; set; }

        /// <summary>
        /// 中間レベル（Lv）。
        /// Start=0, Middle=1..N, End=-1 などで使用。
        /// </summary>
        public int Level { get; set; }

        public bool EndIsDuplicate { get; set; }
        public int? EndDuplicateDisplayGroupIndex { get; set; }

        /// <summary>
        /// 列種別（Start / Middle / End）。
        /// </summary>
        public TraceDisplayColumnKind ColumnKind { get; set; }

        // ===== 表示項目 =====

        public string ProductionOrderNumber { get; set; }
        public string LotNumber { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public DateTime? StartDate { get; set; }
        public string StartDateLabel { get; set; }
        public decimal? Weight { get; set; }

        /// <summary>
        /// 表示フラグ（抑制制御）。
        /// </summary>
        public bool IsVisible { get; set; }

        /// <summary>
        /// 抑制理由（デバッグ用）。
        /// </summary>
        public string SuppressReason { get; set; }

        /// <summary>
        /// 旧互換用。
        /// </summary>
        public string ParentMasterKey { get; set; }
    }


    public class TraceDisplayRow
    {
        public string RouteSystem { get; set; }
        public int DisplayOrder { get; set; }

        public bool IsDisplayTarget { get; set; }
        public string SuppressReason { get; set; }

        public TraceDisplayCell Start { get; set; }
        public List<TraceDisplayCell> Middles { get; private set; }
        public TraceDisplayCell End { get; set; }

        /// <summary>
        /// 始点グループ末尾か。
        /// </summary>
        public bool IsLastRowOfStartGroup { get; set; }

        /// <summary>
        /// ★フォワード専用
        /// 各レベルのLotグループ情報。
        /// </summary>
        public List<TraceLotGroupInfo> LevelLotGroups { get; private set; }

        /// <summary>
        /// 剪定対象。
        /// </summary>
        public bool IsPruned { get; set; }

        /// <summary>
        /// 剪定理由。
        /// </summary>
        public string PruneReason { get; set; }

        public TraceDisplayRow()
        {
            Middles = new List<TraceDisplayCell>();

            // ★追加
            LevelLotGroups = new List<TraceLotGroupInfo>();
        }
    }

    /// <summary>
    /// UI線描画用のノードレンジ情報。
    ///
    /// 目的：
    /// MainForm がこれまで内部で生成していた
    /// NodeRenderRange をサービス側で構築し、
    /// UI はそれを受け取って描画だけ行うための受け皿。
    ///
    /// 注意：
    /// これは表示用のレンジ情報であり、
    /// Node の同一性判定そのものを担う型ではない。
    /// Node の同一性は引き続きサービス側の MergeKey 系で扱う。
    /// </summary>
    public class NodeRenderRange
    {
        public string NodeKey { get; set; }

        /// <summary>
        /// 自分自身が表示されている行
        /// </summary>
        public int RowIndex { get; set; }

        /// <summary>
        /// 自分の子が表示されている行一覧
        /// </summary>
        public List<int> ChildRowIndices { get; private set; }

        public int ChildCount
        {
            get { return ChildRowIndices == null ? 0 : ChildRowIndices.Count; }
        }

        public int? FirstChildRowIndex
        {
            get
            {
                if (ChildRowIndices == null || ChildRowIndices.Count == 0)
                    return null;

                return ChildRowIndices.Min();
            }
        }

        public int? LastChildRowIndex
        {
            get
            {
                if (ChildRowIndices == null || ChildRowIndices.Count == 0)
                    return null;

                return ChildRowIndices.Max();
            }
        }

        public NodeRenderRange()
        {
            ChildRowIndices = new List<int>();
        }
    }

    /// <summary>
    /// UI線描画用の中グリッド木レンジ情報。
    ///
    /// 目的：
    /// MainForm がこれまで内部で生成していた
    /// MiddleTreeRenderRange をサービス側で構築し、
    /// UI はそれを受け取って描画だけ行うための受け皿。
    ///
    /// 注意：
    /// これは Middle 側の表示レンジ情報であり、
    /// 幹・枝の太さ計算も今後はサービス側で確定する前提。
    /// </summary>
    public class MiddleTreeRenderRange
    {
        public string GroupKey { get; set; }
        public int Level { get; set; }

        public string NodeKey { get; set; }
        /// <summary>
        /// このノード自身が最初に現れる行
        /// </summary>
        public int StartRowIndex { get; set; }

        /// <summary>
        /// このノード配下の末端行
        /// </summary>
        public int EndRowIndex { get; set; }

        public bool IsTrunk
        {
            get { return Level == 1; }
        }
    }

    public class TraceDisplayResult
    {
        public List<TraceDisplayRow> Rows { get; private set; }
        public int MaxMiddleDepth { get; set; }
        public TraceDisplayBuildOptions Options { get; set; }

        /// <summary>
        /// UI線描画用のノードレンジ情報。
        ///
        /// MainForm がこれまで BuildNodeRenderRanges(...) で
        /// 生成していた結果を、今後はサービス側で構築して保持する。
        /// UI 側ではこの結果を受け取り、そのまま描画に使用する。
        /// </summary>
        public Dictionary<string, NodeRenderRange> NodeRenderRanges { get; private set; }

        /// <summary>
        /// UI線描画用の中グリッド木レンジ情報。
        ///
        /// MainForm がこれまで BuildMiddleTreeRenderRanges(...) で
        /// 生成していた結果を、今後はサービス側で構築して保持する。
        /// UI 側ではこの結果を受け取り、そのまま描画に使用する。
        /// </summary>
        public Dictionary<string, MiddleTreeRenderRange> MiddleTreeRenderRanges { get; private set; }

        public List<TraceLineRange> LineRanges { get; } = new List<TraceLineRange>();

        public TraceDisplayResult()
        {
            Rows = new List<TraceDisplayRow>();
            Options = new TraceDisplayBuildOptions();

            NodeRenderRanges =
                new Dictionary<string, NodeRenderRange>(StringComparer.OrdinalIgnoreCase);

            MiddleTreeRenderRanges =
                new Dictionary<string, MiddleTreeRenderRange>(StringComparer.OrdinalIgnoreCase);
        }
    }

    public class TraceGridDrawContext
    {
        public StartGridDrawInfo Start { get; set; }
        public MiddleGridDrawInfo Middle { get; set; }
        public EndGridDrawInfo End { get; set; }

        public int RowCount { get; set; }

        public TraceGridDrawContext()
        {
            Start = new StartGridDrawInfo();
            Middle = new MiddleGridDrawInfo();
            End = new EndGridDrawInfo();
        }
    }

    public class StartGridDrawInfo
    {
        public List<StartRowDrawInfo> Rows { get; private set; }

        public StartGridDrawInfo()
        {
            Rows = new List<StartRowDrawInfo>();
        }
    }

    public class StartRowDrawInfo
    {
        public int RowIndex { get; set; }
        public bool DrawBottomDivider { get; set; }
    }

    public class MiddleGridDrawInfo
    {
        public List<MiddleHorizontalLineDrawInfo> HorizontalLines { get; private set; }
        public List<MiddleVerticalLineDrawInfo> VerticalLines { get; private set; }

        public MiddleGridDrawInfo()
        {
            HorizontalLines = new List<MiddleHorizontalLineDrawInfo>();
            VerticalLines = new List<MiddleVerticalLineDrawInfo>();
        }
    }

    public class MiddleHorizontalLineDrawInfo
    {
        public int StartRowIndex { get; set; }
        public int EndRowIndex { get; set; }
        public int FromXLevel { get; set; }
        public int ToXLevel { get; set; }
        public string LineKind { get; set; }
    }

    public class MiddleVerticalLineDrawInfo
    {
        public int XLevel { get; set; }
        public string LineKind { get; set; }
        public bool IncludeHeaderArea { get; set; }
    }

    public class EndGridDrawInfo
    {
        public List<EndHorizontalLineDrawInfo> HorizontalLines { get; private set; }

        public EndGridDrawInfo()
        {
            HorizontalLines = new List<EndHorizontalLineDrawInfo>();
        }
    }

    public class EndHorizontalLineDrawInfo
    {
        public int StartRowIndex { get; set; }
        public int EndRowIndex { get; set; }
        public string LineKind { get; set; }
    }


    /// <summary>
    /// 原材料（ItemCode + LotNumber のペア）
    /// </summary>
    public class MaterialPair : IEquatable<MaterialPair>
    {
        public string ItemCode { get; set; }
        public string LotNumber { get; set; }

        public MaterialPair() { }

        public MaterialPair(string itemCode, string lotNumber)
        {
            ItemCode = itemCode;
            LotNumber = lotNumber;
        }

        public override string ToString()
        {
            return string.Format("{0}|{1}", ItemCode, LotNumber);
        }

        public bool Equals(MaterialPair other)
        {
            if (other == null) return false;

            return string.Equals(ItemCode, other.ItemCode, StringComparison.OrdinalIgnoreCase)
                && string.Equals(LotNumber, other.LotNumber, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as MaterialPair);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((ItemCode == null ? 0 : ItemCode.ToLowerInvariant().GetHashCode()) * 397) ^
                       (LotNumber == null ? 0 : LotNumber.ToLowerInvariant().GetHashCode());
            }
        }
    }

    /// <summary>
    /// トレース結果
    /// 旧画面互換のため Start/Middle/End を残しつつ、
    /// 今後は RootNodes / AllNodes / AllLinks を主に使う
    /// </summary>
    public class TraceResult
    {
        // 旧互換
        public List<ProductionResultNode> StartNodes { get; private set; }
        public List<ProductionResultNode> MiddleNodes { get; private set; }
        public List<ProductionResultNode> EndNodes { get; private set; }

        // 新構造
        public List<ProductionResultNode> RootNodes { get; private set; }
        public List<ProductionResultNode> AllNodes { get; private set; }
        public List<ProductionResultLink> AllLinks { get; private set; }
        public List<TracePathRow> PathRows { get; private set; }

        // 罫線橋渡し
        public List<TraceLineRange> TraceLineRanges { get; private set; }


        public TraceResult()
        {
            StartNodes = new List<ProductionResultNode>();
            MiddleNodes = new List<ProductionResultNode>();
            EndNodes = new List<ProductionResultNode>();

            RootNodes = new List<ProductionResultNode>();
            AllNodes = new List<ProductionResultNode>();
            AllLinks = new List<ProductionResultLink>();
            PathRows = new List<TracePathRow>();
            TraceLineRanges = new List<TraceLineRange>();
        }

        public bool IsEmpty
            
        {
            get
            {
                return PathRows.Count == 0;
            }
        }

    }



    /// <summary>
    /// 交点検出結果 1 行分
    /// </summary>
    public class CrossPointRecord
    {
        public CrossPointRecord()
        {
            TabPresence = new Dictionary<int, int>();
        }

        /// <summary>
        /// NodeIdentityKey 由来のNode識別子。
        /// 交点判定、通常グリッドのセル強調に使用する。
        /// </summary>
        public string NodeKey { get; set; }

        /// <summary>
        /// 複数の対象タブに存在する場合は 1、それ以外は 0。
        /// CSV/Excel出力で扱いやすいよう数値で保持する。
        /// </summary>
        public int CrossPointFlag { get; set; }

        public string ProductionOrderNumber { get; set; }
        public string LotNumber { get; set; }
        public string ItemName { get; set; }
        public string StartDateText { get; set; }
        public float? Weight { get; set; }

        /// <summary>
        /// 内部保持用。画面/CSV/Excelでは対象タブだけを列化し、
        /// 値は必ず 1 または 0 とする。
        /// </summary>
        public Dictionary<int, int> TabPresence { get; private set; }

        public int GetTabPresence(int tabNo)
        {
            int value;
            return TabPresence != null && TabPresence.TryGetValue(tabNo, out value)
                ? value
                : 0;
        }
    }

/// <summary>
/// Repository → Service 間で使用する親子関係構造。
/// Serviceが枝構造を作る際の根拠情報となる。
/// </summary>
public class ChildCandidate
    {
        /// <summary>
        /// 子ノード実体（Repository内で完成させる）。
        /// </summary>
        public ProductionResultNode ChildNode { get; set; }
        /// <summary>
        /// 親ノード実体（Repositories内で完成させる
        /// </summary>
        public ProductionResultNode PearentNode { get; set; }
        /// <summary>
        /// BFSの再探索キーになる
        /// Repository内での使用
        /// </summary>
        public string ChildLotNumber { get; set; }

        /// <summary>
        /// 親子間の接続文脈を表す。
        ///Repository内で完成させる（子Lot+親Lot)
        /// </summary>
        public string RelationKey { get; set; }
        /// <summary>
        /// 念のため。主にダンプ用途
        /// </summary>
        public string DebugSource { get; set; }
    }

    public class TracePathRow
    {
        public ProductionResultNode StartNode { get; set; }
        public List<ProductionResultNode> MiddleNodes { get; private set; }
        public ProductionResultNode EndNode { get; set; }
        public List<ProductionResultLink> PathLinks { get; private set; }

        public bool EndIsDuplicate { get; set; }
        public int? EndDuplicateDisplayGroupIndex { get; set; }

        /// <summary>
        /// 経路の安定順序。
        /// </summary>
        public int PathOrder { get; set; }

        /// <summary>
        /// StartNodeから見た下流枝署名。
        /// </summary>
        public string StartBranchSignature { get; set; }

        /// <summary>
        /// ★フォワード専用
        /// 各レベルのLotグループ情報。
        /// </summary>
        public List<TraceLotGroupInfo> LevelLotGroups { get; private set; }

        /// <summary>
        /// 剪定対象フラグ。
        /// </summary>
        public bool IsPruned { get; set; }

        /// <summary>
        /// 剪定理由。
        /// </summary>
        public string PruneReason { get; set; }

        public TracePathRow()
        {
            MiddleNodes = new List<ProductionResultNode>();
            PathLinks = new List<ProductionResultLink>();

            // ★追加
            LevelLotGroups = new List<TraceLotGroupInfo>();
        }
    }

    #region トレースフォワード枝剪定新モデル群

    /// <summary>
    /// グルーピング対象軸。
    ///
    /// ★重要：
    /// 本グルーピングは「トレースフォワード専用」の概念である。
    /// トレースバックでは使用しないこと。
    ///
    /// Start / Middle / End を同一ロジックで扱うための軸定義。
    /// </summary>
    public enum TraceGroupAxis
    {
        Unknown = 0,
        Start = 1,
        Middle = 2,
        End = 3
    }

    /// <summary>
    /// 同一Lv・同一LotNo の塊（グループ）情報。
    ///
    /// ★最重要ルール：
    /// 本クラスは「トレースフォワード専用」の構造メタである。
    /// トレースバックでは一切使用しないこと。
    ///
    /// ----------------------------------------
    /// ■目的
    /// ----------------------------------------
    /// - 同一LotNoを持つNode群を「一塊」として扱う
    /// - 塊内のNodeは表示対象に残す
    /// - ただし、その先に続く枝は代表1本のみ採用する
    ///
    /// ----------------------------------------
    /// ■適用範囲
    /// ----------------------------------------
    /// - StartNode
    /// - MiddleNode（同一Lv）
    ///
    /// ※EndNodeには通常適用しない
    ///
    /// ----------------------------------------
    /// ■例
    /// ----------------------------------------
    /// Lv2で Lot=ABC のNodeが3件ある場合：
    /// → 3行とも表示はする
    /// → ただし子ノード展開は1行だけ（IsRepresentative=true）
    ///
    /// ----------------------------------------
    /// ■注意
    /// ----------------------------------------
    /// - Nodeの一意性とは無関係（NodeIdentityKeyとは別概念）
    /// - 「表示構造」と「枝の採用制御」のためのメタ情報
    /// </summary>
    public class TraceLotGroupInfo
    {
        /// <summary>
        /// 軸（Start / Middle / End）。
        /// </summary>
        public TraceGroupAxis Axis { get; set; }

        /// <summary>
        /// レベル。
        /// Start=0, Middle=1..N, End=-1 を想定。
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// 同一塊の識別キー。
        ///
        /// 基本構成：
        /// Axis + Level + LotNumber
        /// </summary>
        public string GroupKey { get; set; }

        /// <summary>
        /// グループ代表LotNo。
        /// </summary>
        public string LotNumber { get; set; }

        /// <summary>
        /// グループ単位の表示順。
        /// </summary>
        public int GroupOrder { get; set; }

        /// <summary>
        /// グループ内の行順。
        /// </summary>
        public int RowOrderInGroup { get; set; }

        /// <summary>
        /// この行がグループ先頭かどうか。
        /// </summary>
        public bool IsFirstRowOfGroup { get; set; }

        /// <summary>
        /// この行がグループ末尾かどうか。
        /// </summary>
        public bool IsLastRowOfGroup { get; set; }

        /// <summary>
        /// この行が代表枝かどうか。
        ///
        /// true の行のみ、次の枝（子Node）を展開する。
        /// </summary>
        public bool IsRepresentative { get; set; }

        /// <summary>
        /// 代表枝の識別キー。
        /// （デバッグ・再現性確保用）
        /// </summary>
        public string RepresentativeKey { get; set; }

        /// <summary>
        /// この地点から見た下流枝の署名。
        ///
        /// 同内容枝の比較・剪定に使用する。
        /// </summary>
        public string DownstreamBranchSignature { get; set; }

        /// <summary>
        /// 上流からここまでの経路署名。
        /// </summary>
        public string UpstreamBranchSignature { get; set; }

        /// <summary>
        /// 剪定対象かどうか。
        /// </summary>
        public bool IsPruned { get; set; }

        /// <summary>
        /// 剪定理由（デバッグ用）。
        /// </summary>
        public string PruneReason { get; set; }

        /// <summary>
        /// 描画用：上境界線を引くか。
        /// </summary>
        public bool DrawDividerTop { get; set; }

        /// <summary>
        /// 描画用：下境界線を引くか。
        /// </summary>
        public bool DrawDividerBottom { get; set; }
    }

    #endregion

    #region トレースバック用新モデル群

    /// <summary>
    /// トレースバック専用の親候補中間モデル。
    ///
    /// 目的:
    /// - Repository が「current から辿れる親候補」を Service に渡すための共通器
    /// - Service が Node統合 / Link形成 / BFS / 枝構造化 にそのまま使える形にする
    ///
    /// 注意:
    /// - このモデルはフォワード用 ChildCandidate の代替であり、バック専用
    /// - Node は「親候補Node」
    /// - ChildLotNumber は「この親候補を見つけたときの子側Lot」
    /// </summary>
    public sealed class BackwardParentCandidate
    {
        /// <summary>
        /// 親候補として採用する Node 実体。
        ///
        /// Repository側責務:
        /// - A/B 各系統の抽出結果を ProductionResultNode に詰めてここへ入れる
        /// - この Node は Service で GetOrAddNode に渡される
        ///
        /// Service側責務:
        /// - Node統合
        /// - Depth設定
        /// - 親子Link形成
        /// </summary>
        public ProductionResultNode Node { get; set; }

        
        /// <summary>
        /// この親候補が接続される子Node実体。
        ///
        /// トレースバックでは「ある子Nodeに対して複数の親Node候補を接続していく」。
        /// そのため BackwardParentCandidate は、親候補Nodeだけでなく、
        /// 「どの子Nodeに対する親候補か」を明示的に保持しなければならない。
        ///
        /// RelationKey はこの ChildNode と親候補Node の関係性を一意に表すため、
        /// 親Nodeから見た子Node実体情報の参照元として本プロパティを使用する。
        /// </summary>
        public ProductionResultNode ChildNode { get; set; }

        /// <summary>
        /// リポジトリ内で親探索に使用するプロパティ
        /// </summary>
        public string ParentLotNumber { get; set; }

        /// <summary>
        /// Repositoriesで確定する１世代の関係文脈キー
        /// 枝の束ね用途だが、あまり依存しないように。
        /// </summary>
        public string RelationKey { get; set; }
        

        /// <summary>
        /// デバッグ用の自由記述補助。
        ///
        /// 用途:
        /// - STEP1/STEP2 のどの経路で見つかったか
        /// - A/B どちらのクエリ由来か
        /// - 一時的な切り分け
        ///
        /// 本番ロジックの判定には使わないこと。
        /// </summary>
        public string DebugSource { get; set; }

     
       
      
        
    }
    #endregion
    /// <summary>
    /// 単一制御工程履歴 1件分
    /// </summary>
    public class SingleControlHistoryModel
    {
        public string MasterKey { get; set; }
        public string ForeignKey { get; set; }
        public string DataCategory { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string ManufacturingProcessName { get; set; }

        // 既存項目
        public decimal? PumpDelayTime { get; set; }
        public decimal? ValveDelayTime { get; set; }
        public decimal? CirculationPumpStartupSpeed { get; set; }
        public decimal? CirculationPumpStopWeight { get; set; }
        public int? NumberOfWashes { get; set; }
        public decimal? CirculationLineCleaningTime { get; set; }
        public decimal? BypassLineCirculationTime { get; set; }
        public decimal? ShowerBallCleaningTime { get; set; }
        public decimal? CirculatingLineFrequencySettingValue { get; set; }
        public decimal? BypassLineFrequencySettingValue { get; set; }
        public decimal? ShowerBallFrequencySettingValue { get; set; }
        public bool? SamplingFlag { get; set; }
        public bool? ShowerBallLineFlag { get; set; }
        public string FillingMachineValveOpeningAndClosingPattern { get; set; }
        public decimal? N2PressTimer1 { get; set; }
        public decimal? N2PressTimer2 { get; set; }
        public decimal? N2PressTimer3 { get; set; }

        // 追加項目
        public decimal? CirculationPumpFrequency { get; set; }
        public decimal? AirRemovalTime1 { get; set; }
        public decimal? AirRemovalTime2 { get; set; }

        public decimal? Weight { get; set; }
        public decimal? LiquidLevel { get; set; }

        public decimal? AgitatorFrequency { get; set; }
        public decimal? MixerOperationTime { get; set; }
        public decimal? MixerStopWeight { get; set; }

        public string SourceTankName { get; set; }
        public decimal? SourceTankPumpStartupSpeed { get; set; }
        public decimal? SourceTankPump1tageFrequency { get; set; }
        public decimal? SourceTankPump2stageFrequency { get; set; }
        public decimal? SourceTankPump2stageFrequencySwitchingWeight { get; set; }
        public decimal? AmountOfStock { get; set; }
        public decimal? ValveHalfopenWeight { get; set; }
        public decimal? DroopAmountSettingValue { get; set; }
        public decimal? ControlCompletionDelayTime { get; set; }

        public decimal? CirculationPumpOperationTime { get; set; }
        public string PressureSelection { get; set; }
        public decimal? CirculationPumpTargetFrequency { get; set; }
        public decimal? CirculationPumpStartupTime { get; set; }
        public decimal? PressureTargetValue { get; set; }
        public decimal? CirculationPumpStartupSpeed1 { get; set; }
        public decimal? TargetUpperLimitPressure { get; set; }
        public decimal? TargetLowerLimitPressure { get; set; }
        public decimal? CirculationPumpStartupSpeed2 { get; set; }
        public decimal? CorrectionUpperLimitPressure { get; set; }
        public decimal? CorrectionLowerLimitPressure { get; set; }
        public decimal? CorrectionSpeed { get; set; }
        public decimal? Timer { get; set; }
        public decimal? UpperLimitAlarmPressure_H { get; set; }
        public decimal? LowerLimitAlarmPressure_L { get; set; }
        public decimal? UpperLimitAlarmPressure_HH { get; set; }
        public decimal? LowerLimitAlarmPressure_LL { get; set; }
        public bool? WeightMonitoring { get; set; }

        public decimal? WashingPumpStartupSpeed { get; set; }
        public decimal? WashingPumpFrequency { get; set; }

        public decimal? FlowTargetValue { get; set; }
        public decimal? TargetUpperLimitFlow { get; set; }
        public decimal? TargetLowerLimitFlow { get; set; }
        public decimal? CorrectionUpperLimitFlow { get; set; }
        public decimal? CorrectionLowerLimitFlow { get; set; }
        public decimal? UpperLimitAlarmFlow_HH { get; set; }
        public decimal? UpperLimitAlarmFlow_H { get; set; }
        public decimal? LowerLimitAlarmFlow_L { get; set; }

        public decimal? CirculationPumpFrequencySetting_L { get; set; }
        public decimal? WasteVolume { get; set; }

        public decimal? AmountOfExtrction { get; set; }
        public decimal? TotalDischargePumpStopDelayTime { get; set; }

        public string WasteLiquidTypeFlag { get; set; }

        public decimal? ManholeLockTimer { get; set; }
        public decimal? ManholeAlarmTimer { get; set; }
        public decimal? ManholeOpeningTime { get; set; }
        public decimal? ManholeClosingTime { get; set; }
        public decimal? ManholeOpenedHours { get; set; }
        public int? NumberOfManholesOpenedAndClosed { get; set; }

        /// <summary>
        /// フィルタ履歴 1件分
        /// </summary>
        public class FilterHistoryModel
        {
            public string MasterKey { get; set; }
            public string ForeignKey { get; set; }
            public string DataCategory { get; set; }

            public string FilterItemCode1 { get; set; }
            public int? FilterSetNumber1 { get; set; }
            public string FilterItemCode2 { get; set; }
            public int? FilterSetNumber2 { get; set; }

            public string FilterLotNumber01 { get; set; }
            public string FilterLotNumber02 { get; set; }
            public string FilterLotNumber03 { get; set; }
            public string FilterLotNumber04 { get; set; }
            public string FilterLotNumber05 { get; set; }
            public string FilterLotNumber06 { get; set; }
            public string FilterLotNumber07 { get; set; }
            public string FilterLotNumber08 { get; set; }
            public string FilterLotNumber09 { get; set; }
            public string FilterLotNumber10 { get; set; }
            public string FilterLotNumber11 { get; set; }
            public string FilterLotNumber12 { get; set; }
            public string FilterLotNumber13 { get; set; }
            public string FilterLotNumber14 { get; set; }
            public string FilterLotNumber15 { get; set; }
            public string FilterLotNumber16 { get; set; }
            public string FilterLotNumber17 { get; set; }
            public string FilterLotNumber18 { get; set; }
            public string FilterLotNumber19 { get; set; }
            public string FilterLotNumber20 { get; set; }
            public string FilterLotNumber21 { get; set; }
            public string FilterLotNumber22 { get; set; }
            public string FilterLotNumber23 { get; set; }
            public string FilterLotNumber24 { get; set; }
            public string FilterLotNumber25 { get; set; }
            public string FilterLotNumber26 { get; set; }
            public string FilterLotNumber27 { get; set; }
            public string FilterLotNumber28 { get; set; }
            public string FilterLotNumber29 { get; set; }
            public string FilterLotNumber30 { get; set; }
            public string FilterLotNumber31 { get; set; }
            public string FilterLotNumber32 { get; set; }
            public string FilterLotNumber33 { get; set; }
            public string FilterLotNumber34 { get; set; }
            public string FilterLotNumber35 { get; set; }
            public string FilterLotNumber36 { get; set; }
            public string FilterLotNumber37 { get; set; }
            public string FilterLotNumber38 { get; set; }
            public string FilterLotNumber39 { get; set; }
            public string FilterLotNumber40 { get; set; }



        }
    }
}




