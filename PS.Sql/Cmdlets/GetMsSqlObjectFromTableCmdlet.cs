using Nutstone.Persistence.Provider.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace PS.Sql.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "MsSqlObjectFromTable")]
    public class GetMsSqlObjectFromTableCmdlet : BaseCmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public MsSqlConnectionModel Connection { get; set; }

        [Parameter(Mandatory = true)]
        public string DatabaseName { get; set; }

        [Parameter(Mandatory = true)]
        public string TableName { get; set; }

        [Parameter(Mandatory = true)]
        public Type TableType { get; set; }
    
        protected override void Process()
        {
            if (string.IsNullOrEmpty(DatabaseName))
            {
                Connection.WithDatabase(DatabaseName);
            }
            var result = this.MsSqlService.WithSqlConnection(Connection, MsgHandler)
                             .WithOpenConnection(MsgHandler)
                             .WithGetTable(TableName, TableType, MsgHandler);
            this.MsSqlService.WithCloseConnection(MsgHandler);
            this.WriteObject(result);
        }

    }
}
