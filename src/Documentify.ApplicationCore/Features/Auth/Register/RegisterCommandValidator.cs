using Documentify.Domain.Constants;
using FluentValidation;
using System.Text.RegularExpressions;

namespace Documentify.ApplicationCore.Features.Auth.Register
{
    public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterCommandValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty()
                    .WithMessage(ValidationMessages.RequiredError)
                .MinimumLength(5)
                    .WithMessage(ValidationMessages.MinLengthError)
                .MaximumLength(256)
                    .WithMessage(ValidationMessages.MaxLengthError)
                .Matches("^[a-zA-Z0-9_]{5,256}$")
                    .WithMessage("Username can only contain characters, numbers and underscore.");

            RuleFor(x => x.Email)
                .NotEmpty()
                    .WithMessage(ValidationMessages.RequiredError)
                .EmailAddress()
                    .WithMessage("A valid email is required.")
                .Matches(new Regex("^.+@(gmail|hotmail|outlook|yahoo)\\.com$", RegexOptions.IgnoreCase))
                    .WithMessage("Only gmail, hotmail, outlook, yahoo is allowed");

            RuleFor(x => x.Password)
                .NotEmpty()
                    .WithMessage(ValidationMessages.RequiredError)
                .MinimumLength(8)
                    .WithMessage(ValidationMessages.MinLengthError)
                .MaximumLength(100)
                    .WithMessage(ValidationMessages.MaxLengthError)
                .Matches("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d).+$")
                    .WithMessage("Password must contain at least one lowercase character, one uppercase character and a number");

            // TODO: check email, username uniqueness
        }
    }
}
