using Microsoft.Extensions.DependencyInjection;
using Nutstone.Persistence.Provider.Models;
using Nutstone.Persistence.Provider.Registration;
using Nutstone.Persistence.Provider.Services.MsSql;
using PS.Help.Builder.Models;
using PS.Help.Builder.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace PS.Sql.Cmdlets
{
    public class BaseCmdlet : PSCmdlet
    {
        public BaseCmdlet()
        {
            // Load the SNI library if needed
            SniLoader.LoadSni();
        }

        [Parameter(Mandatory = false, HelpMessage = "Display help for this cmdlet")]
        public SwitchParameter Help { get; set; }

        protected IMsSqlService MsSqlService { get; private set; }

        protected override void BeginProcessing()
        {
            this.Initialise();
        }

        protected override void ProcessRecord()
        {
            if (Help.IsPresent)
            {
                this.ShowHelp();
            }
            else
            {
                Process();
            }
        }

        protected bool ShouldThrowError()
        {
            if (this.HaveSystemParameter("ErrorAction"))
            {
                if (MyInvocation.BoundParameters.TryGetValue("ErrorAction", out var errorActionString))
                {
                    ActionPreference errorAction = (ActionPreference)errorActionString;
                    if (errorAction == ActionPreference.Continue)
                    {
                        return false;
                    }
                }
            }
            return true;

        }

        protected void StopOnMissingParameter(object obj)
        {
            var isOK = true;
            if (obj != null)
            {
                if (obj.GetType() == typeof(string))
                {
                    isOK = !string.IsNullOrEmpty(obj.ToString());
                }
                else
                {
                    isOK = obj != null;
                }
            }
            else
            {
                isOK = false;
            }
            if (!isOK)
            {
                this.StopError(new Exception($"The required parameter -{obj.GetType().Name} is missing or invalid"), new object());
            }

        }

        protected void HandleVerbose(string msg)
        {
            if (this.HaveVerbose())
            {
                this.MessageWithColour(msg, ConsoleColor.Yellow);
            }
        }

        protected void ShowHelp()
        {
            var helperService = new HelpGeneratorService();
            var thisHelp = helperService.GetHelp(new List<Type> { this.GetType() });
            helperService.DisplayConsoleHelp(HelpFormaterConfig.Create(), thisHelp);
        }


        protected virtual void Process()
        {
            throw new NotImplementedException("You must override the Process method");
        }

        public void Message(string message)
        {
            Host.UI.WriteLine(message);
        }

        protected void Verbose(string message)
        {
            if (HaveVerbose())
            {
                MessageWithColour(message, ConsoleColor.Yellow);
            }
        }

        protected bool HaveVerbose()
        {
            if (HaveSystemParameter("Verbose"))
            {
                return true;
            }
            var verbosePref = (ActionPreference)SessionState.PSVariable.GetValue("VerbosePreference");
            if (verbosePref == ActionPreference.Continue)
            {
                return true;
            }
            return false;
        }

        public void MessageWithColour(string message, ConsoleColor colour)
        {
            Host.UI.WriteLine(colour, Host.UI.RawUI.BackgroundColor, message);
        }

        protected bool HaveSystemParameter(string systemParameter)
        {
            return MyInvocation.BoundParameters.ContainsKey(systemParameter);
        }

        protected Collection<PSObject> ExecuteScript(ScriptBlock scriptBlock, object[]? parameters)
        {
            try
            {
                return this.InvokeCommand.InvokeScript(true, scriptBlock, null, parameters ?? new object[0]);
            }
            catch (Exception ex)
            {
                this.StopError(ex, scriptBlock);
                return null;
            }
        }

        protected void StopError(Exception ex, object o)
        {
            ThrowTerminatingError(new ErrorRecord(ex, "1", ErrorCategory.MetadataError, o));
        }

        protected void MsgHandler(MsSqlLog msSqlLog)
        {
            switch (msSqlLog.LogLevel)
            {
                case MsSqlMessageType.Error: 
                    this.StopError(new Exception(msSqlLog.Message), msSqlLog);
                    break;
                case MsSqlMessageType.Warning:
                    this.MessageWithColour(msSqlLog.Message, ConsoleColor.Yellow);
                    break;
                case MsSqlMessageType.Information:
                    this.Message(msSqlLog.Message);
                    break;
                case MsSqlMessageType.Exception :
                    this.StopError(msSqlLog.Exception, msSqlLog);
                    break;
            }
        }

        private void Initialise()
        {
            var services = new ServiceCollection()
                .RegisterPersistence();
            var builder = services.BuildServiceProvider();
            this.MsSqlService = builder.GetRequiredService<IMsSqlService>();
        }
    }
}
