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

        #region Fields
        static bool _networkIndicatorCount = 0;
        #endregion

        #region Methods
        public static async Task<List<T>> GetAll<T>() where T : CosmosDbModel<T>
        {
            SetActivityIndicatorStatus(true);

            try
            {
                await Task.Run(() => _readonlyClient.CreateDocumentQuery<PersonModel>(_documentCollectionUri).Where(x => x.TypeName.Equals(typeof(T).Name))?.ToList());
            }
            finally
            {
                SetActivityIndicatorStatus(false);
            }
        }

        public static async Task<T> Get<T>(string id)
        {
            SetActivityIndicatorStatus(true);

            try
            {

                var result = await _readonlyClient.ReadDocumentAsync<T>(CreateDocumentUri(id));

                if (result.StatusCode != HttpStatusCode.Created)
                    return default(T);

                return result;
            }
            finally
            {
                SetActivityIndicatorStatus(false);
            }
        }

        public static async Task<Document> Update<T>(T document) where T : CosmosDbModel<T>
        {
            SetActivityIndicatorStatus(true);

            try
            {
                var documentClient = await GetDocumentClient().ConfigureAwait(false); ;

                return await documentClient?.ReplaceDocumentAsync(CreateDocumentUri(document.CosmosDbId), document);
            }
            finally
            {
                SetActivityIndicatorStatus(false);
            }
        }

        public static async Task<Document> Create<T>(T document) where T : CosmosDbModel<T>
        {
            SetActivityIndicatorStatus(true);

            try
            {
                var documentClient = await GetDocumentClient().ConfigureAwait(false);

                return await documentClient?.CreateDocumentAsync(DocumentCollectionUri, document);
            }
            finally
            {
                SetActivityIndicatorStatus(false);
            }
        }

        public static async Task<HttpStatusCode> Delete(string id)
        {
            SetActivityIndicatorStatus(true);

            try
            {
                var readWriteClient = GetReadWriteDocumentClient();
                if (readWriteClient == null)
                    return default(HttpStatusCode);

                var result = await readWriteClient?.DeleteDocumentAsync(CreateDocumentUri(id));

                return result?.StatusCode ?? throw new HttpRequestException("Delete Failed");
            }
            finally
            {
                SetActivityIndicatorStatus(false);
            }

        }

        static Uri CreateDocumentUri(string id) =>
            UriFactory.CreateDocumentUri(PersonModel.DatabaseId, PersonModel.CollectionId, id);

        static DocumentClient GetReadWriteDocumentClient()
        {
            if (DocumentDbConstants.ReadWritePrimaryKey.Equals("Add Read Write Primary Key"))
                return default(DocumentClient);

            return new DocumentClient(new Uri(DocumentDbConstants.Url), DocumentDbConstants.ReadWritePrimaryKey);
        }

        static void SetActivityIndicatorStatus(bool isNetworkConnectionActive)
        {
            if (isNetworkConnectionActive)
            {
                _networkIndicatorCount++;
                Device.BeginInvokeOnMainThread(() => Application.Current.MainPage.IsBusy = true);
            }
            else if (--_networkIndicatorCount <= 0)
            {
                _networkIndicatorCount = 0;
                Device.BeginInvokeOnMainThread(() => Application.Current.MainPage.IsBusy = false);
            }
        }

        #endregion
    }
}
