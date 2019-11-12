using System;
using System.Net;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Xamarin.Forms;

namespace CosmosDbSampleApp
{
    public static class DocumentDbService
    {
        static readonly Lazy<DocumentClient> _readonlyClientHolder = new Lazy<DocumentClient>(() => new DocumentClient(new Uri(DocumentDbConstants.Url), DocumentDbConstants.ReadOnlyPrimaryKey));
        static readonly Lazy<DocumentClient> _readWriteClientHolder = new Lazy<DocumentClient>(() =>
        {
            if (DocumentDbConstants.ReadWritePrimaryKey.Equals("Add Read Write Primary Key"))
                throw new DocumentDbException("Invalid ReadWrite Primary Key");

            return new DocumentClient(new Uri(DocumentDbConstants.Url), DocumentDbConstants.ReadWritePrimaryKey);
        });

        static readonly Uri _documentCollectionUri = UriFactory.CreateDocumentCollectionUri(PersonModel.DatabaseId, PersonModel.CollectionId);

        static int _networkIndicatorCount = 0;

        static DocumentClient ReadOnlyClient => _readonlyClientHolder.Value;
        static DocumentClient ReadWriteClient => _readWriteClientHolder.Value;

        public static async IAsyncEnumerable<T> GetAll<T>() where T : CosmosDbModel<T>
        {
            await SetActivityIndicatorStatus(true).ConfigureAwait(false);

            try
            {

                var queryable = ReadOnlyClient.CreateDocumentQuery<T>(_documentCollectionUri).Where(x => x.TypeName.Equals(typeof(T).Name)).AsDocumentQuery();

                while (queryable.HasMoreResults)
                {
                    var responseList = await queryable.ExecuteNextAsync<T>().ConfigureAwait(false);

                    foreach (var response in responseList)
                        yield return response;
                }
            }
            finally
            {
                await SetActivityIndicatorStatus(false).ConfigureAwait(false);
            }
        }

        public static async Task<T> Get<T>(string id)
        {
            await SetActivityIndicatorStatus(true).ConfigureAwait(false);

            try
            {
                var result = await ReadOnlyClient.ReadDocumentAsync<T>(CreateDocumentUri(id)).ConfigureAwait(false);

                if (result.StatusCode != HttpStatusCode.Created)
                    throw new DocumentDbException("Get Failed");

                return result;
            }
            finally
            {
                await SetActivityIndicatorStatus(false).ConfigureAwait(false);
            }
        }

        public static async Task<Document> Update<T>(T document) where T : CosmosDbModel<T>
        {
            await SetActivityIndicatorStatus(true).ConfigureAwait(false);

            try
            {
                return await ReadWriteClient.ReplaceDocumentAsync(CreateDocumentUri(document.Id), document).ConfigureAwait(false);
            }
            finally
            {
                await SetActivityIndicatorStatus(false).ConfigureAwait(false);
            }
        }

        public static async Task<Document> Create<T>(T document) where T : CosmosDbModel<T>
        {
            await SetActivityIndicatorStatus(true).ConfigureAwait(false);

            try
            {
                return await ReadWriteClient.CreateDocumentAsync(_documentCollectionUri, document).ConfigureAwait(false);
            }
            finally
            {
                await SetActivityIndicatorStatus(false).ConfigureAwait(false);
            }
        }

        public static async Task Delete(string id)
        {
            await SetActivityIndicatorStatus(true).ConfigureAwait(false);

            try
            {
                var result = await ReadWriteClient.DeleteDocumentAsync(CreateDocumentUri(id)).ConfigureAwait(false);

                if (!IsSuccessStatusCode(result.StatusCode))
                    throw new Exception($"Delete Failed: {result?.StatusCode}");
            }
            finally
            {
                await SetActivityIndicatorStatus(false).ConfigureAwait(false);
            }
        }

        static Uri CreateDocumentUri(string id) =>
            UriFactory.CreateDocumentUri(PersonModel.DatabaseId, PersonModel.CollectionId, id);

        static Task SetActivityIndicatorStatus(bool isNetworkConnectionActive)
        {
            if (isNetworkConnectionActive)
            {
                _networkIndicatorCount++;
                return Device.InvokeOnMainThreadAsync(() => Application.Current.MainPage.IsBusy = true);
            }

            if (--_networkIndicatorCount <= 0)
            {
                _networkIndicatorCount = 0;
                return Device.InvokeOnMainThreadAsync(() => Application.Current.MainPage.IsBusy = false);
            }

            return Task.CompletedTask;
        }

        static bool IsSuccessStatusCode(in HttpStatusCode statusCode) => (int)statusCode >= 200 && (int)statusCode <= 299;

        class DocumentDbException : Exception
        {
            public DocumentDbException(in string message) : base(message)
            {

            }

            public DocumentDbException()
            {

            }
        }
    }
}
