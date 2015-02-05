using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Management.Automation;

namespace FluentShellUnit
{
    public class PsScript : IPsType<PsScript>
    {
        private readonly string _scriptBlock;
        private readonly PsHost _host;
        private readonly PsBase _psBase;

        public PsScript(string scriptBlock, PsHost host)
        {
            _scriptBlock = scriptBlock;
            _host = host;
            _psBase = new PsBase(host);
        }

        public PsScript StubFromFile(string fileName, bool isAbsolute = false)
        {
            _psBase.StubFromFile(fileName, isAbsolute);
            return this;
        }

        public PsScript Stub(string cmdlet)
        {
            _psBase.Stub(cmdlet);
            return this;
        }

        public PsScript Stub(string cmdlet, string value)
        {
            _psBase.Stub(cmdlet, value);
            return this;
        }

        public PsHost Execute(string method, IDictionary parameters = null)
        {
            _host.Shell.AddScript(_scriptBlock);
            _host.Shell.AddCommand(method);
            if (parameters != null)
            {
                _host.Shell.AddParameters(parameters);
            }
            _host.Result = _host.Shell.Invoke();
            var error = _host.HandleNonTerminatingErrors();
            if (!String.IsNullOrEmpty(error))
            {
                _host.Result = new Collection<PSObject> { new PSObject(error) };
            }
            return _host;
        }

        public PsHost Execute(IDictionary parameters = null)
        {
            _host.Shell.AddScript(_scriptBlock);
            if (parameters != null)
            {
                _host.Shell.AddParameters(parameters);
            }
            _host.Result = _host.Shell.Invoke();
            var error = _host.HandleNonTerminatingErrors();
            if (!String.IsNullOrEmpty(error))
            {
                _host.Result = new Collection<PSObject> { new PSObject(error) };
            }
            return _host;
        }
    }
}
