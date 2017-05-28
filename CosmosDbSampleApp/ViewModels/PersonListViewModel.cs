using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using Xamarin.Forms;
using System.Windows.Input;

namespace CosmosDbSampleApp
{
    public class PersonListViewModel : BaseViewModel
    {
        #region Fields
        IList<PersonModel> _personList;
        ICommand _pullToRefreshCommand;
        #endregion

        #region Events
        public event EventHandler PullToRefreshCompleted;
        public event EventHandler<string> Error;
        #endregion

        #region Properties
        public ICommand PullToRefreshCommand => _pullToRefreshCommand ??
            (_pullToRefreshCommand = new Command(async () => await ExecutePullToRefreshCommand()));

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
                PersonList = await DocumentDbService.GetAllPersonModels();
            }
            catch (Exception e)
            {
                OnError(e.InnerException.Message);
                DebugHelpers.PrintException(e);
            }
        }

        async Task ExecutePullToRefreshCommand()
        {
            await UpdatePersonList();

            OnPullToRefreshCompleted();
        }

        void OnError(string message) =>
            Error?.Invoke(this, message);

        void OnPullToRefreshCompleted() =>
            PullToRefreshCompleted?.Invoke(this, EventArgs.Empty);
        #endregion
    }
}
