namespace AppY.ViewModels
{
    public class SendEmail
    {
        public string? Title { get; set; }
        public string? Subject { get; set; }
        public string? Body { get; set; }
        public string? SendTo { get; set; }
        public string? SentFrom { get; set; } = "bluejade@mail.ru";
    }
}
