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
            .Subject("Подтвердите электронную почту [Congrapp]")
            .Body($"<h3>Для подтверждения почты <a href=\"{config["ApplicationUrl"]}api/auth/verify?token={verificationId.ToString()}\">нажмите здесь</a>.</h3>", isHtml: true)
            .SendAsync();

        var emailVerification = new EmailVerification
        {
            Id = verificationId,
            UserId = user.Id
        };
        return emailVerification;
    }
}