namespace Documentify.Domain.Constants
{
    public static class ValidationMessages
    {
        public const string RequiredError = "'{PropertyName}' must not be empty.";
        public const string MaxLengthError = "The length of '{PropertyName}' must be {MaxLength} characters or fewer. You entered {TotalLength} characters.";
        public const string MinLengthError = "The length of '{PropertyName}' must be at least {MinLength} characters. You entered {TotalLength} characters.";
    }
}
