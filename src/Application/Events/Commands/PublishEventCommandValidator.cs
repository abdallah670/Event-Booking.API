using FluentValidation;

namespace Application.Events.Commands;

public sealed class PublishEventCommandValidator : AbstractValidator<PublishEventCommand>
{
    public PublishEventCommandValidator()
    {
        RuleFor(command => command.EventId).NotEmpty();
        RuleFor(command => command.OrganizerId).NotEmpty();
    }
}