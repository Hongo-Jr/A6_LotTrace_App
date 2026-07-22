using LotTraceApp.Models;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LotTraceApp.Models.SingleControlHistoryModel;

namespace LotTraceApp.Repositories
{
    public class ResultRepositories
    {
        private readonly string _connectionString;
        public ResultRepositories(string connectionString)
        {
            if (connectionString == null)
            {
                throw new ArgumentNullException("connectionString");
            }
            _connectionString = connectionString;
        }
        private SqlConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
        /// <summary>
        /// 詳細履歴を呼び出してクラスをServicesに返す
        /// </summary>
        public List<SingleControlHistoryModel> FindSingleControlHistory(
     string productionOrderNumber,
     string itemCode,
     string lotNumber,
     string processId)
        {
            var Current = new List<SingleControlHistoryModel>();
            DataTable i;

            i = new DataTable();

            i = GetSingleControlHistory(productionOrderNumber, itemCode, lotNumber, processId);

            Current = FromDataTableSingle(i);

            return Current;
        }
        public DataTable FindSingleControlHistory2(
    string productionOrderNumber,
    string itemCode,
    string lotNumber,
    string processId)
        {
            var Current = new List<SingleControlHistoryModel>();
            DataTable i;

            i = new DataTable();

            i = GetSingleControlHistory(productionOrderNumber, itemCode, lotNumber, processId);

            Current = FromDataTableSingle(i);

            return i;
        }
        // <summary>
        /// 制御工程テーブルから詳細履歴を取得
        /// </summary>
        public DataTable GetSingleControlHistory(
     string productionOrderNumber,
     string itemCode,
     string lotNumber,
     string processId)
        {
            var dt = new DataTable();

            var selectColumns = BuildSingleControlSelectColumns(processId);

            using (var conn = CreateConnection())
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();

                cmd.CommandText = @"
SELECT
scp.MasterKey,
scp.ForeignKey,
CASE
    WHEN SUBSTRING(PARSENAME(REPLACE(scp.MasterKey, '_', '.'), 2), 2, 1) = '1'
        THEN N'指図'
    WHEN SUBSTRING(PARSENAME(REPLACE(scp.MasterKey, '_', '.'), 2), 2, 1) IN ('2', '3')
        THEN N'実績'
    ELSE N''
END AS DataCategory,
" + selectColumns + @"
FROM [MES31].[dbo].[SingleControlProcessTable] scp
WHERE scp.ForeignKey = @productionOrderNumber
AND scp.ItemCode = @ItemCode
AND scp.LotNumber = @LotNumber
AND SUBSTRING(
        scp.MasterKey,
        LEN(scp.MasterKey) - CHARINDEX('_', REVERSE(scp.MasterKey)),
        1
    ) = @processId
ORDER BY
CASE
    WHEN SUBSTRING(PARSENAME(REPLACE(scp.MasterKey, '_', '.'), 2), 2, 1) = '1'
        THEN 0
    WHEN SUBSTRING(PARSENAME(REPLACE(scp.MasterKey, '_', '.'), 2), 2, 1) IN ('2', '3')
        THEN 1
    ELSE 9
END,
scp.StartDate DESC,
scp.ManufacturingProcessName;";

                cmd.Parameters.Add("@productionOrderNumber", SqlDbType.NVarChar).Value = productionOrderNumber;
                cmd.Parameters.Add("@ItemCode", SqlDbType.NVarChar).Value = itemCode;
                cmd.Parameters.Add("@LotNumber", SqlDbType.NVarChar).Value = lotNumber;
                cmd.Parameters.Add("@processId", SqlDbType.NVarChar).Value = processId;

                using (var adp = new SqlDataAdapter((SqlCommand)cmd))
                {
                    adp.Fill(dt);
                }
            }

            return dt;
        }



        private string BuildSingleControlSelectColumns(string processId)
        {
            var sql = new StringBuilder();
            bool first = true;

            void AppendColumn(string column)
            {
                if (first)
                {
                    sql.Append(column);
                    first = false;
                }
                else
                {
                    sql.Append(",\r\n    ");
                    sql.Append(column);
                }
            }
            AppendColumn("scp.StartDate");
            AppendColumn("scp.EndDate");
            AppendColumn("scp.ManufacturingProcessName");


            switch ((processId ?? string.Empty).ToUpperInvariant())
            {
                // 1: CIP制御工程
                case "1":
                    AppendColumn("scp.PumpDelayTime");
                    AppendColumn("scp.ValveDelayTime");
                    AppendColumn("scp.CirculationPumpStartupSpeed");
                    AppendColumn("scp.CirculationPumpStopWeight");
                    AppendColumn("scp.NumberOfWashes");
                    AppendColumn("scp.CirculationLineCleaningTime");
                    AppendColumn("scp.BypassLineCirculationTime");
                    AppendColumn("scp.ShowerBallCleaningTime");
                    AppendColumn("scp.CirculatingLineFrequencySettingValue");
                    AppendColumn("scp.BypassLineFrequencySettingValue");
                    AppendColumn("scp.ShowerBallFrequencySettingValue");
                    AppendColumn("scp.SamplingFlag");
                    AppendColumn("scp.ShowerBallLineFlag");
                    AppendColumn("scp.FillingMachineValveOpeningAndClosingPattern");
                    break;

                // 2: N2押し制御工程
                case "2":
                    AppendColumn("scp.N2PressTimer1");
                    AppendColumn("scp.N2PressTimer2");
                    AppendColumn("scp.N2PressTimer3");
                    AppendColumn("scp.ShowerBallLineFlag");
                    break;

                // 3: エア抜き制御工程
                case "3":
                    AppendColumn("scp.PumpDelayTime");
                    AppendColumn("scp.ValveDelayTime");
                    AppendColumn("scp.CirculationPumpStartupSpeed");
                    AppendColumn("scp.CirculationPumpFrequency");
                    AppendColumn("scp.AirRemovalTime1");
                    AppendColumn("scp.AirRemovalTime2");
                    break;

                // 4: 液上制御工程
                case "4":
                    AppendColumn("scp.Weight");
                    AppendColumn("scp.LiquidLevel");
                    AppendColumn("scp.PumpDelayTime");
                    AppendColumn("scp.ValveDelayTime");
                    break;

                // 6: 撹拌制御工程
                case "6":
                    AppendColumn("scp.AgitatorFrequency");
                    AppendColumn("scp.MixerOperationTime");
                    AppendColumn("scp.MixerStopWeight");
                    break;

                // 7: 仕込制御工程
                case "7":
                    AppendColumn("scp.Weight");
                    AppendColumn("scp.SourceTankName");
                    AppendColumn("scp.PumpDelayTime");
                    AppendColumn("scp.ValveDelayTime");
                    AppendColumn("scp.SourceTankPumpStartupSpeed");
                    AppendColumn("scp.SourceTankPump1tageFrequency");
                    AppendColumn("scp.SourceTankPump2stageFrequency");
                    AppendColumn("scp.SourceTankPump2stageFrequencySwitchingWeight");
                    AppendColumn("scp.AmountOfStock");
                    AppendColumn("scp.ValveHalfopenWeight");
                    AppendColumn("scp.DroopAmountSettingValue");
                    AppendColumn("scp.ControlCompletionDelayTime");
                    break;

                // C: 循環制御工程(圧力制御有)
                case "C":
                    AppendColumn("scp.PumpDelayTime");
                    AppendColumn("scp.ValveDelayTime");
                    AppendColumn("scp.CirculationPumpOperationTime");
                    AppendColumn("scp.CirculationPumpStopWeight");
                    AppendColumn("scp.PressureSelection");
                    AppendColumn("scp.CirculationPumpTargetFrequency");
                    AppendColumn("scp.CirculationPumpStartupTime");
                    AppendColumn("scp.PressureTargetValue");
                    AppendColumn("scp.CirculationPumpStartupSpeed1");
                    AppendColumn("scp.TargetUpperLimitPressure");
                    AppendColumn("scp.TargetLowerLimitPressure");
                    AppendColumn("scp.CirculationPumpStartupSpeed2");
                    AppendColumn("scp.CorrectionUpperLimitPressure");
                    AppendColumn("scp.CorrectionLowerLimitPressure");
                    AppendColumn("scp.CorrectionSpeed");
                    AppendColumn("scp.Timer");
                    AppendColumn("scp.UpperLimitAlarmPressure_H");
                    AppendColumn("scp.LowerLimitAlarmPressure_L");
                    AppendColumn("scp.UpperLimitAlarmPressure_HH");
                    AppendColumn("scp.LowerLimitAlarmPressure_LL");
                    AppendColumn("scp.WeightMonitoring");
                    break;

                // D: 循環制御工程(圧力制御無)
                case "D":
                    AppendColumn("scp.PumpDelayTime");
                    AppendColumn("scp.ValveDelayTime");
                    AppendColumn("scp.CirculationPumpStartupSpeed");
                    AppendColumn("scp.CirculationPumpFrequency");
                    AppendColumn("scp.CirculationPumpOperationTime");
                    AppendColumn("scp.CirculationPumpStopWeight");
                    AppendColumn("scp.WashingPumpStartupSpeed");
                    AppendColumn("scp.WashingPumpFrequency");
                    break;

                // E: 循環制御工程(流量制御有)
                case "E":
                    AppendColumn("scp.PumpDelayTime");
                    AppendColumn("scp.ValveDelayTime");
                    AppendColumn("scp.PressureSelection");
                    AppendColumn("scp.CirculationPumpOperationTime");
                    AppendColumn("scp.CirculationPumpStopWeight");
                    AppendColumn("scp.CirculationPumpTargetFrequency");
                    AppendColumn("scp.CirculationPumpStartupTime");
                    AppendColumn("scp.CirculationPumpStartupSpeed1");
                    AppendColumn("scp.CirculationPumpStartupSpeed2");
                    AppendColumn("scp.CorrectionSpeed");
                    AppendColumn("scp.Timer");
                    AppendColumn("scp.UpperLimitAlarmPressure_H");
                    AppendColumn("scp.LowerLimitAlarmPressure_L");
                    AppendColumn("scp.UpperLimitAlarmPressure_HH");
                    AppendColumn("scp.LowerLimitAlarmPressure_LL");
                    AppendColumn("scp.WeightMonitoring");
                    AppendColumn("scp.FlowTargetValue");
                    AppendColumn("scp.TargetUpperLimitFlow");
                    AppendColumn("scp.TargetLowerLimitFlow");
                    AppendColumn("scp.CorrectionUpperLimitFlow");
                    AppendColumn("scp.CorrectionLowerLimitFlow");
                    AppendColumn("scp.UpperLimitAlarmFlow_HH");
                    AppendColumn("scp.UpperLimitAlarmFlow_H");
                    AppendColumn("scp.LowerLimitAlarmFlow_L");
                    break;

                // F: 初液抜き制御工程
                case "F":
                    AppendColumn("scp.Weight");
                    AppendColumn("scp.LiquidLevel");
                    AppendColumn("scp.PumpDelayTime");
                    AppendColumn("scp.ValveDelayTime");
                    AppendColumn("scp.CirculationPumpStartupSpeed");
                    AppendColumn("scp.CirculationPumpFrequency");
                    AppendColumn("scp.CirculationPumpOperationTime");
                    AppendColumn("scp.CirculationPumpFrequencySetting_L");
                    AppendColumn("scp.WasteVolume");
                    break;

                // G: 抜出制御工程
                case "G":
                    AppendColumn("scp.Weight");
                    AppendColumn("scp.LiquidLevel");
                    AppendColumn("scp.PumpDelayTime");
                    AppendColumn("scp.ValveDelayTime");
                    AppendColumn("scp.CirculationPumpStartupSpeed");
                    AppendColumn("scp.CirculationPumpFrequency");
                    AppendColumn("scp.CirculationPumpStopWeight");
                    AppendColumn("scp.AmountOfExtrction");
                    AppendColumn("scp.DroopAmountSettingValue");
                    AppendColumn("scp.TotalDischargePumpStopDelayTime");
                    break;

                // H: 廃液制御工程
                case "H":
                    AppendColumn("scp.PumpDelayTime");
                    AppendColumn("scp.ValveDelayTime");
                    AppendColumn("scp.CirculationPumpStartupSpeed");
                    AppendColumn("scp.CirculationPumpFrequency");
                    AppendColumn("scp.CirculationPumpOperationTime");
                    AppendColumn("scp.CirculationPumpStopWeight");
                    AppendColumn("scp.WasteLiquidTypeFlag");
                    break;

                // I: 蓋ロック制御工程
                case "I":
                    AppendColumn("scp.ManholeLockTimer");
                    AppendColumn("scp.ManholeAlarmTimer");
                    AppendColumn("scp.ManholeOpeningTime");
                    AppendColumn("scp.ManholeClosingTime");
                    AppendColumn("scp.ManholeOpenedHours");
                    AppendColumn("scp.NumberOfManholesOpenedAndClosed");
                    break;
            }

            return sql.ToString();
        }
        /// <summary>
        /// フィルタテーブルから履歴を取得
        /// </summary>
        public DataTable GetFilterHistory(
            string productionOrderNumber,
            string itemCode,
            string lotNumber)
        {
            var dt = new DataTable();

            using (var conn = CreateConnection())
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();

                cmd.CommandText = @"
SELECT
    ft.MasterKey,
    ft.ForeignKey,
    CASE
        WHEN ft.MasterKey LIKE '%[_]%'
         AND SUBSTRING(
                ft.MasterKey,
                LEN(ft.MasterKey) - CHARINDEX('_', REVERSE(ft.MasterKey)) - 2,
                1
             ) = '1'
            THEN N'指図'
        WHEN ft.MasterKey LIKE '%[_]%'
         AND SUBSTRING(
                ft.MasterKey,
                LEN(ft.MasterKey) - CHARINDEX('_', REVERSE(ft.MasterKey)) - 2,
                1
             ) IN ('2', '3')
            THEN N'実績'
        ELSE N''
    END AS DataCategory,
    ft.FilterItemCode1,
    ft.FilterSetNumber1,
    ft.FilterItemCode2,
    ft.FilterSetNumber2,
    ft.FilterLotNumber01,
    ft.FilterLotNumber02,
    ft.FilterLotNumber03,
    ft.FilterLotNumber04,
    ft.FilterLotNumber05,
    ft.FilterLotNumber06,
    ft.FilterLotNumber07,
    ft.FilterLotNumber08,
    ft.FilterLotNumber09,
    ft.FilterLotNumber10,
    ft.FilterLotNumber11,
    ft.FilterLotNumber12,
    ft.FilterLotNumber13,
    ft.FilterLotNumber14,
    ft.FilterLotNumber15,
    ft.FilterLotNumber16,
    ft.FilterLotNumber17,
    ft.FilterLotNumber18,
    ft.FilterLotNumber19,
    ft.FilterLotNumber20,
    ft.FilterLotNumber21,
    ft.FilterLotNumber22,
    ft.FilterLotNumber23,
    ft.FilterLotNumber24,
    ft.FilterLotNumber25,
    ft.FilterLotNumber26,
    ft.FilterLotNumber27,
    ft.FilterLotNumber28,
    ft.FilterLotNumber29,
    ft.FilterLotNumber30,
    ft.FilterLotNumber31,
    ft.FilterLotNumber32,
    ft.FilterLotNumber33,
    ft.FilterLotNumber34,
    ft.FilterLotNumber35,
    ft.FilterLotNumber36,
    ft.FilterLotNumber37,
    ft.FilterLotNumber38,
    ft.FilterLotNumber39,
    ft.FilterLotNumber40
FROM [MES31].[dbo].[FilterTable] ft
WHERE ft.ItemCode = @ItemCode
  AND ft.LotNumber = @LotNumber
ORDER BY
    CASE
        WHEN ft.MasterKey LIKE '%[_]%'
         AND SUBSTRING(
                ft.MasterKey,
                LEN(ft.MasterKey) - CHARINDEX('_', REVERSE(ft.MasterKey)) - 2,
                1
             ) = '1'
            THEN 0
        WHEN ft.MasterKey LIKE '%[_]%'
         AND SUBSTRING(
                ft.MasterKey,
                LEN(ft.MasterKey) - CHARINDEX('_', REVERSE(ft.MasterKey)) - 2,
                1
             ) IN ('2', '3')
            THEN 1
        ELSE 9
    END,
    ft.MasterKey;";

                cmd.Parameters.Add("@ItemCode", SqlDbType.NVarChar).Value = itemCode;
                cmd.Parameters.Add("@LotNumber", SqlDbType.NVarChar).Value = lotNumber;

                using (var adp = new SqlDataAdapter((SqlCommand)cmd))
                {
                    adp.Fill(dt);
                }
            }

            return dt;
        }
        public static List<SingleControlHistoryModel> FromDataTableSingle(DataTable dt)
        {
            var list = new List<SingleControlHistoryModel>();

            if (dt == null || dt.Rows.Count == 0)
            {
                return list;
            }

            foreach (DataRow row in dt.Rows)
            {
                var model = new SingleControlHistoryModel
                {
                    MasterKey = ToNullableString(row, "MasterKey"),
                    ForeignKey = ToNullableString(row, "ForeignKey"),
                    DataCategory = ToNullableString(row, "DataCategory"),

                    StartDate = ToNullableDateTime(row, "StartDate"),
                    EndDate = ToNullableDateTime(row, "EndDate"),
                    ManufacturingProcessName = ToNullableString(row, "ManufacturingProcessName"),

                    PumpDelayTime = ToNullableDecimal(row, "PumpDelayTime"),
                    ValveDelayTime = ToNullableDecimal(row, "ValveDelayTime"),
                    CirculationPumpStartupSpeed = ToNullableDecimal(row, "CirculationPumpStartupSpeed"),
                    CirculationPumpStopWeight = ToNullableDecimal(row, "CirculationPumpStopWeight"),
                    NumberOfWashes = ToNullableInt(row, "NumberOfWashes"),
                    CirculationLineCleaningTime = ToNullableDecimal(row, "CirculationLineCleaningTime"),
                    BypassLineCirculationTime = ToNullableDecimal(row, "BypassLineCirculationTime"),
                    ShowerBallCleaningTime = ToNullableDecimal(row, "ShowerBallCleaningTime"),
                    CirculatingLineFrequencySettingValue = ToNullableDecimal(row, "CirculatingLineFrequencySettingValue"),
                    BypassLineFrequencySettingValue = ToNullableDecimal(row, "BypassLineFrequencySettingValue"),
                    ShowerBallFrequencySettingValue = ToNullableDecimal(row, "ShowerBallFrequencySettingValue"),
                    SamplingFlag = ToNullableBool(row, "SamplingFlag"),
                    ShowerBallLineFlag = ToNullableBool(row, "ShowerBallLineFlag"),
                    FillingMachineValveOpeningAndClosingPattern = ToNullableString(row, "FillingMachineValveOpeningAndClosingPattern"),
                    N2PressTimer1 = ToNullableDecimal(row, "N2PressTimer1"),
                    N2PressTimer2 = ToNullableDecimal(row, "N2PressTimer2"),
                    N2PressTimer3 = ToNullableDecimal(row, "N2PressTimer3"),

                    CirculationPumpFrequency = ToNullableDecimal(row, "CirculationPumpFrequency"),
                    AirRemovalTime1 = ToNullableDecimal(row, "AirRemovalTime1"),
                    AirRemovalTime2 = ToNullableDecimal(row, "AirRemovalTime2"),

                    Weight = ToNullableDecimal(row, "Weight"),
                    LiquidLevel = ToNullableDecimal(row, "LiquidLevel"),

                    AgitatorFrequency = ToNullableDecimal(row, "AgitatorFrequency"),
                    MixerOperationTime = ToNullableDecimal(row, "MixerOperationTime"),
                    MixerStopWeight = ToNullableDecimal(row, "MixerStopWeight"),

                    SourceTankName = ToNullableString(row, "SourceTankName"),
                    SourceTankPumpStartupSpeed = ToNullableDecimal(row, "SourceTankPumpStartupSpeed"),
                    SourceTankPump1tageFrequency = ToNullableDecimal(row, "SourceTankPump1tageFrequency"),
                    SourceTankPump2stageFrequency = ToNullableDecimal(row, "SourceTankPump2stageFrequency"),
                    SourceTankPump2stageFrequencySwitchingWeight = ToNullableDecimal(row, "SourceTankPump2stageFrequencySwitchingWeight"),
                    AmountOfStock = ToNullableDecimal(row, "AmountOfStock"),
                    ValveHalfopenWeight = ToNullableDecimal(row, "ValveHalfopenWeight"),
                    DroopAmountSettingValue = ToNullableDecimal(row, "DroopAmountSettingValue"),
                    ControlCompletionDelayTime = ToNullableDecimal(row, "ControlCompletionDelayTime"),

                    CirculationPumpOperationTime = ToNullableDecimal(row, "CirculationPumpOperationTime"),
                    PressureSelection = ToNullableString(row, "PressureSelection"),
                    CirculationPumpTargetFrequency = ToNullableDecimal(row, "CirculationPumpTargetFrequency"),
                    CirculationPumpStartupTime = ToNullableDecimal(row, "CirculationPumpStartupTime"),
                    PressureTargetValue = ToNullableDecimal(row, "PressureTargetValue"),
                    CirculationPumpStartupSpeed1 = ToNullableDecimal(row, "CirculationPumpStartupSpeed1"),
                    TargetUpperLimitPressure = ToNullableDecimal(row, "TargetUpperLimitPressure"),
                    TargetLowerLimitPressure = ToNullableDecimal(row, "TargetLowerLimitPressure"),
                    CirculationPumpStartupSpeed2 = ToNullableDecimal(row, "CirculationPumpStartupSpeed2"),
                    CorrectionUpperLimitPressure = ToNullableDecimal(row, "CorrectionUpperLimitPressure"),
                    CorrectionLowerLimitPressure = ToNullableDecimal(row, "CorrectionLowerLimitPressure"),
                    CorrectionSpeed = ToNullableDecimal(row, "CorrectionSpeed"),
                    Timer = ToNullableDecimal(row, "Timer"),
                    UpperLimitAlarmPressure_H = ToNullableDecimal(row, "UpperLimitAlarmPressure_H"),
                    LowerLimitAlarmPressure_L = ToNullableDecimal(row, "LowerLimitAlarmPressure_L"),
                    UpperLimitAlarmPressure_HH = ToNullableDecimal(row, "UpperLimitAlarmPressure_HH"),
                    LowerLimitAlarmPressure_LL = ToNullableDecimal(row, "LowerLimitAlarmPressure_LL"),
                    WeightMonitoring = ToNullableBool(row, "WeightMonitoring"),

                    WashingPumpStartupSpeed = ToNullableDecimal(row, "WashingPumpStartupSpeed"),
                    WashingPumpFrequency = ToNullableDecimal(row, "WashingPumpFrequency"),

                    FlowTargetValue = ToNullableDecimal(row, "FlowTargetValue"),
                    TargetUpperLimitFlow = ToNullableDecimal(row, "TargetUpperLimitFlow"),
                    TargetLowerLimitFlow = ToNullableDecimal(row, "TargetLowerLimitFlow"),
                    CorrectionUpperLimitFlow = ToNullableDecimal(row, "CorrectionUpperLimitFlow"),
                    CorrectionLowerLimitFlow = ToNullableDecimal(row, "CorrectionLowerLimitFlow"),
                    UpperLimitAlarmFlow_HH = ToNullableDecimal(row, "UpperLimitAlarmFlow_HH"),
                    UpperLimitAlarmFlow_H = ToNullableDecimal(row, "UpperLimitAlarmFlow_H"),
                    LowerLimitAlarmFlow_L = ToNullableDecimal(row, "LowerLimitAlarmFlow_L"),

                    CirculationPumpFrequencySetting_L = ToNullableDecimal(row, "CirculationPumpFrequencySetting_L"),
                    WasteVolume = ToNullableDecimal(row, "WasteVolume"),

                    AmountOfExtrction = ToNullableDecimal(row, "AmountOfExtrction"),
                    TotalDischargePumpStopDelayTime = ToNullableDecimal(row, "TotalDischargePumpStopDelayTime"),

                    WasteLiquidTypeFlag = ToNullableString(row, "WasteLiquidTypeFlag"),

                    ManholeLockTimer = ToNullableDecimal(row, "ManholeLockTimer"),
                    ManholeAlarmTimer = ToNullableDecimal(row, "ManholeAlarmTimer"),
                    ManholeOpeningTime = ToNullableDecimal(row, "ManholeOpeningTime"),
                    ManholeClosingTime = ToNullableDecimal(row, "ManholeClosingTime"),
                    ManholeOpenedHours = ToNullableDecimal(row, "ManholeOpenedHours"),
                    NumberOfManholesOpenedAndClosed = ToNullableInt(row, "NumberOfManholesOpenedAndClosed")
                };

                list.Add(model);
            }

            return list;
        }

        private static object? GetValue(DataRow row, string columnName)
        {
            if (row == null || row.Table == null || !row.Table.Columns.Contains(columnName))
            {
                return null;
            }

            var value = row[columnName];
            return value == DBNull.Value ? null : value;
        }

        private static string? ToNullableString(DataRow row, string columnName)
        {
            var value = GetValue(row, columnName);
            return value == null ? null : value.ToString();
        }

        private static DateTime? ToNullableDateTime(DataRow row, string columnName)
        {
            var value = GetValue(row, columnName);
            return value == null ? (DateTime?)null : Convert.ToDateTime(value);
        }

        private static decimal? ToNullableDecimal(DataRow row, string columnName)
        {
            var value = GetValue(row, columnName);
            return value == null ? (decimal?)null : Convert.ToDecimal(value);
        }

        private static int? ToNullableInt(DataRow row, string columnName)
        {
            var value = GetValue(row, columnName);
            return value == null ? (int?)null : Convert.ToInt32(value);
        }

        private static bool? ToNullableBool(DataRow row, string columnName)
        {
            var value = GetValue(row, columnName);
            return value == null ? (bool?)null : Convert.ToBoolean(value);
        }

        public static List<FilterHistoryModel> FromDataTableFill(DataTable dt)
        {
            var list = new List<FilterHistoryModel>();

            if (dt == null || dt.Rows.Count == 0)
            {
                return list;
            }

            foreach (DataRow row in dt.Rows)
            {
                var model = new FilterHistoryModel
                {
                    MasterKey = ToNullableString(row, "MasterKey"),
                    ForeignKey = ToNullableString(row, "ForeignKey"),
                    DataCategory = ToNullableString(row, "DataCategory"),

                    FilterItemCode1 = ToNullableString(row, "FilterItemCode1"),
                    FilterSetNumber1 = ToNullableInt(row, "FilterSetNumber1"),
                    FilterItemCode2 = ToNullableString(row, "FilterItemCode2"),
                    FilterSetNumber2 = ToNullableInt(row, "FilterSetNumber2"),

                    FilterLotNumber01 = ToNullableString(row, "FilterLotNumber01"),
                    FilterLotNumber02 = ToNullableString(row, "FilterLotNumber02"),
                    FilterLotNumber03 = ToNullableString(row, "FilterLotNumber03"),
                    FilterLotNumber04 = ToNullableString(row, "FilterLotNumber04"),
                    FilterLotNumber05 = ToNullableString(row, "FilterLotNumber05"),
                    FilterLotNumber06 = ToNullableString(row, "FilterLotNumber06"),
                    FilterLotNumber07 = ToNullableString(row, "FilterLotNumber07"),
                    FilterLotNumber08 = ToNullableString(row, "FilterLotNumber08"),
                    FilterLotNumber09 = ToNullableString(row, "FilterLotNumber09"),
                    FilterLotNumber10 = ToNullableString(row, "FilterLotNumber10"),
                    FilterLotNumber11 = ToNullableString(row, "FilterLotNumber11"),
                    FilterLotNumber12 = ToNullableString(row, "FilterLotNumber12"),
                    FilterLotNumber13 = ToNullableString(row, "FilterLotNumber13"),
                    FilterLotNumber14 = ToNullableString(row, "FilterLotNumber14"),
                    FilterLotNumber15 = ToNullableString(row, "FilterLotNumber15"),
                    FilterLotNumber16 = ToNullableString(row, "FilterLotNumber16"),
                    FilterLotNumber17 = ToNullableString(row, "FilterLotNumber17"),
                    FilterLotNumber18 = ToNullableString(row, "FilterLotNumber18"),
                    FilterLotNumber19 = ToNullableString(row, "FilterLotNumber19"),
                    FilterLotNumber20 = ToNullableString(row, "FilterLotNumber20"),
                    FilterLotNumber21 = ToNullableString(row, "FilterLotNumber21"),
                    FilterLotNumber22 = ToNullableString(row, "FilterLotNumber22"),
                    FilterLotNumber23 = ToNullableString(row, "FilterLotNumber23"),
                    FilterLotNumber24 = ToNullableString(row, "FilterLotNumber24"),
                    FilterLotNumber25 = ToNullableString(row, "FilterLotNumber25"),
                    FilterLotNumber26 = ToNullableString(row, "FilterLotNumber26"),
                    FilterLotNumber27 = ToNullableString(row, "FilterLotNumber27"),
                    FilterLotNumber28 = ToNullableString(row, "FilterLotNumber28"),
                    FilterLotNumber29 = ToNullableString(row, "FilterLotNumber29"),
                    FilterLotNumber30 = ToNullableString(row, "FilterLotNumber30"),
                    FilterLotNumber31 = ToNullableString(row, "FilterLotNumber31"),
                    FilterLotNumber32 = ToNullableString(row, "FilterLotNumber32"),
                    FilterLotNumber33 = ToNullableString(row, "FilterLotNumber33"),
                    FilterLotNumber34 = ToNullableString(row, "FilterLotNumber34"),
                    FilterLotNumber35 = ToNullableString(row, "FilterLotNumber35"),
                    FilterLotNumber36 = ToNullableString(row, "FilterLotNumber36"),
                    FilterLotNumber37 = ToNullableString(row, "FilterLotNumber37"),
                    FilterLotNumber38 = ToNullableString(row, "FilterLotNumber38"),
                    FilterLotNumber39 = ToNullableString(row, "FilterLotNumber39"),
                    FilterLotNumber40 = ToNullableString(row, "FilterLotNumber40")
                };

                list.Add(model);
            }

            return list;
        }
        public List<string> GetAvailableProcessIds(string productionOrderNumber, string itemCode, string lotNumber)
        {
            var list = new List<string>();

            using (var conn = CreateConnection())
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();

                cmd.CommandText = @"
SELECT DISTINCT
    SUBSTRING(
        scp.MasterKey,
        LEN(scp.MasterKey) - CHARINDEX('_', REVERSE(scp.MasterKey)),
        1
    ) AS ProcessId
FROM [MES31].[dbo].[SingleControlProcessTable] scp
WHERE scp.ForeignKey = @productionOrderNumber
  AND scp.ItemCode   = @ItemCode
  AND scp.LotNumber  = @LotNumber
  AND scp.MasterKey LIKE '%[_]%'  -- 念のため
ORDER BY ProcessId;";

                cmd.Parameters.Add("@productionOrderNumber", SqlDbType.NVarChar).Value = productionOrderNumber;
                cmd.Parameters.Add("@ItemCode", SqlDbType.NVarChar).Value = itemCode;
                cmd.Parameters.Add("@LotNumber", SqlDbType.NVarChar).Value = lotNumber;

                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        var pid = r["ProcessId"] == DBNull.Value ? null : r["ProcessId"].ToString();
                        if (!string.IsNullOrWhiteSpace(pid))
                            list.Add(pid);
                    }
                }
            }

            return list;
        }

        public bool HasFilterHistory(string itemCode, string lotNumber)
        {
            using (var conn = CreateConnection())
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();

                cmd.CommandText = @"
SELECT CASE WHEN EXISTS (
    SELECT 1
    FROM [MES31].[dbo].[FilterTable] ft
    WHERE ft.ItemCode = @ItemCode
      AND ft.LotNumber = @LotNumber
) THEN 1 ELSE 0 END;";

                cmd.Parameters.Add("@ItemCode", SqlDbType.NVarChar).Value = itemCode;
                cmd.Parameters.Add("@LotNumber", SqlDbType.NVarChar).Value = lotNumber;

                return Convert.ToInt32(cmd.ExecuteScalar()) == 1;
            }
        }

    }
}
