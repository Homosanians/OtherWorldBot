namespace DisgraceDiscordBot
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Since we cannot make the entry method asynchronous,
            // let's pass the execution to asynchronous code
            new Bot().InitAsync().GetAwaiter().GetResult();
        }
    }
}