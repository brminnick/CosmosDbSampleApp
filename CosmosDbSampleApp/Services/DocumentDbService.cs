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
                var resultList = new List<T>();

                var queryable = _readonlyClient.CreateDocumentQuery<T>(_documentCollectionUri).Where(x => x.TypeName.Equals(typeof(T).Name)).AsDocumentQuery();

                while (queryable.HasMoreResults)
                {
                    var response = await queryable.ExecuteNextAsync<T>().ConfigureAwait(false);
                    resultList.AddRange(response);
                }

                return resultList;
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
                    return default;

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

        public static async Task Delete(string id)
        {
            SetActivityIndicatorStatus(true);

            try
            {
                var documentClient = GetReadWriteDocumentClient();

                var result = await documentClient.DeleteDocumentAsync(CreateDocumentUri(id));

                if (!IsSuccessStatusCode(result.StatusCode))
                    throw new Exception($"Delete Failed: {result?.StatusCode}");
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
                throw new DocumentDbException("Invalid ReadWrite Primary Key");

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

        static bool IsSuccessStatusCode(HttpStatusCode statusCode) => (int)statusCode >= 200 && (int)statusCode <= 299;
        #endregion
    }

    public class DocumentDbException : Exception
    {
        public DocumentDbException(string message) : base(message)
        {

        }

        public DocumentDbException()
        {

        }
    }
}
