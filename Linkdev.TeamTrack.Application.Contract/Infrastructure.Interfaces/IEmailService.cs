namespace Linkdev.TeamTrack.Contract.Infrastructure.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(IEnumerable<string> toEmails, string subject, string messageBody);
    }
}
