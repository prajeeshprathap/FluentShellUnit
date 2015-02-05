using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;

namespace FluentShellUnit
{
    public class PsHost
    {
        private bool _failOnNonTerminatingError;
        public System.Management.Automation.PowerShell Shell { get; private set; }
        public Collection<PSObject> Result { get; set; }

        public PsHost(HostState context)
        {
            _failOnNonTerminatingError = false;
            Shell = CreateShell(context);
        }

        public PsHost() : this(HostState.Core)
        {
        }

        public PsModule Load(string moduleFile, bool isAbsolute = false)
        {
            if (!isAbsolute)
            {
                var currentDirectory = Directory.GetCurrentDirectory();
                moduleFile = Path.Combine(currentDirectory, moduleFile);
            }
            Shell.Commands.AddCommand("Import-Module")
                .AddArgument(moduleFile);
            Shell.Invoke();

            return new PsModule(moduleFile, this);
        }

        public PsScript LoadScriptBlock(string scriptBlock)
        {
            return new PsScript(scriptBlock, this);
        }

        public PsScript LoadScriptFromFile(string file, bool isAbsolute = false)
        {
            if (!isAbsolute)
            {
                var currentDirectory = Directory.GetCurrentDirectory();
                file = Path.Combine(currentDirectory, file);
            }
            return new PsScript(File.ReadAllText(file), this);
        }

        public PsHost SetGlobalVariable(string name, string value)
        {
            Shell.AddScript(String.Format("New-Variable -name {0} -Scope Global -Visibility Public -Value {1} -Force", name,
                value)
                );
            Shell.Invoke();
            return this;
        }

        public PsHost FailOnNonTerminatingError()
        {
            _failOnNonTerminatingError = true;
            return this;
        }

        private System.Management.Automation.PowerShell CreateShell(HostState context)
        {
            var state = CreateState(context);
            state.AuthorizationManager = new TestContextAuthorizationManager("PowerShellUnitTest");
            return System.Management.Automation.PowerShell.Create(state);
        }

        private InitialSessionState CreateState(HostState context)
        {
            switch (context)
            {
                case HostState.None:
                    return InitialSessionState.Create();
                case HostState.Core:
                    return InitialSessionState.CreateDefault2();
                case HostState.Full:
                    return InitialSessionState.CreateDefault();
                default:
                    throw new ArgumentException("Not a valid state");
            }
        }

        public void DisposeHost()
        {
            if (Shell == null) return;
            Shell.Dispose();
            Shell = null;
        }

        internal string HandleNonTerminatingErrors()
        {
            var errors = Shell.Streams.Error;
            if (errors == null || errors.Count <= 0) return String.Empty;

            var errorBuilder = new StringBuilder();
            foreach (var err in errors)
            {
                errorBuilder.AppendLine(err.ToString());
            }
            if (_failOnNonTerminatingError)
            {
                throw new RuntimeException(errorBuilder.ToString());
            }
            return errorBuilder.ToString();
        }

        public IEnumerable<T> ResultAs<T>()
        {
            if (Result == null)
            {
                throw new ArgumentException("You should first execute a function before querying the result");
            }
            return Result.Select(psObject => psObject.BaseObject).OfType<T>();
        }

        public T FirstResultItemAs<T>()
        {
            if (Result == null)
            {
                throw new ArgumentException("You should first execute a function before querying the result");
            }
            return Result.Select(psObject => psObject.BaseObject).OfType<T>().First();
        }
    }
}