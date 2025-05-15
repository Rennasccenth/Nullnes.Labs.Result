using Nullnes.Labs.Result.Abstractions;

namespace Nullnes.Labs.Result.Errors;

public abstract record BaseError : IError
{
    public abstract string Type { get; }
    public abstract string Message { get; }

    public static implicit operator Task<BaseError>(BaseError error) => Task.FromResult(error);

    // Validation
    public static ValidationError ValidationError(string propertyName, string errorMessage) 
        => new(propertyName, errorMessage);
    public static ValidationError ValidationError(ValidationError.ValidationErrorItem[] errorItems) 
        => new(errorItems);
    public static ValidationError ValidationError(IEnumerable<ValidationError.ValidationErrorItem> errorItems) 
        => new([..errorItems]);
    public static ValidationError ValidationError(params List<ValidationError.ValidationErrorItem> errorItems) 
        => new(errorItems);

    // Duplicated Resource
    public static DuplicatedResourceError DuplicatedResourceError(ResourceName resourceName)
        => new(resourceName);
    public static DuplicatedResourceError DuplicatedResourceError(ResourceName resourceName, ResourceIdentifier resourceIdentifier)
        => new(resourceName, resourceIdentifier);

    // Not Found
    public static NotFoundError NotFoundError(string message)
        => new(message);
    public static NotFoundError NotFoundError(ResourceName resourceName)
        => new((string)resourceName);
    public static NotFoundError NotFoundError(ResourceName resourceName, ResourceIdentifier resourceIdentifier)
        => new(resourceName, resourceIdentifier);

    // Permission Denied
    public static PermissionDeniedError PermissionDeniedError() => new();
    public static PermissionDeniedError PermissionDeniedError(string reason) => new(reason);
    public static PermissionDeniedError PermissionDeniedError(IEnumerable<string> reasons) => new(reasons);

    // Invalid Argument
    public static InvalidArgumentError InvalidArgumentError(string argumentName, string? reason = null) 
        => new(argumentName,reason);

    // Missing Pré Condition
    public static MissingPreConditionError MissingPreConditionError(params string[] missingSteps) => new(missingSteps);

    // Unprocessable
    public static UnprocessableError UnprocessableError() => new();
}

public readonly struct ResourceName
{
    private string Value { get; }

    private ResourceName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentNullException(nameof(value));
        }

        Value = value.ToLowerInvariant();
    }

    public static implicit operator string(ResourceName resourceName) => resourceName.Value;
    public static implicit operator ResourceName(string value) => FromString(value);

    private static ResourceName FromString(string value) => new(value);
}

public readonly struct ResourceIdentifier
{
    private string Value { get; }

    private ResourceIdentifier(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentNullException(nameof(value));
        }

        Value = value.ToUpperInvariant();
    }

    public static implicit operator string(ResourceIdentifier resourceName) => resourceName.Value;
    public static implicit operator ResourceIdentifier(string value) => new(value);
}