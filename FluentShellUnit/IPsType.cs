using System.Collections;

namespace FluentShellUnit
{
    internal interface IPsType<out TType> where TType : class 
    {
        TType StubFromFile(string fileName, bool isAbsolute = false);
        TType Stub(string cmdlet);
        TType Stub(string cmdlet, string value);
        PsHost Execute(string method, IDictionary parameters = null);
        PsHost Execute(IDictionary parameters = null);
    }
}