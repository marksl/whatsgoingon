namespace ServiceExample
{
    public class MyService : IMyService
    {
        public string Add(int a, int b, int c)
        {
            return (a + b + c).ToString();
        }

        public string Sub(int a, int b)
        {
            return (a - b).ToString();
        }
    }
}
