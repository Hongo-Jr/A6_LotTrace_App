using LotTraceApp.Models;
using LotTraceApp.Repositories;
using LotTraceApp.Services;
using LotTraceApp.Utils;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace LotTraceApp
{
    internal static class Program
    {
        private const string SingleInstanceMutexName = @"Local\LotTraceApp_8D80A746_BE3B_462A_A22B_A896F53AF1F8";
        private const uint AttachParentProcess = 0xFFFFFFFF;

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool AttachConsole(uint dwProcessId);

        [STAThread]
        private static void Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            bool createdNew;
            using (var singleInstanceMutex = new Mutex(true, SingleInstanceMutexName, out createdNew))
            {
                if (!createdNew)
                {
                    MessageBox.Show("すでにアプリが起動しています",
                                    "LotTraceApp",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                    return;
                }

                try
                {
                    Run(args);
                }
                finally
                {
                    singleInstanceMutex.ReleaseMutex();
                }
            }
        }

        private static void Run(string[] args)
        {
            AttachParentConsoleIfCommandLine(args);

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
            var resultService = new ResultService(resultRepo);
            
            var bottleRepo = new BottleTraceRepository(connBottle,liquidRepo);
            var bottleService = new BottleTraceService(bottleRepo, customerItemRepo);

            var bottleResultRepo = new BottleResultRepositories(connBottle);
            var bottleResultService = new BottleResultService(bottleResultRepo);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            CommandLineOptions options;
            string parseError;
            if (args != null && args.Length > 0)
            {
                if (!CommandLineOptions.TryParse(args, out options, out parseError))
                {
                    MessageBox.Show(parseError, "コマンドライン引数エラー",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var form = new MainForm(liquidService, bottleService, resultService,bottleResultService);

                if (options.Mode == 2)
                {
                    RunHeadlessCommandLine(form, options);
                    return;
                }

                form.SetCommandLineStartup(options);
                Application.Run(form);
                return;
            }

            Application.Run(new MainForm(liquidService, bottleService, resultService, bottleResultService));
        }

        private static void RunHeadlessCommandLine(MainForm form, CommandLineOptions options)
        {
            try
            {
                form.ExecuteCommandLineHeadless(options);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("コマンドライン処理に失敗しました: " + ex.Message);
            }
            finally
            {
                form.Dispose();
            }
        }

        private static void AttachParentConsoleIfCommandLine(string[] args)
        {
            if (args == null || args.Length == 0)
                return;

            AttachConsole(AttachParentProcess);
        }
    }

    public sealed class CommandLineOptions
    {
        public int Mode { get; private set; }
        public TraceSearchParameters SearchParameters { get; private set; }

        private CommandLineOptions()
        {
            Mode = 1;
            SearchParameters = new TraceSearchParameters();
        }

        public bool ExportsCsv
        {
            get { return Mode == 1 || Mode == 2; }
        }

        public static bool TryParse(string[] args, out CommandLineOptions options, out string errorMessage)
        {
            options = new CommandLineOptions();
            errorMessage = null;

            for (int i = 0; i < args.Length; i++)
            {
                string raw = args[i];
                if (string.IsNullOrWhiteSpace(raw))
                    continue;

                string flag;
                string value;
                SplitArgument(raw, out flag, out value);

                if (flag.Equals("-F", StringComparison.OrdinalIgnoreCase))
                {
                    options.SearchParameters.Direction = TraceDirection.Forward;
                }
                else if (flag.Equals("-B", StringComparison.OrdinalIgnoreCase))
                {
                    options.SearchParameters.Direction = TraceDirection.Backward;
                }
                else if (flag.Equals("-S", StringComparison.OrdinalIgnoreCase))
                {
                    string parsedValue;
                    if (!ReadValue(args, ref i, value, flag, out parsedValue, out errorMessage))
                        return false;

                    options.SearchParameters.ProductionOrderNumber = parsedValue;
                }
                else if (flag.Equals("-N", StringComparison.OrdinalIgnoreCase))
                {
                    string parsedValue;
                    if (!ReadValue(args, ref i, value, flag, out parsedValue, out errorMessage))
                        return false;

                    options.SearchParameters.ItemName = parsedValue;
                }
                else if (flag.Equals("-C", StringComparison.OrdinalIgnoreCase))
                {
                    string parsedValue;
                    if (!ReadValue(args, ref i, value, flag, out parsedValue, out errorMessage))
                        return false;

                    options.SearchParameters.ItemCode = parsedValue;
                }
                else if (flag.Equals("-L", StringComparison.OrdinalIgnoreCase))
                {
                    string parsedValue;
                    if (!ReadValue(args, ref i, value, flag, out parsedValue, out errorMessage))
                        return false;

                    options.SearchParameters.LotNumber = parsedValue;
                }
                else if (flag.Equals("-M", StringComparison.OrdinalIgnoreCase))
                {
                    string modeText;
                    if (!ReadValue(args, ref i, value, flag, out modeText, out errorMessage))
                        return false;

                    int mode;
                    if (!int.TryParse(modeText, out mode) || mode < 1 || mode > 3)
                    {
                        errorMessage = "-M には 1、2、3 のいずれかを指定してください。";
                        return false;
                    }

                    options.Mode = mode;
                }
                else
                {
                    errorMessage = "未対応のコマンドライン引数です: " + raw;
                    return false;
                }
            }

            if (!HasAnySearchCondition(options.SearchParameters))
            {
                errorMessage = "検索条件を1つ以上指定してください。";
                return false;
            }

            return true;
        }

        private static void SplitArgument(string raw, out string flag, out string value)
        {
            int separatorIndex = raw.IndexOf(':');
            if (separatorIndex < 0)
                separatorIndex = raw.IndexOf('=');

            if (separatorIndex < 0)
            {
                flag = raw;
                value = null;
                return;
            }

            flag = raw.Substring(0, separatorIndex);
            value = raw.Substring(separatorIndex + 1);
        }

        private static bool ReadValue(
            string[] args,
            ref int index,
            string inlineValue,
            string flag,
            out string value,
            out string errorMessage)
        {
            errorMessage = null;

            if (!string.IsNullOrWhiteSpace(inlineValue))
            {
                value = inlineValue.Trim();
                return true;
            }

            if (index + 1 >= args.Length || IsFlag(args[index + 1]))
            {
                value = null;
                errorMessage = flag + " の値を指定してください。";
                return false;
            }

            index++;
            value = args[index].Trim();
            return true;
        }

        private static bool IsFlag(string value)
        {
            return !string.IsNullOrWhiteSpace(value) && value.StartsWith("-", StringComparison.Ordinal);
        }

        private static bool HasAnySearchCondition(TraceSearchParameters p)
        {
            if (p == null)
                return false;

            return
                !string.IsNullOrWhiteSpace(p.ProductionOrderNumber) ||
                !string.IsNullOrWhiteSpace(p.ItemName) ||
                !string.IsNullOrWhiteSpace(p.ItemCode) ||
                !string.IsNullOrWhiteSpace(p.LotNumber);
        }
    }
}
