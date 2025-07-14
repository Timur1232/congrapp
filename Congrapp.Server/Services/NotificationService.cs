using Congrapp.Server.Data;
using FluentEmail.Core;
using Congrapp.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace Congrapp.Server.Services;

public class NotificationService(IServiceProvider services) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var now = DateTime.Now;
        var notificationTime = now
            .AddHours(12 - now.Hour)
            .AddMinutes(-now.Minute)
            .AddSeconds(-now.Second)
            .AddMilliseconds(-now.Millisecond);
        
        if (notificationTime < now)
        {
            await Notify();
            var awakeTime = notificationTime.AddDays(1);
            var period = awakeTime - DateTime.Now;
            try
            {
                await new PeriodicTimer(period)
                    .WaitForNextTickAsync(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                return;
            }
        }
        
        var timer = new PeriodicTimer(TimeSpan.FromDays(1));
        try
        {
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                await Notify();
            }
        }
        catch (OperationCanceledException) {}
    }

    private async Task Notify()
    {
        using var scope = services.CreateScope();
        var birthdayDbContext = scope.ServiceProvider.GetRequiredService<BirthdayDbContext>();
        var fluentEmail = scope.ServiceProvider.GetRequiredService<IFluentEmail>();
        
        var now = DateOnly.FromDateTime(DateTime.Today);
        var query =
            from record in birthdayDbContext.NotificationRecords
            join birthday in birthdayDbContext.BirthdayInfos on record.BirthdayId equals birthday.Id
            join user in birthdayDbContext.Users on birthday.UserId equals user.Id
            where user.EmailVerified
            select new
            {
                record.Id,
                record.DaysBefore,
                birthday.BirthdayDate,
                birthday.PersonName,
                user.Email,
            };
        var records = await query.ToListAsync();

        foreach (var record in records)
        {
            var upcomingBirthday = record.BirthdayDate.AddYears(now.Year - record.BirthdayDate.Year);
            var daysBefore = upcomingBirthday.DayNumber - now.DayNumber;
            if (daysBefore == record.DaysBefore)
            {
                await fluentEmail
                    .To(record.Email)
                    .Subject($"Скоро День Рождения У {record.PersonName}!")
                    .Body($"<h2>Дней до дня рождения: {record.DaysBefore}</h2><p>Не забудьте подготовиться!</p>", isHtml: true)
                    .SendAsync();
                birthdayDbContext.NotificationRecords.Remove(new NotificationRecord{ Id = record.Id });
            }
        }
        await birthdayDbContext.SaveChangesAsync();
    }
}