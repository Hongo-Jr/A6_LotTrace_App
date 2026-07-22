using LotTraceApp.Models;
using LotTraceApp.Repositories;
using System;
using System.Data;


namespace LotTraceApp.Services
{
    public class BottleResultService
    {
        private readonly BottleResultRepositories _repo;

        public BottleResultService(BottleResultRepositories bottleResultRepositories)
        {
            _repo = bottleResultRepositories ?? throw new ArgumentNullException(nameof(bottleResultRepositories));
        }


        public DataTable GetBottleOrder(string order, string lot)
            => _repo.GetBottleOrder(order, lot);


        public DataTable GetBottleOrderVerticalAll(string order, string lot)
        {
            var wide = _repo.GetBottleOrder(order, lot);
            return ToVerticalAllColumns(wide);
        }

        private static DataTable ToVerticalAllColumns(DataTable wide)
        {
            var vertical = new DataTable();
            vertical.Columns.Add("Item", typeof(string));
            vertical.Columns["Item"].Caption = "項目";

            vertical.Columns.Add("order", typeof(string));
            vertical.Columns["order"].Caption = "指図";


            if (wide == null || wide.Columns.Count == 0)
                return vertical;

            for (int r = 0; r < wide.Rows.Count; r++)
            {
                var row = wide.Rows[r];

                foreach (DataColumn col in wide.Columns)
                {

                    string value = row.IsNull(col) ? null : Convert.ToString(row[col]);
                    vertical.Rows.Add(col.Caption, value);

                }
            }

            return vertical;
        }


        public FillingResultTable GetFillingResultPage(string order, string lot, int pageNo, int pageSize)
        {
            var table = new FillingResultTable();
            table.BottleTable = SetBottleResultTable();
            table.DrumTable = SetDrumResultTable();

            var page = _repo.GetFillingResultPages(order, lot, pageNo, pageSize);
            table.TotalCount = page?.TotalCount ?? 0;

            float fMaxPage = (float)table.TotalCount / (float)pageSize;
            int iMaxPage = (int)Math.Ceiling(fMaxPage);

            table.MaxPageNo = iMaxPage;

            foreach (var row in page.BottleRows)
            {
                table.BottleTable.Rows.Add(row.OrderNumber, row.ProductLotNumber, row.ProductItemCode, row.MiddleProductLotNumber, row.MiddleProductItemCode, row.BottleID, row.SamplingGroup, row.BottleINumber, row.FillingNozzleNumber, row.CapTighteningTorqueValue, row.CapTighteningTorqueJudgment, row.CapTiltDetectionJudgment, row.FillingMachineNumber, row.TotalCahckJudgment, row.BottleLocation, row.FillingWeight, row.FillingTime, row.FillingStartDate, row.FillingEndDate, row.ProcessType);

            }

            foreach (var row in page.DrumRows)
            {
                table.DrumTable.Rows.Add(row.OrderNumber, row.ProcessType, row.ProductLotNumber, row.ProductItemCode, row.MiddleProductLotNumber, row.MiddleProductItemCode, row.DrumcanNumber, row.FillingNozzleNumber, row.CapTighteningTorqueValue_Big, row.CapTighteningTorqueJudgment, row.CapTiltDetectionJudgment, row.TotalCahckJudgment, row.CapTighteningTorqueValue_Small, row.FillingWeightJudgment, row.BottleLocation, row.FillingWeight, row.FillingTime, row.FillingStartDate, row.FillingEndDate);

            }

            return table;
        }



        private DataTable SetBottleResultTable()
        {
            var result = new DataTable();

            result.Columns.Add("OrderNumber");
            result.Columns.Add("ProductLotNumber");
            result.Columns.Add("ProductItemCode");
            result.Columns.Add("MiddleProductLotNumber");
            result.Columns.Add("MiddleProductItemCode");
            result.Columns.Add("BottleID");
            result.Columns.Add("SamplingGroup");
            result.Columns.Add("BottleINumber");
            result.Columns.Add("FillingNozzleNumber");
            result.Columns.Add("CapTighteningTorqueValue");
            result.Columns.Add("CapTighteningTorqueJudgment");
            result.Columns.Add("CapTiltDetectionJudgment");
            result.Columns.Add("FillingMachineNumber");
            result.Columns.Add("TotalCahckJudgment");
            result.Columns.Add("BottleLocation");
            result.Columns.Add("FillingWeight");
            result.Columns.Add("FillingTime");
            result.Columns.Add("FillingStartDate");
            result.Columns.Add("FillingEndDate");
            result.Columns.Add("ProcessType");

            result.Columns["OrderNumber"].Caption = "指図番号";
            result.Columns["ProductLotNumber"].Caption = "製品ロットNo";
            result.Columns["ProductItemCode"].Caption = "製品品目コード";
            result.Columns["MiddleProductLotNumber"].Caption = "中間品ロットNo";
            result.Columns["MiddleProductItemCode"].Caption = "中間品品目コード";
            result.Columns["BottleID"].Caption = "瓶ID";
            result.Columns["SamplingGroup"].Caption = "サンプリンググループ";
            result.Columns["BottleINumber"].Caption = "瓶番号";
            result.Columns["FillingNozzleNumber"].Caption = "充填ノズルNo";
            result.Columns["CapTighteningTorqueValue"].Caption = "キャップ締付トルク値";
            result.Columns["CapTighteningTorqueJudgment"].Caption = "キャップ締付トルク判定";
            result.Columns["CapTiltDetectionJudgment"].Caption = "キャップ締付方向判定";
            result.Columns["FillingMachineNumber"].Caption = "充填機番号";
            result.Columns["TotalCahckJudgment"].Caption = "総合判定";
            result.Columns["BottleLocation"].Caption = "瓶ロケーション";
            result.Columns["FillingWeight"].Caption = "充填重量";
            result.Columns["FillingTime"].Caption = "充填時間";
            result.Columns["FillingStartDate"].Caption = "充填開始日時";
            result.Columns["FillingEndDate"].Caption = "充填終了日時";
            result.Columns["ProcessType"].Caption = "工程種別";




            return result;

        }
        private DataTable SetDrumResultTable()
        {
            var result = new DataTable();

            result.Columns.Add("OrderNumber");
            result.Columns.Add("ProcessType");
            result.Columns.Add("ProductLotNumber");
            result.Columns.Add("ProductItemCode");
            result.Columns.Add("MiddleProductLotNumber");
            result.Columns.Add("MiddleProductItemCode");
            result.Columns.Add("DrumcanNumber");
            result.Columns.Add("FillingNozzleNumber");
            result.Columns.Add("CapTighteningTorqueValue_Big");
            result.Columns.Add("CapTighteningTorqueJudgment");
            result.Columns.Add("CapTiltDetectionJudgment");
            result.Columns.Add("TotalCahckJudgment");
            result.Columns.Add("CapTighteningTorqueValue_Small");
            result.Columns.Add("FillingWeightJudgment");
            result.Columns.Add("BottleLocation");
            result.Columns.Add("FillingWeight");
            result.Columns.Add("FillingTime");
            result.Columns.Add("FillingStartDate");
            result.Columns.Add("FillingEndDate");

            result.Columns["OrderNumber"].Caption = "指図番号";
            result.Columns["ProcessType"].Caption = "工程種別";
            result.Columns["ProductLotNumber"].Caption = "製品ロットNo";
            result.Columns["ProductItemCode"].Caption = "製品品目コード";
            result.Columns["MiddleProductLotNumber"].Caption = "中間品ロットNo";
            result.Columns["MiddleProductItemCode"].Caption = "中間品品目コード";
            result.Columns["DrumcanNumber"].Caption = "ドラム缶番号";
            result.Columns["FillingNozzleNumber"].Caption = "充填ノズルNo";
            result.Columns["CapTighteningTorqueValue_Big"].Caption = "キャップ締付トルク値(大)";
            result.Columns["CapTighteningTorqueJudgment"].Caption = "キャップ締付トルク判定";
            result.Columns["CapTiltDetectionJudgment"].Caption = "キャップ締付方向判定";
            result.Columns["TotalCahckJudgment"].Caption = "総合判定";
            result.Columns["CapTighteningTorqueValue_Small"].Caption = "キャップ締付トルク値(小)";
            result.Columns["FillingWeightJudgment"].Caption = "充填重量判定";
            result.Columns["BottleLocation"].Caption = "瓶ロケーション";
            result.Columns["FillingWeight"].Caption = "充填重量";
            result.Columns["FillingTime"].Caption = "充填時間";
            result.Columns["FillingStartDate"].Caption = "充填開始日時";
            result.Columns["FillingEndDate"].Caption = "充填終了日時";

            return result;
        }
        
    }
}