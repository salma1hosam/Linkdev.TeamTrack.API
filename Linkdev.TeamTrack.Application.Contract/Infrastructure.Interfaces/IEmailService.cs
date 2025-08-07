namespace Linkdev.TeamTrack.Contract.Infrastructure.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string messageBody);
    }
}
