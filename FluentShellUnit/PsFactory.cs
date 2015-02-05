namespace FluentShellUnit
{
    public class PsFactory
    {
        public static PsHost Create()
        {
            return new PsHost();
        }

        public static PsHost Create(HostState context)
        {
            return new PsHost(context);
        }

    }
}
