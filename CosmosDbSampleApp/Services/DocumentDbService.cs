using System;
using System.Net;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

using Xamarin.Forms;

namespace CosmosDbSampleApp
{
    public static class DocumentDbService
    {
        #region Constant Fields
        static readonly DocumentClient _readonlyClient = new DocumentClient(new Uri(DocumentDbConstants.Url), DocumentDbConstants.ReadOnlyPrimaryKey);
        static readonly Uri _documentCollectionUri = UriFactory.CreateDocumentCollectionUri(PersonModel.DatabaseId, PersonModel.CollectionId);
        #endregion

        #region Fields
        static int _networkIndicatorCount = 0;
        #endregion

        #region Methods
        public static async Task<List<T>> GetAll<T>() where T : CosmosDbModel<T>
        {
            SetActivityIndicatorStatus(true);

            try
            {
                return await Task.Run(() => _readonlyClient.CreateDocumentQuery<T>(_documentCollectionUri).Where(x => x.TypeName.Equals(typeof(T).Name))?.ToList()).ConfigureAwait(false);
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

                var result = await _readonlyClient.ReadDocumentAsync<T>(CreateDocumentUri(id)).ConfigureAwait(false);

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
                var documentClient = GetReadWriteDocumentClient();

                return await documentClient?.ReplaceDocumentAsync(CreateDocumentUri(document.Id), document);
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
                var documentClient = GetReadWriteDocumentClient();

                return await documentClient?.CreateDocumentAsync(_documentCollectionUri, document);
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
