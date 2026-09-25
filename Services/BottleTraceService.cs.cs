using LotTraceApp.Forms;
using LotTraceApp.Models;
using LotTraceApp.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;


namespace LotTraceApp.Services
{
    /// <summary>
    /// 瓶設備ロットトレースの実装
    /// 図 7.2 / 7.4 のフローをそのままコード化（単段検索）
    /// </summary>
    public class BottleTraceService
    {
        private readonly BottleTraceRepository _repo;
        private readonly ICustomerItemMasterRepository _customerItemMasterRepository;

     
        public BottleTraceService(BottleTraceRepository repo, ICustomerItemMasterRepository customerItemMasterRepository)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _customerItemMasterRepository = customerItemMasterRepository;
        }

        private bool ResolveItemNameCondition(TraceSearchParameters p)
        {
            if (p == null)
                return true;

            if (string.IsNullOrWhiteSpace(p.ItemName))
                return true;

            p.ResolvedItemCodes =
                _customerItemMasterRepository.GetItemCodeByName(p.ItemName);

            p.ItemCode = null; // ItemName優先

            return p.ResolvedItemCodes != null &&
                   p.ResolvedItemCodes.Count > 0;
        }

        #region フォワード

        public BottleTraceResult BottleTraceForwardResult(
            TraceSearchParameters? p,
            IProgress<TraceProgressState>? progress = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (p == null) throw new ArgumentNullException("p");

            cancellationToken.ThrowIfCancellationRequested();
            ReportProgress(progress, "品目名条件を解決しています...", 12);

            if (!ResolveItemNameCondition(p))
            {
                return new BottleTraceResult();
            }

            cancellationToken.ThrowIfCancellationRequested();
            ReportProgress(progress, "瓶設備の候補を取得しています...", 25);

            var bottleNodes = _repo.FindBottleNodeForward(p);

            cancellationToken.ThrowIfCancellationRequested();
            ReportProgress(progress, "グリッド用データを作成しています..", 62);

            var bottleTable = CreateBottleTable(bottleNodes, cancellationToken);

            return new BottleTraceResult
            {
                BottleTable = bottleTable,
                BottleNodes = bottleNodes
            };
        }


        #endregion

        #region バック

        public BottleTraceResult BottleTraceBackwardResult(
            TraceSearchParameters? p,
            IProgress<TraceProgressState>? progress = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {

            if (p == null) throw new ArgumentNullException("p");

            cancellationToken.ThrowIfCancellationRequested();
            ReportProgress(progress, "品目名条件を解決しています...", 12);

            if (!ResolveItemNameCondition(p))
            {
                return new BottleTraceResult();
            }

            cancellationToken.ThrowIfCancellationRequested();
            ReportProgress(progress, "瓶設備の候補を取得しています...", 25);

            var bottleNodes = _repo.FindBottleNodesBackward(p);

            cancellationToken.ThrowIfCancellationRequested();
            ReportProgress(progress, "グリッド用データを作成しています..", 62);

            var bottleTable = CreateBottleTable(bottleNodes, cancellationToken);

            return new BottleTraceResult
            {
                BottleTable = bottleTable,
                BottleNodes = bottleNodes
            };            
        }

        #endregion




        #region 汎用


        private DataTable CreateBottleTable(List<Bottle_ProductionResultNode> bottleNodes, CancellationToken cancellationToken)
        {
            var bottleTable = new DataTable();

            bottleTable.Columns.Add("OrderNumber", typeof(string));
            bottleTable.Columns.Add("Lot", typeof(string));
            bottleTable.Columns.Add("ItemCode", typeof(string));
            bottleTable.Columns.Add("Mid_Lot", typeof(string));
            bottleTable.Columns.Add("StartDate", typeof(DateTime));
            bottleTable.Columns.Add("OK_Num", typeof(int));
            bottleTable.Columns.Add("NG_Num", typeof(int));
            bottleTable.Columns.Add("Total_Num", typeof(int));
            bottleTable.Columns.Add("NodeKey", typeof(string));

            foreach (var node in bottleNodes)
            {
                cancellationToken.ThrowIfCancellationRequested();

                bottleTable.Rows.Add(node.OrderNumber,
                    node.ProductLotNumber,
                    node.ProductItemCode,
                    node.MiddleProductLotNumber,
                    node.StartDate,
                    node.FillingBottleNum_OK,
                    node.FillingBottleNum_NG,
                    node.FillingBottleNum_OK + node.FillingBottleNum_NG,
                    node.NodeIdentifyKey);
            }

            return bottleTable;
        }
            
        private static void ReportProgress(IProgress<TraceProgressState>? progress, string message, int? percent = null)
        {
            if (progress != null)
                progress.Report(new TraceProgressState(message, percent));
        }


        #endregion
    }
}
