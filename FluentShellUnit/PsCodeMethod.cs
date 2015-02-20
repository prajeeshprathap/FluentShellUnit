using System;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Reflection;

namespace FluentShellUnit
{
    public delegate object PsDelegate(params object[] argsObjects);
    public class PsCodeMethod : PSMethodInfo
    {
        private readonly string _name;
        private readonly PsDelegate _action;
        private readonly Func<object> _method;

        public PsCodeMethod(string name, Func<object> method)
        {
            _name = name;
            _method = method;

            var nameField = this.GetType().GetField("name",
                BindingFlags.NonPublic | BindingFlags.Instance);
            nameField?.SetValue(this, name);
        }

        public PsCodeMethod(string name, PsDelegate action)
        {
            _name = name;
            _action = action;

            var nameField = this.GetType().GetField("name",
                BindingFlags.NonPublic | BindingFlags.Instance);
            nameField?.SetValue(this, name);
        }

        public override PSMemberInfo Copy()
        {
            if (_method != null)
            {
                return new PsCodeMethod(_name, _method);
            }
            return new PsCodeMethod(_name, _action);
        }

        public override PSMemberTypes MemberType => PSMemberTypes.CodeMethod;

        public override string TypeNameOfValue => typeof (PSObject).ToString();

        public override object Invoke(params object[] arguments)
        {
            return _method != null ? _method.Invoke() : _action.Invoke(arguments);
        }

        public override Collection<string> OverloadDefinitions => new Collection<string>();
    }
}