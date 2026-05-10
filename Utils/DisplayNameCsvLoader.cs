using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LotTraceApp.Utils
{
    public static class DisplayNameCsvLoader
    {
        /// <summary>
        /// Key,DisplayName 形式のCSVを読み込み、
        /// 内部名 -> 表示名 の辞書を返す。
        /// </summary>
        public static Dictionary<string, string> LoadDisplayNameMap(string csvPath)
        {
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (string.IsNullOrWhiteSpace(csvPath))
                throw new ArgumentException("csvPath is null or empty.", nameof(csvPath));

            if (!File.Exists(csvPath))
                throw new FileNotFoundException("CSV file not found.", csvPath);

            var lines = File.ReadAllLines(csvPath, Encoding.UTF8);
            if (lines == null || lines.Length == 0)
                return result;

            // 先頭行はヘッダ無し
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                string[] parts = line.Split(',');
                //if (parts.Length < 2)
                //    continue;

                string key = (parts[2] ?? string.Empty).Trim();
                string displayName = (parts[3] ?? string.Empty).Trim();

                if (string.IsNullOrWhiteSpace(key))
                    continue;

                result[key] = displayName;
            }

            return result;
        }

        /// <summary>
        /// 辞書にあれば表示名を返し、なければ元の名前を返す。
        /// </summary>
        public static string ResolveDisplayName(
            Dictionary<string, string> displayNameMap,
            string originalName)
        {
            if (string.IsNullOrWhiteSpace(originalName))
                return originalName;

            if (displayNameMap == null || displayNameMap.Count == 0)
                return originalName;

            string displayName;
            if (displayNameMap.TryGetValue(originalName, out displayName) &&
                !string.IsNullOrWhiteSpace(displayName))
            {
                return displayName;
            }

            return originalName;
        }


    }

    public sealed class DisplayNameProvider
    {
        private readonly Dictionary<string, string> _map;

        public DisplayNameProvider(string csvPath)
        {
            _map = DisplayNameCsvLoader.LoadDisplayNameMap(csvPath);
        }

        public string Get(string originalName)
        {
            return DisplayNameCsvLoader.ResolveDisplayName(_map, originalName);
        }

        public IReadOnlyDictionary<string, string> Map
        {
            get { return _map; }
        }
    }
}
