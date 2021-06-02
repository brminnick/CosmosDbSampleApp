using System;
using System.Net;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Xamarin.Forms;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Xamarin.Essentials;

namespace CosmosDbSampleApp
{
    public static class DocumentDbService
    {
        static readonly Container _readOnlyContainer = new CosmosClient(DocumentDbConstants.Url, DocumentDbConstants.ReadOnlyPrimaryKey).GetContainer(PersonModel.DatabaseId, PersonModel.CollectionId);
        static readonly Container _readWriteContainer = DocumentDbConstants.ReadWritePrimaryKey == "Add Read Write Primary Key"
                                                        ? throw new DocumentDbException("Invalid ReadWrite Primary Key")
                                                        : new CosmosClient(DocumentDbConstants.Url, DocumentDbConstants.ReadWritePrimaryKey).GetContainer(PersonModel.DatabaseId, PersonModel.CollectionId);

        static int _networkIndicatorCount;

        public static async IAsyncEnumerable<T> GetAll<T>() where T : CosmosDbModel<T>
        {
            await SetActivityIndicatorStatus(true).ConfigureAwait(false);

            try
            {

                var queryable = _readOnlyContainer.GetItemLinqQueryable<T>().Where(x => x.TypeName.Equals(typeof(T).Name));
                var feedIterator = queryable.ToFeedIterator();

                if (feedIterator.HasMoreResults)
                {
                    var responseList = await feedIterator.ReadNextAsync().ConfigureAwait(false);

                    foreach (var response in responseList)
                        yield return response;
                }
            }
            finally
            {
                await SetActivityIndicatorStatus(false).ConfigureAwait(false);
            }
        }

        public static async Task<ItemResponse<T>> UpsertItem<T>(T document) where T : CosmosDbModel<T>
        {
            await SetActivityIndicatorStatus(true).ConfigureAwait(false);

            try
            {
                return await _readWriteContainer.UpsertItemAsync(document).ConfigureAwait(false);
            }
            finally
            {
                await SetActivityIndicatorStatus(false).ConfigureAwait(false);
            }
        }

        public static async Task Delete<T>(string id) where T : CosmosDbModel<T>
        {
            await SetActivityIndicatorStatus(true).ConfigureAwait(false);

            try
            {
                var result = await _readWriteContainer.DeleteItemAsync<T>(id, PartitionKey.Null);

                if (!IsSuccessStatusCode(result.StatusCode))
                    throw new Exception($"Delete Failed: {result.StatusCode}");
            }
            finally
            {
                await SetActivityIndicatorStatus(false).ConfigureAwait(false);
            }
        }

        static async Task SetActivityIndicatorStatus(bool isNetworkConnectionActive)
        {
            if (isNetworkConnectionActive)
            {
                _networkIndicatorCount++;
                await MainThread.InvokeOnMainThreadAsync(() => Application.Current.MainPage.IsBusy = true).ConfigureAwait(false);
            }
            else if (--_networkIndicatorCount <= 0)
            {
                _networkIndicatorCount = 0;
                await MainThread.InvokeOnMainThreadAsync(() => Application.Current.MainPage.IsBusy = false).ConfigureAwait(false);
            }
        }

        static bool IsSuccessStatusCode(in HttpStatusCode statusCode) => (int)statusCode >= 200 && (int)statusCode <= 299;

        class DocumentDbException : Exception
        {
            public DocumentDbException(in string message) : base(message)
            {

            }
        }
    }
}
