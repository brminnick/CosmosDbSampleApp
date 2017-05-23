namespace CosmosDbSampleApp
{
    public abstract class DocumentDbModel<T> where T : DocumentDbModel<T>
    {
        public static string CollectionId => typeof(T).Name;
        public static string DatabaseId => "CosmosDbSampleAppDatabase";
    }
}
