using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Management.Automation;

namespace FluentShellUnit
{
    public class PsModule : IPsType<PsModule>
    {
        private readonly PsHost _host;
        private readonly PsBase _psBase;

        private readonly string _moduleName;
        public PsModule(string fileName, PsHost host) 
        {
            _host = host;
            _psBase = new PsBase(_host);
            _moduleName = FetchModuleName(fileName);
        }

        private string FetchModuleName(string fileName)
        {
            fileName = fileName.Replace(".psm1", String.Empty);
            return fileName.Split(Path.DirectorySeparatorChar).Last();
        }

        public PsModule Load(string moduleFile, bool isAbsolute = false)
        {
            _psBase.Load(moduleFile, isAbsolute);
            return this;
        }

        public PsModule StubFromFile(string fileName, bool isAbsolute = false)
        {
            _psBase.StubFromFile(fileName, isAbsolute);
            return this;
        }

        public PsModule Stub(string cmdlet)
        {
            _psBase.Stub(cmdlet);
            return this;
        }

        public PsModule Stub(string cmdlet, string value)
        {
            _psBase.Stub(cmdlet, value);
            return this;
        }

        public PsHost Execute(IDictionary parameters = null)
        {
            throw new ArgumentException("You should provide a method name to execute");
        }

        public PsHost Execute(string method, IDictionary parameters = null)
        {
            _host.Shell.AddCommand(_moduleName + @"\" + method);
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

        public PsModule SetGlobalVariable(string name, string value)
        {
            _host.SetGlobalVariable(name, value);
            return this;
        }

        public PsModule LoadScriptBlock(string scriptBlock)
        {
            _host.LoadScriptBlock(scriptBlock).Execute();
            return this;
        }

        public PsModule LoadScriptFromFile(string file, bool isAbsolute = false)
        {
            _host.LoadScriptFromFile(file, isAbsolute).Execute();
            return this;
        }
    }
}
