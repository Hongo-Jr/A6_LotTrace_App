using System;
using System.Collections.Generic;
using System.IO;

namespace LotTraceApp.Utils
{
    /// <summary>
    /// 簡易 INI ファイルリーダー（読み取り専用）
    ///   [セクション]
    ///   Key = Value
    /// という形式を想定
    /// </summary>
    public class IniFile
    {
        private readonly Dictionary<string, Dictionary<string, string>> _data =
            new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);

        public IniFile(string path)
        {
            if (path == null) throw new ArgumentNullException("path");
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("INI ファイルが見つかりません。", path);
            }

            Load(path);
        }

        private void Load(string path)
        {
            string? currentSection = null;

            string[] lines = File.ReadAllLines(path);
            foreach (string raw in lines)
            {
                string line = raw.Trim();

                // 空行・コメント行はスキップ
                if (line.Length == 0) continue;
                if (line.StartsWith(";") || line.StartsWith("#")) continue;

                // セクション行 [Section]
                if (line.StartsWith("[") && line.EndsWith("]") && line.Length > 2)
                {
                    currentSection = line.Substring(1, line.Length - 2).Trim();
                    if (!_data.ContainsKey(currentSection))
                    {
                        _data[currentSection] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    }
                    continue;
                }

                // キー=値 行
                int idx = line.IndexOf('=');
                if (idx <= 0) continue;

                string key = line.Substring(0, idx).Trim();
                string value = line.Substring(idx + 1).Trim();

                if (currentSection == null)
                {
                    // セクション無し行は無視
                    continue;
                }

                
                if (!_data.TryGetValue(currentSection, out var section))
                {
                    section = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    _data[currentSection] = section;
                }

                section[key] = value;
            }
        }

        /// <summary>
        /// section/key の値を取得（見つからなければ null）
        /// </summary>
        public string? GetValue(string? section, string? key)
        {
            if (section == null) throw new ArgumentNullException("section");
            if (key == null) throw new ArgumentNullException("key");

            
            if (!_data.TryGetValue(section, out var sec))
            {
                return null;
            }

            
            if (!sec.TryGetValue(key, out var value))
            {
                return null;
            }

            return value;
        }

        public string? GetString(string section, string key, string? defaultValue)
        {
            string? value = GetValue(section, key);
            return string.IsNullOrWhiteSpace(value) ? defaultValue : value;
        }

        public bool GetBool(string section, string key, bool defaultValue)
        {
            string? value = GetValue(section, key);
            if (string.IsNullOrWhiteSpace(value))
            {
                return defaultValue;
            }

            value = value.Trim();

            if (string.Equals(value, "1", StringComparison.OrdinalIgnoreCase)) return true;
            if (string.Equals(value, "0", StringComparison.OrdinalIgnoreCase)) return false;
            if (string.Equals(value, "true", StringComparison.OrdinalIgnoreCase)) return true;
            if (string.Equals(value, "false", StringComparison.OrdinalIgnoreCase)) return false;
            if (string.Equals(value, "yes", StringComparison.OrdinalIgnoreCase)) return true;
            if (string.Equals(value, "no", StringComparison.OrdinalIgnoreCase)) return false;

            return defaultValue;
        }

        public int GetInt(string section, string key, int defaultValue)
        {
            string? value = GetValue(section, key);
            if (string.IsNullOrWhiteSpace(value))
            {
                return defaultValue;
            }

            int result;
            return int.TryParse(value.Trim(), out result) ? result : defaultValue;
        }
    }
}