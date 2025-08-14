using Nutstone.Persistence.Provider.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace PS.Sql.Cmdlets
{
    [Cmdlet(VerbsCommon.Set, "MsSqlConnection")]
    public class SetMsSqlConnection : BaseCmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public MsSqlConnectionModel Connection { get; set; }

        [Parameter(Mandatory = false)]
        public string DatabaseName { get; set; }    

        [Parameter(Mandatory = false)]
        public string UserId { get; set; }    

        protected override void ProcessRecord()
        {
            if (!string.IsNullOrEmpty(DatabaseName))
            {
                Connection.Database = DatabaseName;
            }
            if (!string.IsNullOrEmpty(UserId))
            {
                Connection.UserId = UserId;
            }
            if (!string.IsNullOrEmpty(Connection.DataSource))
            {
                Connection.DataSource = Connection.DataSource.Trim();
            }
            if (!string.IsNullOrEmpty(Connection.Password))
            {
                Connection.Password = Connection.Password.Trim();
            }
            if (Connection.IsValid())
            {
                this.WriteObject(Connection);
            }
            else             {
                this.ThrowTerminatingError(new ErrorRecord(new ArgumentException("Invalid connection parameters"), "InvalidConnectionParameters", ErrorCategory.InvalidArgument, null));
            }
        }

    }
}
