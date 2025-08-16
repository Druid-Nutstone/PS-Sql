using Nutstone.Persistence.Provider.Extensions;
using Nutstone.Persistence.Provider.Models;
using Nutstone.Persistence.Provider.Services.MsSql;
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

        [Parameter(Mandatory = false)]
        public MsSqlInputType Input { get; set; } = MsSqlInputType.Sql;

        [Parameter(Mandatory = false)]
        public Type? Type { get; set; }

        [Parameter(Mandatory = false)]
        public MsSqlStoredProcRequestParameterCollection SqlParameters;
            
        private Dictionary<MsSqlOutputType, Func<object>> processors;

        public GetMsSqlCmdlet()
        {
            this.processors = new Dictionary<MsSqlOutputType, Func<object>>()
            {
                { MsSqlOutputType.Dynamic, this.GetAsDynmaic },
                { MsSqlOutputType.DataTable, this.GetAsDataTable },
                { MsSqlOutputType.DataSet, this.GetAsDataSet },
                { MsSqlOutputType.Object, this.GetAsObject } // Assuming Object is similar to DataTable for this context
            };
        }

        protected override void Process()
        {
            object result = null;
            
            if (!string.IsNullOrEmpty(DatabaseName))
            {
                Connection.WithDatabase(DatabaseName);
            }
            
            if (this.processors.ContainsKey(Output))
            {
                result = this.processors[Output]();
            }
            else
            {
                throw new ArgumentException($"Unsupported output type: {Output}");
            }

            this.MsSqlService.WithCloseConnection(MsgHandler);
            this.WriteObject(result);   
        }

        private object GetAsObject()
        {
            if (Type == null)
            {
                throw new ArgumentNullException(nameof(Type), "Type must be specified for Object output.");
            }

            if (Input == MsSqlInputType.StoredProcedure)
            {
                var request = MsSqlStoredProcRequest.Create()
                                                    .WithName(Sql)
                                                    .WithParameters(SqlParameters);
                return this.GetConnection()
                           .WithGetObjectListFromStoredProcedure(request, Type, MsgHandler);    
            }
            else
            {
                return this.GetConnection()
                           .WithGetObjectList(Sql, Type, MsgHandler);
            }
        }

        private object GetAsDataTable()
        {
            if (Input == MsSqlInputType.StoredProcedure)
            {
                var request = MsSqlStoredProcRequest.Create()
                                                    .WithName(Sql)
                                                    .WithParameters(SqlParameters);
                var ds = this.GetConnection()
                           .WithGetStoredProcedure(request, MsgHandler);
                if (ds.Tables.Count > 0)
                {
                    return ds.Tables[0];
                }
                else
                {
                    throw new InvalidOperationException("Stored procedure did not return any data.");
                }
            }
            else
            {
                return this.GetConnection()
                           .WithGetDataTable(Sql, MsgHandler);
            }
        }

        private object GetAsDataSet()
        {
            if (Input == MsSqlInputType.StoredProcedure)
            {
                var request = MsSqlStoredProcRequest.Create()
                                                    .WithName(Sql)
                                                    .WithParameters(SqlParameters);
                return this.GetConnection()
                           .WithGetStoredProcedure(request, MsgHandler);
            }
            else
            {
                return this.GetConnection()
                           .WithGetDataSet(Sql, MsgHandler);
            }
        }

        private object GetAsDynmaic()
        {
            if (Input == MsSqlInputType.StoredProcedure)
            {
                var request = MsSqlStoredProcRequest.Create()
                                                    .WithName(Sql)
                                                    .WithParameters(SqlParameters);
                var ds =  this.GetConnection()
                              .WithGetStoredProcedure(request, MsgHandler);
                
                if (ds.Tables.Count > 0)
                {
                    return this.GetDynamicFromDataTable(ds.Tables[0]);
                }
                else
                {
                    throw new InvalidOperationException("Stored procedure did not return any data.");
                }
            }
            else
            {
                var dataTable = this.GetConnection()
                                    .WithGetDataTable(Sql, MsgHandler);
                return this.GetDynamicFromDataTable(dataTable);
            }
        }

        private object GetDynamicFromDataTable(DataTable dataTable)
        {
            var dynamicList = new List<dynamic>();
            foreach (DataRow row in dataTable.Rows)
            {
                dynamicList.Add(row.RowToDynamic());
            }
            return dynamicList;
        }

        private IMsSqlService GetConnection()
        {
           return this.MsSqlService.WithSqlConnection(Connection, MsgHandler)
                                   .WithOpenConnection(MsgHandler);
        }
    }
}
