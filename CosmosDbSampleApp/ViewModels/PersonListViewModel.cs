using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using AsyncAwaitBestPractices;
using AsyncAwaitBestPractices.MVVM;

namespace CosmosDbSampleApp
{
    class PersonListViewModel : BaseViewModel
    {
        readonly WeakEventManager<string> _errorTriggeredEventManager = new();

        bool _isDeletingPerson, _isRefreshing;

        public PersonListViewModel()
        {
            //Ensure Observable Collection is Thread Safe https://codetraveler.io/2019/09/11/using-observablecollection-in-a-multi-threaded-xamarin-forms-application/
            Xamarin.Forms.BindingBase.EnableCollectionSynchronization(PersonList, null, ObservableCollectionCallback);

            PullToRefreshCommand = new AsyncCommand(UpdatePersonList);
        }

        public event EventHandler<string> ErrorTriggered
        {
            add => _errorTriggeredEventManager.AddEventHandler(value);
            remove => _errorTriggeredEventManager.RemoveEventHandler(value);
        }

        public ICommand PullToRefreshCommand { get; }

        public ObservableCollection<PersonModel> PersonList { get; } = new();

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
            PersonList.Clear();

            try
            {
                await foreach (var personModel in DocumentDbService.GetAll<PersonModel>())
                {
                    PersonList.Add(personModel);
                }
            }
            catch (Exception e)
            {
                OnErrorTriggered(e.InnerException is null ? e.Message : e.InnerException.Message);
                DebugService.PrintException(e);
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        //Ensure Observable Collection is Thread Safe https://codetraveler.io/2019/09/11/using-observablecollection-in-a-multi-threaded-xamarin-forms-application/
        void ObservableCollectionCallback(IEnumerable collection, object context, Action? accessMethod, bool writeAccess)
        {
            lock (collection)
            {
                accessMethod?.Invoke();
            }
        }

        void OnErrorTriggered(in string message) => _errorTriggeredEventManager.RaiseEvent(this, message, nameof(ErrorTriggered));
    }
}
