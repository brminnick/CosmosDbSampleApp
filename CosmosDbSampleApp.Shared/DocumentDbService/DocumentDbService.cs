using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.Azure.Documents.Client;

using Xamarin.Forms;

using CosmosDbSampleApp.Shared;

[assembly: Dependency(typeof(DocumentDbService))]
namespace CosmosDbSampleApp.Shared
{
    public class DocumentDbService : IDocumentDbService
    {
		readonly DocumentClient _client = new DocumentClient(new Uri(DocumentDbConstants.Url),
												DocumentDbConstants.ReadOnlyPrimaryKey);

        public async Task<List<PersonModel>> GetAllPersonModels()
        {
            return await Task.Run(() =>
            {
                return _client.CreateDocumentQuery<PersonModel>(
                    UriFactory.CreateDocumentCollectionUri(PersonModel.DatabaseId, PersonModel.CollectionId))
                                  .ToList();
            });
        }

        public async Task<PersonModel> GetPersonModel(string id)
        {
			return await Task.Run(() =>
			{
                return _client.CreateDocumentQuery<PersonModel>(
                    UriFactory.CreateDocumentCollectionUri(PersonModel.DatabaseId, PersonModel.CollectionId))
                             .Where(x => x.Id.Equals(id))
                             .FirstOrDefault();
			});
        }
    }
}
