using Nutstone.Persistence.Provider.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace PS.Sql.Cmdlets
{
    [Cmdlet(VerbsCommon.Find, "Table")]
    public class FindTableCmdlet : BaseCmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public DataSet DataSet { get; set; }    

        [Parameter(Mandatory = true)]
        public string TableName { get; set; }

        protected override void Process()
        {
            if (DataSet == null)
            {
                this.ThrowTerminatingError(new ErrorRecord(new ArgumentException("DataSet cannot be null"), "InvalidDataSet", ErrorCategory.InvalidArgument, null));
            }
            var table = DataSet.GetTableByName(TableName);
            this.WriteObject(table);
        }
    
    }
}
