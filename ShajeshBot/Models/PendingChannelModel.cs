namespace ShajeshBot.Models
{
    public class PendingChannelModel
    {
        public ulong? CategoryId { get; set; }
        public ulong RequestUserId { get; set; }
        public string ChannelName { get; set; }
    }
}
