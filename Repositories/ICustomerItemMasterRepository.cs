using LotTraceApp.Forms;
using LotTraceApp.Settings;
using LotTraceApp.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace LotTraceApp.Repositories
{
    public interface ICustomerItemMasterRepository
    {
        Dictionary<string, string> GetItemNamesByCodes(IEnumerable<string> itemCodes);
        List<string> GetItemCodeByName(string itemName);
    }


    public sealed class CustomerItemMasterRepository : ICustomerItemMasterRepository
    {
        private readonly IniFile _ini;

        private readonly Dictionary<string, string> _itemNameCache
    = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public CustomerItemMasterRepository(IniFile ini)
        {
            if (ini == null)
            {
                throw new ArgumentNullException("ini");
            }

            _ini = ini;
        }

        public List<string> GetItemCodeByName(string itemName)
        {
            List<string> result = new List<string>();

            if (string.IsNullOrWhiteSpace(itemName))
            {
                return result;
            }

            var settings = LoadCustomerItemMasterSettings(_ini);
            if (!settings.IsEnabledAndValid())
            {
                return result;
            }

            ValidateSqlIdentifier(settings.SchemaName, "SchemaName");
            ValidateSqlIdentifier(settings.TableName, "TableName");
            ValidateSqlIdentifier(settings.ItemCodeColumnName, "ItemCodeColumnName");
            ValidateSqlIdentifier(settings.ItemNameColumnName, "ItemNameColumnName");

            string connectionString = BuildConnectionString(settings);
            string sql = BuildSelectSql2(settings, 500);

            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand(sql, connection))
            {
                command.CommandType = CommandType.Text;
                command.CommandTimeout = settings.TimeoutSeconds;


                command.Parameters.AddWithValue("@p", ConvertWildcardPattern(itemName));

                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    var itemCodeOrdinal = reader.GetOrdinal(settings.ItemCodeColumnName);

                    


                    while (reader.Read())
                    {
                        result.Add(reader.IsDBNull(itemCodeOrdinal) ? string.Empty : reader.GetString(itemCodeOrdinal));
                    }
                }
            }

            return result;
        }


        public Dictionary<string, string> GetItemNamesByCodes(IEnumerable<string> itemCodes)
        {
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (itemCodes == null)
            {
                return result;
            }

            var normalizedCodes = itemCodes
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (normalizedCodes.Count == 0)
            {
                return result;
            }

            var codesToFetch = new List<string>();

            foreach (var code in normalizedCodes)
            {
                string cachedName;
                if (_itemNameCache.TryGetValue(code, out cachedName))
                {
                    result[code] = cachedName;
                }
                else
                {
                    codesToFetch.Add(code);
                }
            }

            if (codesToFetch.Count == 0)
            {
                return result;
            }

            var settings = LoadCustomerItemMasterSettings(_ini);
            if (!settings.IsEnabledAndValid())
            {
                return result;
            }

            ValidateSqlIdentifier(settings.SchemaName, "SchemaName");
            ValidateSqlIdentifier(settings.TableName, "TableName");
            ValidateSqlIdentifier(settings.ItemCodeColumnName, "ItemCodeColumnName");
            ValidateSqlIdentifier(settings.ItemNameColumnName, "ItemNameColumnName");

            string connectionString = BuildConnectionString(settings);
            string sql = BuildSelectSql1(settings, codesToFetch.Count);

            var fetched = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand(sql, connection))
            {
                command.CommandType = CommandType.Text;
                command.CommandTimeout = settings.TimeoutSeconds;

                for (int i = 0; i < codesToFetch.Count; i++)
                {
                    command.Parameters.AddWithValue("@p" + i, codesToFetch[i]);
                }

                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string itemCode = reader[settings.ItemCodeColumnName] == DBNull.Value
                            ? null
                            : Convert.ToString(reader[settings.ItemCodeColumnName]);

                        string itemName = reader[settings.ItemNameColumnName] == DBNull.Value
                            ? null
                            : Convert.ToString(reader[settings.ItemNameColumnName]);

                        if (string.IsNullOrWhiteSpace(itemCode))
                        {
                            continue;
                        }

                        itemCode = itemCode.Trim();

                        if (!fetched.ContainsKey(itemCode))
                        {
                            fetched[itemCode] = itemName ?? string.Empty;
                        }
                    }
                }
            }

            foreach (var code in codesToFetch)
            {
                string itemName;
                if (fetched.TryGetValue(code, out itemName))
                {
                    _itemNameCache[code] = itemName;
                    result[code] = itemName;
                }
                else
                {
                    // 未登録もキャッシュして無駄な再検索を防ぐ
                    _itemNameCache[code] = string.Empty;
                }
            }

            return result;
        }

        private CustomerItemMasterSettings LoadCustomerItemMasterSettings(IniFile ini)
        {
            const string section = "CustomerItemMaster";

            return new CustomerItemMasterSettings
            {
                Enabled = ini.GetBool(section, "Enabled", false),
                ServerName = ini.GetString(section, "ServerName", string.Empty),
                DatabaseName = ini.GetString(section, "DatabaseName", string.Empty),
                SchemaName = ini.GetString(section, "SchemaName", "dbo"),
                TableName = ini.GetString(section, "TableName", string.Empty),
                ItemCodeColumnName = ini.GetString(section, "ItemCodeColumnName", string.Empty),
                ItemNameColumnName = ini.GetString(section, "ItemNameColumnName", string.Empty),
                UseTrustedConnection = ini.GetBool(section, "UseTrustedConnection", true),
                TimeoutSeconds = ini.GetInt(section, "TimeoutSeconds", 5)
            };
        }

        private string BuildConnectionString(CustomerItemMasterSettings settings)
        {
            var sb = new SqlConnectionStringBuilder
            {
                DataSource = settings.ServerName,
                InitialCatalog = settings.DatabaseName,
                IntegratedSecurity = settings.UseTrustedConnection,
                ConnectTimeout = settings.TimeoutSeconds,
                TrustServerCertificate = true
            };

            return sb.ConnectionString;
        }

        private string BuildSelectSql1(CustomerItemMasterSettings settings, int codeCount)
        {
            var parameterNames = Enumerable.Range(0, codeCount)
                .Select(i => "@p" + i)
                .ToArray();

            return string.Format(
                "SELECT {0}, {1} FROM {2}.{3} WHERE {0} IN ({4})",
                Bracket(settings.ItemCodeColumnName),
                Bracket(settings.ItemNameColumnName),
                Bracket(settings.SchemaName),
                Bracket(settings.TableName),
                string.Join(", ", parameterNames));
        }

        private string BuildSelectSql2(CustomerItemMasterSettings settings, int topCount)
        {
            return string.Format(
                "SELECT TOP ({0}) {1}, {2} FROM {3}.{4} WHERE {2} LIKE @p ORDER BY {2}, {1}",
                topCount,
                Bracket(settings.ItemCodeColumnName),
                Bracket(settings.ItemNameColumnName),
                Bracket(settings.SchemaName),
                Bracket(settings.TableName));
        }

        private void ValidateSqlIdentifier(string value, string parameterName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException(parameterName + " is empty.", parameterName);
            }

            foreach (char c in value)
            {
                bool ok =
                    (c >= 'a' && c <= 'z') ||
                    (c >= 'A' && c <= 'Z') ||
                    (c >= '0' && c <= '9') ||
                    c == '_';

                if (!ok)
                {
                    throw new ArgumentException(parameterName + " contains invalid character: " + value, parameterName);
                }
            }
        }

        private string Bracket(string identifier)
        {
            return "[" + identifier.Replace("]", "]]") + "]";
        }

        private string ConvertWildcardPattern(string value)
        {
            return value
                .Replace("[", "[[]")
                .Replace("%", "[%]")
                .Replace("_", "[_]")
                .Replace("*", "%");
        }
    }
}

