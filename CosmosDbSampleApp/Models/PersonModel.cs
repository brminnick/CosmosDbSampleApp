namespace CosmosDbSampleApp
{
    public class PersonModel : CosmosDbModel<PersonModel>
    {
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
    }
}
