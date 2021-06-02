using System.ComponentModel;

namespace CosmosDbSampleApp
{
    public class PersonModel : CosmosDbModel<PersonModel>
    {
        public string Name { get; init; } = string.Empty;
        public int Age { get; init; }
    }
}

namespace System.Runtime.CompilerServices
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public record IsExternalInit;
}