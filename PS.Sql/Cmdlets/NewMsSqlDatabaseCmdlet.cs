using Nutstone.Persistence.Provider.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace PS.Sql.Cmdlets
{
    [Cmdlet(VerbsCommon.New, "MsSqlDatabase")]
    public class NewMsSqlDatabaseCmdlet : BaseCmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public MsSqlConnectionModel Connection { get; set; }

        [Parameter(Mandatory = true)]
        public string DatabaseName { get; set; }

        protected override void Process()
        {
            this.MsSqlService.WithSqlConnection(Connection, MsgHandler)
                             .WithOpenConnection(MsgHandler)
                             .WithCreateDatabase(DatabaseName, MsgHandler)
                             .WithCloseConnection(MsgHandler);

        }

    }
}
