namespace DisgraceDiscordBot.Entities
{
    public class BotConfig
    {
        public string Token { get; private set; }

        public string CommandPrefix { get; private set; } = "!";

        public int GoodColor { get; set; } = 0xFF0000;

        public int BadColor { get; set; } = 0xFF0000;

        public int TimeoutColor { get; set; } = 0xFF0000;

        public int CommonColor { get; set; } = 0xFF0000;
    }
}