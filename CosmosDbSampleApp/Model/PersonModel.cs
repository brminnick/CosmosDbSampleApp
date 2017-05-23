using System;

namespace CosmosDbSampleApp
{
    public class PersonModel : DocumentDbModel<PersonModel>
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
    }
}
