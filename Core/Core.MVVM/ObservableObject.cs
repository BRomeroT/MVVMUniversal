using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Codeland.Core.MVVM
{
    /// <summary>
    /// Object with property changes notifications
    /// </summary>
    public abstract class ObservableObject : INotifyPropertyChanged, IDisposable, INotifyDataErrorInfo
    {
        /// <summary>
        /// Fires when any observable property changes
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Sets property value and notify changes
        /// </summary>
        /// <typeparam name="T">Property type</typeparam>
        /// <param name="field">Internal variable for storage</param>
        /// <param name="newValue">New value</param>
        /// <param name="propertyName">Property name</param>
        protected void Set<T>(ref T field, T newValue = default!, [CallerMemberName] string? propertyName = null)
        {
            field = newValue;
            Validate(field, propertyName);
            RaisePropertyChanged(propertyName);
        }

        /// <summary>
        /// Sends notification for property changes
        /// </summary>
        /// <param name="propertyName">Property name</param>
        /// <remarks>Preferably use nameof</remarks>
        public virtual void RaisePropertyChanged([CallerMemberName] string? propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        protected virtual void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpression) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(((propertyExpression.Body as MemberExpression)?.Member as PropertyInfo)?.Name));

        #region INotifyDataErrorInfo

        private ObservableCollection<ErrorInfo>? messagesErrors;
        public ObservableCollection<ErrorInfo>? MessagesErrors { get => messagesErrors; set => Set(ref messagesErrors, value); }

        public string ValidationSumary
        {
            get
            {
                var res = new System.Text.StringBuilder();
                if (HasErrors && MessagesErrors != null)
                    foreach (var errorInfo in MessagesErrors)
                        res.Append($"{errorInfo.ErrorMessage}{Environment.NewLine}");
                return res.ToString();
            }
        }

        protected readonly Dictionary<string, ObservableCollection<ValidationResult>> Errors = new Dictionary<string, ObservableCollection<ValidationResult>>();

        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        public bool HasErrors => Errors.Count > 0 || (MessagesErrors != null && MessagesErrors.Count > 0);

        public IEnumerable GetErrors([CallerMemberName] string? propertyName = "")
        {
            return propertyName != null && Errors.ContainsKey(propertyName)
                ? Errors[propertyName]
                : new ObservableCollection<ValidationResult>();
        }

        public bool Validate(object? value, [CallerMemberName] string propertyName = "", bool notifyError = true)
        {
            var results = new ObservableCollection<ValidationResult>();
            var context = new ValidationContext(this, null, null) { MemberName = propertyName };
            Validator.TryValidateProperty(value, context, results);

            if (notifyError)
            {
                MessagesErrors ??= new ObservableCollection<ErrorInfo>();

                var erroresEmptys = MessagesErrors.Where(e => string.IsNullOrEmpty(e.PropertyName)).ToList();
                foreach (var error in erroresEmptys)
                {
                    MessagesErrors.Remove(error);
                }

                if (Errors.ContainsKey(propertyName))
                {
                    Errors.Remove(propertyName);
                    var messages = MessagesErrors.Where(m => m.PropertyName == propertyName).ToList();
                    foreach (var message in messages)
                    {
                        MessagesErrors.Remove(message);
                    }
                    NotifyErrorChange(propertyName);
                }

                if (results.Count > 0)
                {
                    if (!Errors.ContainsKey(propertyName))
                    {
                        Errors.Add(propertyName, new ObservableCollection<ValidationResult>());
                    }
                    foreach (var result in results)
                    {
                        Errors[propertyName].Add(new ValidationResult(result.ErrorMessage));
                        var existingError = MessagesErrors.Where(m => m.PropertyName == propertyName).FirstOrDefault();
                        if (existingError == null)
                            MessagesErrors.Add(new ErrorInfo() { PropertyName = propertyName, ErrorMessage = result.ErrorMessage });
                    }
                    NotifyErrorChange(propertyName);
                }
            }

            return results.Count == 0;
        }

        private void NotifyErrorChange(string propertyName)
        {
            RaisePropertyChanged(nameof(ValidationSumary));
            RaisePropertyChanged(nameof(HasErrors));
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        protected bool Validate(object valueObject, bool notifyErrors = true)
        {

            var results = new ObservableCollection<ValidationResult>();

            var context = new ValidationContext(this, null, null);
            Validator.TryValidateObject(valueObject, context, results);

            if (notifyErrors)
            {
                MessagesErrors = new ObservableCollection<ErrorInfo>();

                if (results.Count > 0)
                {
                    foreach (var result in results)
                    {
                        MessagesErrors.Add(new ErrorInfo() { PropertyName = string.Empty, ErrorMessage = result.ErrorMessage ?? string.Empty });
                    }
                    NotifyErrorChange(string.Empty);
                }
            }
            return results.Count == 0;
        }

        public virtual bool Validate() => true;
        #endregion

        #region IDisposable
        bool disposed = false;
        /// <summary>
        /// Delegate thar most explicit release resources usage.
        /// </summary>
        protected Action? DisposeResources;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                    DisposeResources?.Invoke();
                disposed = true;
            }
        }


        #endregion
    }
}
