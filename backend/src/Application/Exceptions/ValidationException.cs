namespace Application.Exceptions;

public class ValidationException : Exception
{
    public IReadOnlyDictionary<string, string> Errors { get; }

    public ValidationException(IDictionary<string, string> errors) : base("Validering fejlede.")
    {
        Errors = new Dictionary<string, string>(errors);
    }

}
