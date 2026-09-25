using FluentValidation;

namespace Application.Events.Commands;

public sealed class CreateEventCommandValidator : AbstractValidator<CreateEventCommand>
{
    public CreateEventCommandValidator()
    {
        RuleFor(command => command.CategoryId)
            .GreaterThan(0);
        RuleFor(command => command.Title)
            .NotEmpty()
            .MaximumLength(200);
        RuleFor(command => command.Description)
            .NotEmpty()
            .MaximumLength(4000);
        RuleFor(command => command.Location)
            .NotEmpty()
            .MaximumLength(300);
        RuleFor(command => command.StartsAt)
            .NotEmpty();
        RuleFor(command => command.TotalSeats)
            .GreaterThan(0);
        RuleFor(command => command.OrganizerId)
            .NotEmpty();
    }
}