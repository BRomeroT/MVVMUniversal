namespace Codeland.Core.MVVM
{
    public class ErrorInfo : ObservableObject
    {
        private string propertyName;
        public string PropertyName { get => propertyName; set => Set(ref propertyName, value); }

        private string errorMessage;
        public string ErrorMessage { get => errorMessage; set => Set(ref errorMessage, value); }
    }
}
