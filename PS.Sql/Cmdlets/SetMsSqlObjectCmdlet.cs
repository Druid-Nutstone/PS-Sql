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
    [Cmdlet(VerbsCommon.Set, "MsSqlObject")]
    public class SetMsSqlObjectCmdlet : BaseCmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline =true)]
        public DataTable DataTable { get; set; }

        [Parameter(Mandatory = true)]
        public Type OutputType { get; set; }    

        protected override void Process()
        {
            var listType = typeof(List<>).MakeGenericType(OutputType);
            var listInstance = Activator.CreateInstance(listType);
            var addMethod = listType.GetMethod("Add");
            foreach (DataRow row in DataTable.Rows)
            {
                addMethod.Invoke(listInstance, new object[] { row.RowToObject(OutputType) });
            }
            this.WriteObject(listInstance); 
        }

    }
}
