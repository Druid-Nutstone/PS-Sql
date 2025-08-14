using Nutstone.Persistence.Provider.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace PS.Sql.Cmdlets
{
    [Cmdlet(VerbsCommon.Set, "MsSql")]
    public class SetMsSqlCmdlet : BaseCmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public MsSqlConnectionModel Connection { get; set; }

        [Parameter(Mandatory = true)]
        public string Sql { get; set; }

        protected override void Process()
        {
            if (Connection == null)
            {
                this.ThrowTerminatingError(new ErrorRecord(new ArgumentNullException(nameof(Connection)), "ConnectionNotSpecified", ErrorCategory.InvalidArgument, null));
            }
            var result = this.MsSqlService.WithSqlConnection(Connection, MsgHandler)
                             .WithOpenConnection(MsgHandler)
                             .WithExecuteNonQuery(Sql, MsgHandler);
            this.MsSqlService.WithCloseConnection(MsgHandler);
            if (result != null)
            {
                this.WriteObject(result);
            }
            else
            {
                this.WriteWarning("No rows affected or returned by the SQL command.");
            }
        }
    }
}
