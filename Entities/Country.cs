namespace DisgraceDiscordBot.Entities
{
    public class Country
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int DisgracePoints { get; set; }
        public long LastUpdateTimestamp { get; set; }
    }
}