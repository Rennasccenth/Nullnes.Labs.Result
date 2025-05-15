namespace Nullnes.Labs.Result.Errors;

public sealed record ValidationError : BaseError
{
    public override string Type => "Validation Error";
    public override string Message => "One or more validation errors has occured.";
    public IReadOnlyList<ValidationErrorItem> Errors { get; }

    internal ValidationError(string propertyName, string errorMessage)
    {
        Errors = [new ValidationErrorItem(propertyName, errorMessage)];
    }
    internal ValidationError(ValidationErrorItem validationErrorItem)
    {
        Errors = [validationErrorItem];
    }
    internal ValidationError(params IEnumerable<ValidationErrorItem> validationErrorItems)
    {
        Errors = [..validationErrorItems];
    }
    internal ValidationError(List<ValidationErrorItem> validationErrorItems)
    {
        Errors = validationErrorItems;
    }

    public static implicit operator ValidationError((string propertyName, string errorMessage) err) => FromIEnumerable(new ValidationErrorItem(err.propertyName, err.errorMessage));
    public static implicit operator ValidationError(Dictionary<string, IEnumerable<string>> dictionary) => FromIEnumerable(ValidationErrorItem.FromDictionary(dictionary));
    public static implicit operator ValidationError(ValidationErrorItem errorItem) => FromErrorItem(errorItem);
    public static implicit operator ValidationError(ValidationErrorItem[] errorItems) => FromErrorItemArray(errorItems);
    public static implicit operator ValidationError(List<ValidationErrorItem> errorItems) => FromErrorItemList(errorItems);

    private static ValidationError FromIEnumerable(params IEnumerable<ValidationErrorItem> errorItems) => new(errorItems);
    private static ValidationError FromErrorItem(ValidationErrorItem errorItem) => FromIEnumerable(errorItem);
    private static ValidationError FromErrorItemArray(ValidationErrorItem[] errorItems) => FromIEnumerable(errorItems);
    private static ValidationError FromErrorItemList(List<ValidationErrorItem> errorItems) => new(errorItems);

    public Dictionary<string, string[]> ToErrorsDictionary()
    {
        return Errors
            .GroupBy(err => err.Property)
            .Select(group => new
            {
                PropertyName = group.Key,
                Errors = group.Select(error => error.ErrorMessage)
            })
            .ToDictionary(x => 
                x.PropertyName, 
                x => x.Errors.ToArray());
    }

    public sealed record ValidationErrorItem
    {
        public string Property { get; }
        public string ErrorMessage { get; }

        public ValidationErrorItem(string property, string errorMessage)
        {
            Property = property;
            ErrorMessage = errorMessage;
        }

        public static IEnumerable<ValidationErrorItem> FromDictionary(Dictionary<string, IEnumerable<string>> dictionary)
        {
            return dictionary.SelectMany(keyValuePair => keyValuePair.Value
                    .Select(errorMessage => new ValidationErrorItem(keyValuePair.Key, errorMessage)));
        }
    }
}
