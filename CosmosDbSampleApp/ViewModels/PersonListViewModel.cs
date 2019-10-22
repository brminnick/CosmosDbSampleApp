using System;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Collections.Generic;
using AsyncAwaitBestPractices.MVVM;
using AsyncAwaitBestPractices;

namespace CosmosDbSampleApp
{
    public class PersonListViewModel : BaseViewModel
    {
        readonly WeakEventManager<string> _errorTriggeredEventManager = new WeakEventManager<string>();

        bool _isDeletingPerson, _isRefreshing;
        IList<PersonModel> _personList;
        ICommand _pullToRefreshCommand;

        public event EventHandler<string> ErrorTriggered
        {
            add => _errorTriggeredEventManager.AddEventHandler(value);
            remove => _errorTriggeredEventManager.RemoveEventHandler(value);
        }

        public ICommand PullToRefreshCommand => _pullToRefreshCommand ??= new AsyncCommand(UpdatePersonList);

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

        async Task UpdatePersonList()
        {
            IsRefreshing = true;

            try
            {
                PersonList = await DocumentDbService.GetAll<PersonModel>().ConfigureAwait(false);
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

        void OnErrorTriggered(in string message) => _errorTriggeredEventManager.HandleEvent(this, message, nameof(ErrorTriggered));
    }
}
