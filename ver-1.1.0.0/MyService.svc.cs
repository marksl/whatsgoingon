namespace ServiceExample
{
    public class MyService : IMyService
    {
        public string Add(int a, int b)
        {
            return (a + b).ToString();
        }

        public string Sub(int a, int b)
        {
            return (a - b).ToString();
        }
    }
}
