using System.IO;
using System.Management.Automation;

namespace FluentShellUnit
{
    internal class PsProxy
    {
        private readonly PsHost _host;

        public PsProxy(PsHost host)
        {
            _host = host;
        }

        internal void CreateDummy(string cmdlet)
        {
            try
            {
                var script = string.Format("Function {0} {{}}", cmdlet);
                _host.Shell.AddScript(script).Invoke();
                _host.HandleNonTerminatingErrors();
            }
            catch (RuntimeException)
            {
                _host.DisposeHost();
                throw;
            }
        }

        internal void CreateStub(string cmdlet, string value)
        {
            try
            {
                var script = string.Format("Function {0} {{ return {1}}}", cmdlet, value);
                _host.Shell.AddScript(script).Invoke();
                _host.HandleNonTerminatingErrors();
            }
            catch (RuntimeException)
            {
                _host.DisposeHost();
                throw;
            }
        }

        internal void CreateStubFromFile(string fileName)
        {
            try
            {
                var scriptBlock = File.ReadAllText(fileName);
                _host.Shell.AddScript(scriptBlock).Invoke();
                _host.HandleNonTerminatingErrors();
            }
            catch (RuntimeException)
            {
                _host.DisposeHost();
                throw;
            }
        }
    }
}
