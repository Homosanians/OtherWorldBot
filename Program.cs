namespace OtherWorldBot
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            new Bot().InitAsync().GetAwaiter().GetResult();
        }
    }
}
