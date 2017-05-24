using Newtonsoft.Json;

namespace CosmosDbSampleApp
{
    public abstract class DocumentDbModel<T>  where T : DocumentDbModel<T>
    {
        public static string CollectionId => typeof(T).Name;
        public static string DatabaseId => "CosmosDbSampleAppDatabase";

        [JsonProperty("id")]
        public string Id { get; set; }
    }
}
