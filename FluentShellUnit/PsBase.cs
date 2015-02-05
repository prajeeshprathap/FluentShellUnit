using System;
using System.IO;

namespace FluentShellUnit
{
    internal class PsBase
    {
        protected readonly PsHost Host;
        private readonly PsProxy _proxy;

        internal PsBase(PsHost host)
        {
            Host = host;
            _proxy = new PsProxy(Host);
        }

        public void Load(string moduleFile, bool isAbsolute = false)
        {
            Host.Load(moduleFile, isAbsolute);
        }

        public void StubFromFile(string fileName, bool isAbsolute = false)
        {
            if (!isAbsolute)
            {
                var currentDirectory = Directory.GetCurrentDirectory();
                fileName = Path.Combine(currentDirectory, fileName);
            }
            if (!File.Exists(fileName))
            {
                throw new ArgumentException("Failed to find a file {0}", fileName);
            }
            _proxy.CreateStubFromFile(fileName);
        }

        public void Stub(string cmdlet)
        {
            _proxy.CreateDummy(cmdlet);
        }

        public void Stub(string cmdlet, string value)
        {
            _proxy.CreateStub(cmdlet, value);
        }
    }
}