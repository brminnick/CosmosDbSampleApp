using System;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Collections.Generic;

using AsyncAwaitBestPractices.MVVM;

namespace CosmosDbSampleApp
{
    public class PersonListViewModel : BaseViewModel
    {
        #region Fields
        bool _isDeletingPerson, _isRefreshing;
        IList<PersonModel> _personList;
        ICommand _pullToRefreshCommand;
        #endregion

        #region Events
        public event EventHandler<string> ErrorTriggered;
        #endregion

        #region Properties
        public ICommand PullToRefreshCommand => _pullToRefreshCommand ??
            (_pullToRefreshCommand = new AsyncCommand(UpdatePersonList, false));

        public IList<PersonModel> PersonList
        {
            get => _personList;
            set => SetProperty(ref _personList, value);
        }

        public bool IsDeletingPerson
        {
            get => _isDeletingPerson;
            set => SetProperty(ref _isDeletingPerson, value);
        }

        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => SetProperty(ref _isRefreshing, value);
        }
        #endregion

        #region Methods
        async Task UpdatePersonList()
        {
            try
            {
                IsRefreshing = true;
                PersonList = await DocumentDbService.GetAll<PersonModel>();
            }
            catch (Exception e)
            {
                OnErrorTriggered(e.InnerException.Message);
                DebugService.PrintException(e);
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        void OnErrorTriggered(string message) => ErrorTriggered?.Invoke(this, message);
        #endregion
    }
}
