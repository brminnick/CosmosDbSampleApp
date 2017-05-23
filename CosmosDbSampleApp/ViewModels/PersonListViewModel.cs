using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using Xamarin.Forms;

namespace CosmosDbSampleApp
{
    public class PersonListViewModel : BaseViewModel
    {
        #region Fields
        IList<PersonModel> _personList;
        #endregion

        #region Constructors
        public PersonListViewModel() =>
            Task.Run(async () => await UpdatePersonList());
        #endregion

        #region Properties
        public IList<PersonModel> PersonList
        {
            get => _personList;
            set => SetProperty(ref _personList, value);
        }
        #endregion

        #region Methods
        async Task UpdatePersonList()
        {
            try
            {
                PersonList = await DependencyService.Get<IDocumentDbService>().GetAllPersonModels();
            }
            catch (Exception e)
            {
                DebugHelpers.PrintException(e);
            }
        }
        #endregion
    }
}
