using System.Text.RegularExpressions;
using Auth.Application.Common;

namespace Auth.Application.Features.Users;

public class CreateOrUpdate
{
    public record Command : IRequest<ApiResult<Result>>
    {
        [JsonIgnore]
        public long Id { get; set; }
        public string Username { get; init; }
        public string DisplayName { get; init; }
        public string Email { get; init; }
        public string? Phone { get; init; }
        public string? Password { get; init; }
        public UserStatus StatusId { get; init; }

        [JsonConverter(typeof(ZCodeCollectionJsonConverter<IEnumerable<long>>))]
        public IEnumerable<long> RoleIds { get; init; } = [];
    }

    public record Result
    {
        [JsonConverter(typeof(ZCodeJsonConverter))]
        public long Id { get; init; }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(v => v.Username)
                .NotNull()
                .NotEmpty();
            RuleFor(v => v.Email)
                .NotNull()
                .NotEmpty();
            RuleFor(v => v.DisplayName)
                .NotNull()
                .NotEmpty();
        }
    }

    public class Handler(IMediator mediator,
        IAppContext context,
        IMapper mapper,
        IActivityLogService activityLogService,
        ICurrentUser currentUser) : IRequestHandler<Command, ApiResult<Result>>
    {
        public async Task<ApiResult<Result>> Handle(Command command, CancellationToken cancellationToken)
        {
            var regex = new Regex("^[a-zA-Z0-9]+([._@-]?[a-zA-Z0-9]+)*$", RegexOptions.IgnoreCase, TimeSpan.FromSeconds(5));
            if (!regex.IsMatch(command.Username))
                return new FailResult<Result>(ErrorMessages.USER_USERNAME_INVALID, "username", command.Username, HttpStatusCode.Conflict);

            var item = await context.Users
                .Where(c => c.Id != command.Id && c.Username.ToLower() == command.Username.ToLower())
                .FirstOrDefaultAsync(cancellationToken);
            if (item != null)
                return new FailResult<Result>(ErrorMessages.USER_EXISTED, "username", command.Username, HttpStatusCode.Conflict);

            item = await context.Users
                .Where(c => c.Id == command.Id)
                .Include(c => c.Roles)
                .FirstOrDefaultAsync(cancellationToken);
            if (item == null && command.Id > 0)
                return new FailResult<Result>(ErrorMessages.USER_NOT_FOUND, HttpStatusCode.NotFound);

            var oldValue = new object();
            var action = ActivityLogAction.Create;

            ApplicationEvent? evt = null;
            if (item == null)
            {
                activityLogService.Setup(LogLabel.CreateUser, $"User [{command.Username}] is added", currentUser);

                item = new User
                {
                    Id = IDGenerator.GenerateId(),
                    Username = command.Username.ToLower(),
                    CreatedTime = DateTime.UtcNow,
                    CreatedBy = currentUser.Username,
                    PasswordChanged = false,
                    StatusId = UserStatus.Active,
                    Password = PasswordBuilder.Create(command.Password ?? "P@ssw0rd@2024"),
                };
                context.Users.Add(item);

                evt = new UserEvents.OnDataCreatedEvent
                {
                    Ids = [item.Id]
                };
            }
            else
            {
                activityLogService.Setup(LogLabel.UpdateUser, $"User [{item.Username}] is updated", currentUser);
                oldValue = mapper.Map<UserLoggingDto>(item);
                action = ActivityLogAction.Update;

                item.UpdatedTime = DateTime.UtcNow;
                item.UpdatedBy = currentUser.Username;

                evt = new UserEvents.OnDataUpdatedEvent
                {
                    Ids = [item.Id]
                };
            }

            item.DisplayName = command.DisplayName;
            item.Email = command.Email.ToLower();
            item.Phone = command.Phone;
            item.StatusId = command.StatusId;

            item.Roles
                .Where(c => !command.RoleIds.Contains(c.RoleId))
                .ToList().ForEach(c =>
                {
                    c.IsDeleted = true;
                    c.RevokedAt = DateTime.UtcNow;
                });

            foreach (var roleId in command.RoleIds)
            {
                var role = item.Roles.FirstOrDefault(x => x.RoleId == roleId);
                if (role == null)
                {
                    item.Roles.Add(new UserRole
                    {
                        RoleId = roleId,
                        UserId = item.Id
                    });
                }
            }

            var newValue = mapper.Map<UserLoggingDto>(item);
            activityLogService.AddLog(new LogEntityModel(nameof(User), item.Id)
            {
                Action = action,
                OldValue = oldValue,
                NewValue = newValue
            });

            await context.SaveChangesAsync(cancellationToken);
            await mediator.Publish(evt.SetCurrentUser(currentUser), cancellationToken);

            await activityLogService.CommitAsync();

            return new SuccessResult<Result>(new Result
            {
                Id = item.Id
            });
        }
    }
}