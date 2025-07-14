using Congrapp.Server.Data;
using Congrapp.Server.Models;
using FluentEmail.Core;

namespace Congrapp.Server.Services;

public class EmailVerificationService(IConfiguration config, IFluentEmail fluentEmail)
{
    public async Task<EmailVerification> SendVerificationEmailAsync(User user)
    {
        var verificationId = Guid.NewGuid();
        await fluentEmail
            .To(user.Email)
            .Subject("Email verification for Congrapp")
            .Body($"To verify your email address <a href=\"{config["ApplicationUrl"]}api/auth/verify?token={verificationId.ToString()}\">click here</a>.", isHtml: true)
            .SendAsync();

        var emailVerification = new EmailVerification
        {
            Id = verificationId,
            UserId = user.Id
        };
        return emailVerification;
    }
}