using System;
using LotTraceApp.Models;
using LotTraceApp.Repositories;

namespace LotTraceApp.Services
{
    /// <summary>
    /// 瓶設備ロットトレースの実装
    /// 図 7.2 / 7.4 のフローをそのままコード化（単段検索）
    /// </summary>
    public class BottleTraceService
    {
        private readonly BottleTraceRepository _repo;

        public BottleTraceService(BottleTraceRepository repo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        /// <summary>
        /// 検索条件に基づき、オーダ（始点）とボトル／ドラムの充填結果（終点）を取得
        /// </summary>
        public BottleTraceResult ExecuteTrace(TraceSearchParameters p)
        {
            var result = new BottleTraceResult();

            // 1. 検索始点（オーダ）取得
            var orders = _repo.FindStartOrders(p);
            result.StartOrders.AddRange(orders);

            if (orders.Count == 0)
                return result;

            // 2. オーダ単位で充填結果抽出
            var fillings = _repo.FindFillingsByOrders(orders, p.Direction, p.From, p.To);
            result.Fillings.AddRange(fillings);

            return result;
        }
    }
}