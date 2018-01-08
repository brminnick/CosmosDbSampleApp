using System;
using System.Windows.Input;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace CosmosDbSampleApp
{
    public class AddPersonViewModel : BaseViewModel
    {
        #region Fields
        ICommand _saveButtonCommand;
        string _ageEntryText, _nameEntryText;
        bool _isActivityIndicatorActive;
        #endregion

        #region Events
        public event EventHandler<string> SaveErrorred;
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

        public bool IsActivityIndicatorActive
        {
            get => _isActivityIndicatorActive;
            set => SetProperty(ref _isActivityIndicatorActive, value);
        }
        #endregion

        #region Methods
        async Task ExecuteSaveButtonCommand()
        {
            var ageParseSucceeded = int.TryParse(AgeEntryText, out var age);
            if (!ageParseSucceeded)
            {
                OnSaveError("Age Must Be A Whole Number");
                return;
            }

            var person = new PersonModel
            {
                Name = NameEntryText,
                Age = age
            };

            IsActivityIndicatorActive = true;

            try
            {
                var result = await DocumentDbService.Create(person);

                if (result != null)
                    OnSaveCompleted();
                else if (DocumentDbConstants.ReadWritePrimaryKey.Equals("Add Read Write Primary Key"))
                    OnSaveError("Invalid DocumentDb Read/Write Key");
                else
                    OnSaveError("Save Failed");
            }
            catch (System.Net.WebException e)
            {
                OnSaveError(e.Message);
                DebugHelpers.PrintException(e);
            }
            catch (Exception e)
            {
                OnSaveError(e.Message);
                DebugHelpers.PrintException(e);
            }
            finally
            {
                IsActivityIndicatorActive = false;
            }
        }

        void OnSaveCompleted() => SaveCompleted?.Invoke(this, EventArgs.Empty);
        void OnSaveError(string message) => SaveErrorred?.Invoke(this, message);
        #endregion
    }
}
