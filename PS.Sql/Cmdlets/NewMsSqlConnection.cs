using Nutstone.Persistence.Provider.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace PS.Sql.Cmdlets
{
    [Cmdlet(VerbsCommon.New, "MsSqlConnection")]
    public class NewMsSqlConnection : BaseCmdlet
    {
        [Parameter(Mandatory = false)]
        public string? DataSource { get; set; }

        [Parameter(Mandatory = false)]
        public string? Database { get; set; }

        [Parameter(Mandatory = false)]
        public string? UserId { get; set; }

        [Parameter(Mandatory = false)]
        public string? Password { get; set; }

        protected override void ProcessRecord()
        {
            var defaultConnection = MsSqlConnectionModel.Create()
                                                        .WithDataSource(GetEnvVariable(nameof(MsSqlConnectionModel.DataSource)) ?? this.DataSource)  
                                                        .WithDatabase(GetEnvVariable(nameof(MsSqlConnectionModel.Database)) ?? this.Database)
                                                        .WithUserId(GetEnvVariable(nameof(MsSqlConnectionModel.UserId)) ?? this.UserId)
                                                        .WithPassword(GetEnvVariable(nameof(MsSqlConnectionModel.Password)) ?? this.Password)
                                                        .WithIntegratedSecurity(bool.Parse(GetEnvVariable(nameof(MsSqlConnectionModel.IntegratedSecurity)) ?? "False"))
                                                        .WithPort(int.Parse(GetEnvVariable(nameof(MsSqlConnectionModel.Port)) ?? "1433"));
            if (defaultConnection.IsValid())
            {
                this.WriteObject(defaultConnection);
            }
            else
            {
                this.ThrowTerminatingError(new ErrorRecord(new ArgumentException("Invalid connection parameters"), "InvalidConnectionParameters", ErrorCategory.InvalidArgument, null));
            }
        }

        private string? GetEnvVariable(string key)
        {
            var val = Environment.GetEnvironmentVariable($"PowerShellSql{key}", EnvironmentVariableTarget.User); 
            return string.IsNullOrEmpty(val) ? null : val;
        }
    }
}
