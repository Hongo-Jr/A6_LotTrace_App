using System;
using System.IO;
using System.Text;

public static class FileOutputHelper
{
    private static readonly string BaseDir = AppDomain.CurrentDomain.BaseDirectory;
    private static readonly string DebugDir = Path.Combine(BaseDir, "Debug");
    private static readonly string LogDir = Path.Combine(BaseDir, "Logs");

    static FileOutputHelper()
    {
        EnsureDirectory(DebugDir);
        EnsureDirectory(LogDir);
    }

    private static void EnsureDirectory(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }

    /// <summary>
    /// テーブルダンプ出力
    /// </summary>
    public static void WriteDump(string content, string prefix = "TableDump")
    {
        try
        {
            var fileName = $"{prefix}_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
            var filePath = Path.Combine(DebugDir, fileName);

            File.WriteAllText(filePath, content, Encoding.UTF8);
        }
        catch
        {
            // ダンプは失敗しても落とさない
        }
    }

    /// <summary>
    /// ログ出力
    /// </summary>
    public static void WriteLog(string message, Exception? ex = null)
    {
        try
        {
            var fileName = $"log_{DateTime.Now:yyyyMMdd}.txt";
            var filePath = Path.Combine(LogDir, fileName);

            var log = new StringBuilder();
            log.AppendLine("=================================");
            log.AppendLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            log.AppendLine(message);

            if (ex != null)
            {
                log.AppendLine("---- Exception ----");
                log.AppendLine(ex.ToString());
            }

            File.AppendAllText(filePath, log.ToString(), Encoding.UTF8);
        }
        catch
        {
            // ログ失敗でも落とさない
        }
    }
}