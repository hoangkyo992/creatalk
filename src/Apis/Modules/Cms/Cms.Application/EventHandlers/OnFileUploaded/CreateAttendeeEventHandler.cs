using Cdn.Application.Shared.Events;

namespace Cms.Application.EventHandlers.OnFileUploaded;

internal class CreateAttendeeEventHandler(IServiceProvider serviceProvider) : INotificationHandler<OnFileUploadedEvent>
{
    public async Task Handle(OnFileUploadedEvent notification, CancellationToken cancellationToken)
    {
        if (notification.FileIds?.Any() != true)
            return;

        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IAppContext>();
        var cdnService = scope.ServiceProvider.GetRequiredService<ICdnService>();

        foreach (var fileId in notification.FileIds)
        {
            var file = await cdnService.GetFileAsync(fileId, cancellationToken);
            if (file == null)
                continue;

            var fileName = file.Name[..^file.Extension.Length];
            var (name, phone, email) = GetAttendeeData(fileName);
            if (string.IsNullOrWhiteSpace(name))
                continue;

            var attendee = await context.Attendees
                .Where(c => c.FullName.ToLower() == name.ToLower())
                .Where(c => c.Email.ToLower() == email.ToLower())
                .Where(c => c.PhoneNumber.ToLower() == phone.ToLower())
                .FirstOrDefaultAsync(cancellationToken);
            if (attendee is not null) continue;

            var (firstName, lastName) = ParseName(name);

            attendee = new Attendee
            {
                CreatedBy = notification.CurrentUser,
                CreatedTime = DateTime.UtcNow,
                StatusId = 0, //New
                Email = email,
                FullName = name,
                FirstName = firstName,
                LastName = lastName,
                PhoneNumber = phone,
                TicketId = fileId,
                TicketZone = notification.FolderName
            };
            context.Attendees.Add(attendee);
        }
        await context.SaveChangesAsync(cancellationToken);
    }

    private static (string Name, string PhoneNumber, string Email) GetAttendeeData(string file)
    {
        var arr = file.Split('_', StringSplitOptions.RemoveEmptyEntries).ToArray();
        if (arr.Length != 3)
            return ("", "", "");
        return (arr[0], arr[1], arr[2]);
    }

    private static (string FirstName, string LastName) ParseName(string fullName)
    {
        var arr = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries).Reverse().ToArray();
        if (arr.Length > 1)
        {
            return (arr[0], string.Join(" ", arr.Skip(1).Reverse()));
        }
        return (fullName, string.Empty);
    }
}