using Microsoft.Azure.Documents;

namespace CosmosDbSampleApp
{
    public class PersonModel : CosmosDbModel<PersonModel>
    {
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }

		public static explicit operator PersonModel(Document doc)
		{
			return new PersonModel
			{
				Name = doc.GetPropertyValue<string>(nameof(Name)),
				Age = doc.GetPropertyValue<int>(nameof(Age)),
				Id = doc.GetPropertyValue<string>(nameof(Id).ToLower())
			};
		}
    }
}
