using Nutstone.Persistence.Provider.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace PS.Sql.Cmdlets
{
    [Cmdlet(VerbsCommon.New, "SqlParameters")]
    public class NewSqlParametersCmdlet : BaseCmdlet
    {
        protected override void Process()
        {
            this.WriteObject(new MsSqlStoredProcRequestParameterCollection());
        }
    }
}
