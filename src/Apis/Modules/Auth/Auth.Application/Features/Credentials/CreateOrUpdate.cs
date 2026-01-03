namespace Auth.Application.Features.Credentials;

public class CreateOrUpdate
{
    public record Command : IRequest<ApiResult<Result>>
    {
        public string Key { get; init; }
        public string Value { get; init; }
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
            RuleFor(v => v.Key)
                .NotNull()
                .NotEmpty();
        }
    }

    public class Handler(IAppContext context) : IRequestHandler<Command, ApiResult<Result>>
    {
        public async Task<ApiResult<Result>> Handle(Command command, CancellationToken cancellationToken)
        {
            var item = await context.Credentials
                .Where(c => c.Key.ToLower() == command.Key.ToLower())
                .FirstOrDefaultAsync(cancellationToken);

            if (item == null)
            {
                item = new Credential
                {
                    Id = IDGenerator.GenerateId(),
                    Key = command.Key
                };
                context.Credentials.Add(item);
            }

            item.Value = command.Value ?? string.Empty;

            await context.SaveChangesAsync(cancellationToken);

            return new SuccessResult<Result>(new Result
            {
                Id = item.Id
            });
        }
    }
}