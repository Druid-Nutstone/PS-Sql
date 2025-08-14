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

        [Parameter(Mandatory = false)]
        public Type? OutputType { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter AsDynamic { get; set; }

        protected override void Process()
        {
            if (OutputType != null)
            {
                this.ProcessList();
            }
            else if (AsDynamic.IsPresent)
            {
                this.ProcessDynamic();
            }
            else
            {
                this.ThrowTerminatingError(new ErrorRecord(new ArgumentException("Either OutputType or AsDynamic must be specified."), "InvalidParameters", ErrorCategory.InvalidArgument, null));
            }
        }

        private void ProcessDynamic()
        {
            var dynamicList = new List<dynamic>();
            foreach (DataRow row in DataTable.Rows)
            {
                dynamicList.Add(row.RowToDynamic());
            }
            this.WriteObject(dynamicList);
        }

        private void ProcessList()
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
