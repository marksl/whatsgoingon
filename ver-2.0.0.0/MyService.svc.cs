namespace ServiceExample
{
    public class MyService : IMyService
    {
        public EchoData Echo(EchoData data)
        {
            return data;
        }
    }
}
