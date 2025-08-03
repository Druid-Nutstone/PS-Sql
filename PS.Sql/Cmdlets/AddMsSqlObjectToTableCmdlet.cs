using Nutstone.Persistence.Provider.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace PS.Sql.Cmdlets
{
    [Cmdlet(VerbsCommon.Add, "MsSqlObjectToTable")]
    public class AddMsSqlObjectToTableCmdlet : BaseCmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public MsSqlConnectionModel Connection { get; set; }

        [Parameter(Mandatory = true)]
        public string DatabaseName { get; set; }

        [Parameter(Mandatory = true)]
        public string TableName { get; set; }

        [Parameter(Mandatory = true)]
        public object Data { get; set; }

        protected override void Process()
        {
            if (string.IsNullOrEmpty(DatabaseName))
            {
                this.ThrowTerminatingError(new ErrorRecord(new ArgumentException("DatabaseName cannot be null or empty"), "InvalidDatabaseName", ErrorCategory.InvalidArgument, null));
            }
            this.MsSqlService.WithSqlConnection(Connection, MsgHandler)
                             .WithOpenConnection(MsgHandler)
                             .WithObjectInsertArray(Data, TableName, MsgHandler)
                             .WithCloseConnection(MsgHandler);
        }

    }
}
