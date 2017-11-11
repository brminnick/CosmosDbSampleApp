using System;
using System.Net;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.Azure.Documents.Client;

namespace CosmosDbSampleApp
{
    public static class DocumentDbService
    {
        #region Constant Fields
        static readonly DocumentClient _readonlyClient = new DocumentClient(new Uri(DocumentDbConstants.Url), DocumentDbConstants.ReadOnlyPrimaryKey);
        static readonly Uri _documentCollectionUri = UriFactory.CreateDocumentCollectionUri(PersonModel.DatabaseId, PersonModel.CollectionId);
        #endregion

        #region Methods
        public static Task<List<PersonModel>> GetAllPersonModels() =>
            Task.Run(() => _readonlyClient.CreateDocumentQuery<PersonModel>(_documentCollectionUri).ToList());

        public static async Task<PersonModel> GetPersonModel(string id)
        {
            var result = await _readonlyClient.ReadDocumentAsync<PersonModel>(CreateDocumentUri(id));

            if (result.StatusCode != HttpStatusCode.Created)
                return null;

            return result;
        }

        public static async Task<PersonModel> CreatePersonModel(PersonModel person)
        {
            var readWriteClient = GetReadWriteDocumentClient();
            if (readWriteClient == null)
                return null;

            var result = await readWriteClient?.CreateDocumentAsync(_documentCollectionUri, person);

            if (result?.StatusCode != HttpStatusCode.Created)
                return null;

            return (PersonModel)result.Resource;
        }

        public static async Task<HttpStatusCode> DeletePersonModel(string id)
        {
            var readWriteClient = GetReadWriteDocumentClient();
            if (readWriteClient == null)
                return default(HttpStatusCode);

            var result = await readWriteClient?.DeleteDocumentAsync(CreateDocumentUri(id));

            return result.StatusCode;
        }

        static Uri CreateDocumentUri(string id) =>
            UriFactory.CreateDocumentUri(PersonModel.DatabaseId, PersonModel.CollectionId, id);

        static DocumentClient GetReadWriteDocumentClient()
        {
            if (DocumentDbConstants.ReadWritePrimaryKey.Equals("Add Read Write Primary Key"))
                return default(DocumentClient);

            return new DocumentClient(new Uri(DocumentDbConstants.Url), DocumentDbConstants.ReadWritePrimaryKey);
        }

        #endregion
    }
}
