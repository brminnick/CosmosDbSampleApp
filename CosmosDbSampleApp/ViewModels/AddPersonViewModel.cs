using System;
using System.Windows.Input;
using System.Threading.Tasks;

using AsyncAwaitBestPractices.MVVM;

namespace CosmosDbSampleApp
{
    public class AddPersonViewModel : BaseViewModel
    {
        #region Fields
        ICommand _saveButtonCommand;
        string _ageEntryText, _nameEntryText;
        bool _isBusy;
        #endregion

        #region Events
        public event EventHandler<string> SaveErrored;
        public event EventHandler SaveCompleted;
        #endregion

        #region Properties
        public ICommand SaveButtonCommand => _saveButtonCommand ??
            (_saveButtonCommand = new AsyncCommand(ExecuteSaveButtonCommand, false));

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
        #endregion

        #region Methods
        async Task ExecuteSaveButtonCommand()
        {
            var ageParseSucceeded = int.TryParse(AgeEntryText, out var age);
            if (!ageParseSucceeded)
            {
                OnSaveErrorred("Age Must Be A Whole Number");
                return;
            }

            var person = new PersonModel
            {
                Name = NameEntryText,
                Age = age
            };

            IsBusy = true;

            try
            {
                var result = await DocumentDbService.Create(person).ConfigureAwait(false);

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

        void OnSaveCompleted() => SaveCompleted?.Invoke(this, EventArgs.Empty);
        void OnSaveErrorred(string message) => SaveErrored?.Invoke(this, message);
        #endregion
    }
}
