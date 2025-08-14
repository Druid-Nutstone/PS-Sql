using Nutstone.Persistence.Provider.Extensions;
using Nutstone.Persistence.Provider.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace PS.Sql.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "MsSql")]
    public class GetMsSqlCmdlet : BaseCmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public MsSqlConnectionModel Connection { get; set; }

        [Parameter(Mandatory = true)]
        public string Sql { get; set; }

        [Parameter(Mandatory = false)]
        public string DatabaseName { get; set; }

        [Parameter(Mandatory = true)]
        public MsSqlOutputType Output { get; set; }

        protected override void Process()
        {
            object result = null;
            
            if (!string.IsNullOrEmpty(DatabaseName))
            {
                Connection.WithDatabase(DatabaseName);
            }
            
            switch (Output)
            {
                case MsSqlOutputType.DataSet:
                    result = this.MsSqlService.WithSqlConnection(Connection, MsgHandler)
                                                    .WithOpenConnection(MsgHandler)
                                                    .WithGetDataSet(Sql, MsgHandler);
                    break;
                case MsSqlOutputType.DataTable:
                    result = this.MsSqlService.WithSqlConnection(Connection, MsgHandler)
                                                      .WithOpenConnection(MsgHandler)
                                                      .WithGetDataTable(Sql, MsgHandler);
                    break;
                case MsSqlOutputType.Dynamic:
                    result = this.GetAsDynmaic();
                    break;
                default:
                    throw new ArgumentException("Invalid output type specified.");
            }
            this.MsSqlService.WithCloseConnection(MsgHandler);
            this.WriteObject(result);   
        }

        private object GetAsDynmaic()
        {
            var dataTable = this.MsSqlService.WithSqlConnection(Connection, MsgHandler)
                                              .WithOpenConnection(MsgHandler)
                                              .WithGetDataTable(Sql, MsgHandler);
            var dynamicList = new List<dynamic>();
            foreach (DataRow row in dataTable.Rows)
            {
                dynamicList.Add(row.RowToDynamic());
            }
            return dynamicList;
        }
    }
}
