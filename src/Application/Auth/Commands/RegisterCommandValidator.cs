using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Enums;
using FluentValidation;

namespace Application.Auth.Commands;

public sealed class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(command => command.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(320);
        RuleFor(command => command.Password)
            .NotEmpty()
            .MinimumLength(8);
        RuleFor(command => command.FullName)
            .NotEmpty()
            .MaximumLength(200);
        RuleFor(command => command.Role)
            .IsInEnum()
            .Must(role => role is UserRole.Organizer or UserRole.Attendee)
            .WithMessage("Role must be Organizer or Attendee.");
    }
}