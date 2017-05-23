using System.Threading.Tasks;
using System.Collections.Generic;

namespace CosmosDbSampleApp
{
    public interface IDocumentDbService
    {
        Task<List<PersonModel>> GetAllPersonModels();
        Task<PersonModel> GetPersonModel(string id);
    }
}
