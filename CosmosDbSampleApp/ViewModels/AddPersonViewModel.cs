using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace CosmosDbSampleApp
{
    public class AddPersonViewModel : BaseViewModel
    {
        #region Fields
        ICommand _saveButtonCommand;
        string _ageEntryText, _nameEntryText;
        #endregion

        #region Events
        public event EventHandler<string> Error;
        public event EventHandler SaveCompleted;
        #endregion

        #region Properties
        public ICommand SaveButtonCommand => _saveButtonCommand ??
            (_saveButtonCommand = new Command(async () => await ExecuteSaveButtonCommand()));

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
        #endregion

        #region Methods
        async Task ExecuteSaveButtonCommand()
        {
            int.TryParse(AgeEntryText, out var age);
            var person = new PersonModel
            {
                Name = NameEntryText,
                Age = age
            };

            var result = await DocumentDbService.CreatePersonModel(person);

            if (result != null)
                OnSaveCompleted();
            else
                OnError("Save Failed");
        }

        void OnSaveCompleted() =>
            SaveCompleted?.Invoke(this, EventArgs.Empty);

        void OnError(string message) =>
            Error?.Invoke(this, message);
        #endregion
    }
}
