using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotTraceApp.Settings
{
    public sealed class CustomerItemMasterSettings
    {
        public bool Enabled { get; set; }
        public string? ServerName { get; set; }
        public string? DatabaseName { get; set; }
        public string? SchemaName { get; set; } = "dbo";
        public string? TableName { get; set; }
        public string? ItemCodeColumnName { get; set; }
        public string? ItemNameColumnName { get; set; }
        public bool UseTrustedConnection { get; set; } = true;
        public int TimeoutSeconds { get; set; } = 5;

        public bool IsEnabledAndValid()
        {
            if (!Enabled) return false;

            return !string.IsNullOrWhiteSpace(ServerName)
                && !string.IsNullOrWhiteSpace(DatabaseName)
                && !string.IsNullOrWhiteSpace(SchemaName)
                && !string.IsNullOrWhiteSpace(TableName)
                && !string.IsNullOrWhiteSpace(ItemCodeColumnName)
                && !string.IsNullOrWhiteSpace(ItemNameColumnName);
        }
    }
}
