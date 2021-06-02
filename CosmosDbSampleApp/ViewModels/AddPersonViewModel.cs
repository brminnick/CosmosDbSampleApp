using System;
using System.Windows.Input;
using System.Threading.Tasks;
using AsyncAwaitBestPractices;
using AsyncAwaitBestPractices.MVVM;

namespace CosmosDbSampleApp
{
    public class AddPersonViewModel : BaseViewModel
    {
        readonly WeakEventManager _saveCompletedEventManager = new();
        readonly WeakEventManager<string> _saveErroredEventManager = new();

        bool _isBusy;

        string _ageEntryText = string.Empty,
                _nameEntryText = string.Empty;

        public AddPersonViewModel()
        {
            SaveButtonCommand = new AsyncCommand(() => ExecuteSaveButtonCommand(AgeEntryText, NameEntryText));
        }

        public event EventHandler<string> SaveErrored
        {
            add => _saveErroredEventManager.AddEventHandler(value);
            remove => _saveErroredEventManager.RemoveEventHandler(value);
        }

        public event EventHandler SaveCompleted
        {
            add => _saveCompletedEventManager.AddEventHandler(value);
            remove => _saveCompletedEventManager.RemoveEventHandler(value);
        }

        public ICommand SaveButtonCommand { get; }

        public string AgeEntryText
        {
            get => _ageEntryText;
            set => SetProperty(ref _ageEntryText, value);
        }

        public string NameEntryText
        {
            get => _nameEntryText;
            set => SetProperty(ref _nameEntryText, value);
        }

        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        async Task ExecuteSaveButtonCommand(string ageEntryText, string nameEntryText)
        {
            var ageParseSucceeded = int.TryParse(ageEntryText, out var age);
            if (!ageParseSucceeded)
            {
                OnSaveErrorred("Age Must Be A Whole Number");
                return;
            }

            var person = new PersonModel
            {
                Name = nameEntryText,
                Age = age
            };

            IsBusy = true;

            try
            {
                var result = await DocumentDbService.UpsertItem(person).ConfigureAwait(false);

                if (result is null)
                    OnSaveErrorred("Save Failed");
                else
                    OnSaveCompleted();
            }
            catch (Exception e)
            {
                OnSaveErrorred(e.Message);
                DebugService.PrintException(e);
            }
            finally
            {
                IsBusy = false;
            }
        }

        void OnSaveCompleted() => _saveCompletedEventManager.RaiseEvent(this, EventArgs.Empty, nameof(SaveCompleted));
        void OnSaveErrorred(in string message) => _saveErroredEventManager.RaiseEvent(this, message, nameof(SaveErrored));
    }
}
