using Nutstone.Persistence.Provider.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace PS.Sql.Cmdlets
{
    [Cmdlet(VerbsCommon.Remove, "MsSqlTable")]
    public class RemoveMsSqlTable : BaseCmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public MsSqlConnectionModel Connection { get; set; }

        [Parameter(Mandatory = true)]
        public string TableName { get; set; }

        [Parameter(Mandatory = false)]
        public string DatabaseName { get; set; } = string.Empty;

        protected override void Process()
        {
            if (!string.IsNullOrEmpty(DatabaseName))
            {
                this.Connection = this.Connection.WithDatabase(DatabaseName);
            }
            this.MsSqlService.WithSqlConnection(Connection, MsgHandler)
                             .WithOpenConnection(MsgHandler)
                             .WithDropTable(TableName, MsgHandler)
                             .WithCloseConnection(MsgHandler);
        }
    }
}
