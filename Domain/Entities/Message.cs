namespace Domain.Entities
{
    public class Message
    {

        public string Text { get; set; }
        public int IdService { get; set; }
        public long Timestamp { get; set; }
        public int RandomId { get; set; }

        public Message(string text, int idService, long timestamp, int randomId)
        {
            this.Text = text;
            this.IdService = idService;
            this.Timestamp = timestamp;
            this.RandomId = randomId;
        }
        public string MessageToString()
        {
            return $"{Text} {IdService} {Timestamp} {RandomId}";
        }


    }
}
