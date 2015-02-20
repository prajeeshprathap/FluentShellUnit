using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace FluentShellUnit
{
    public static class PSObjectExtensions
    {
        public static PSObject StubMethod(this PSObject psObject, string name, Func<object> method)
        {
            psObject.Methods.Add(new PsCodeMethod(name, method));
            return psObject;
        }
        public static PSObject StubMethod(this PSObject psObject, string name, PsDelegate psDelegate)
        {
            psObject.Methods.Add(new PsCodeMethod(name, psDelegate));
            return psObject;
        }

        public static PSObject StubProperty(this PSObject psObject, string name, object value)
        {
            psObject.Properties.Add(new PSNoteProperty(name, value));
            return psObject;
        }
    }
}
