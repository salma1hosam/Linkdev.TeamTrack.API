namespace Linkdev.TeamTrack.Infrastructure.EmailService
{
    public class SmtpConfiguration
    {
        public string SmtpEmail { get; set; }
        public string SmtpPassword { get; set; }
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
    }
}
