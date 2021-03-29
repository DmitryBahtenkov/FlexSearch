namespace FlexSearch.Router.Models
{
    public record Settings
    {
        public string Host { get; set; }
        public string Port { get; set; }
        
        public string Master { get; set; }
        public string[] Slaves { get; set; }
        
        public EmailData EmailData { get; set; }
        public EmailNotifications EmailNotifications { get; set; }
    }

    public record EmailData
    {
        public string EmailHost { get; set; }
        public string EmailPort { get; set; }
        public string EmailLogin { get; set; }
        public string EmailPassword { get; set; }
        public bool EnableSsl { get; set; }
    }

    public record EmailNotifications
    {
        public string[] Addresses { get; set; }
    }
}