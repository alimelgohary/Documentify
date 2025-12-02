using Documentify.Domain.Entities;
using Documentify.Domain.Constants;
using FluentValidation;

namespace Documentify.ApplicationCore.Features.Categories.Add
{
    public class AddCategoryCommandValidator : AbstractValidator<AddCategoryCommand>
    {
        public AddCategoryCommandValidator()
        {
            RuleFor(x=>x.categoryName)
                .NotNull()
                .NotEmpty()
                    .WithMessage(ValidationMessages.RequiredError)
                .MinimumLength(Category.ValidationConstants.NameMinLength)
                    .WithMessage(ValidationMessages.MinLengthError)
                .MaximumLength(Category.ValidationConstants.NameMaxLength)
                    .WithMessage(ValidationMessages.MaxLengthError);
        }
    }
}
