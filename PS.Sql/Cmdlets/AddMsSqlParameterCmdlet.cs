using Nutstone.Persistence.Provider.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace PS.Sql.Cmdlets
{
    [Cmdlet(VerbsCommon.Add, "MsSqlParameter")]
    public class AddMsSqlParameterCmdlet : BaseCmdlet
    {
        [Parameter(Mandatory = false, ValueFromPipeline = true)]
        public MsSqlStoredProcRequestParameterCollection Parameters { get; set; }

        [Parameter(Mandatory = true)]
        public string Name { get; set; }
        
        [Parameter(Mandatory = true)]
        public object Value { get; set; }

        protected override void Process()
        {
            if (Parameters == null)
            {
                Parameters = new MsSqlStoredProcRequestParameterCollection();
            }   

            Parameters.Add(Name, Value);
            this.WriteObject(Parameters);
        }
    }
}
