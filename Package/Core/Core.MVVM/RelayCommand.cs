using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Sysne.Core.MVVM
{
    [DebuggerStepThrough]
    public class RelayCommand : RelayCommand<object>
    {
        /// <summary>
        /// Constructor de RelayCommand
        /// </summary>
        /// <param name="execute">Función sin parámetros que ejecuta el comando</param>
        /// <param name="canExecute">Función que evalúa si se han cumplido las condiciones para ejecutar el comando</param>
        /// <param name="autoDisable">Deshabilita el comando mientrase se esté ejecutando</param>
        /// <param name="dependencies">Propiedades que debe monitorear sus cambios para notificar que las reglas de ejecución han cambiado</param>
        public RelayCommand(Action execute, Func<bool> canExecute = null, bool autoDisable = true, (INotifyPropertyChanged owner, string[] properties) dependencies = default)
            : base(execute, canExecute, autoDisable, dependencies) { }
        /// <summary>
        /// Constructor de RelayCommand asíncrono
        /// </summary>
        /// <param name="execute">Función asíncrono sin parámetros que ejecuta el comando</param>
        /// <param name="canExecute">Función que evalúa si se han cumplido las condiciones para ejecutar el comando</param>
        /// <param name="autoDisable">Deshabilita el comando mientrase se esté ejecutando</param>
        /// <param name="dependencies">Propiedades que debe monitorear sus cambios para notificar que las reglas de ejecución han cambiado</param>
        public RelayCommand(Func<Task> execute, Func<bool> canExecute = null, bool autoDisable = true, (INotifyPropertyChanged owner, string[] properties) dependencies = default)
            : base(execute, canExecute, autoDisable, dependencies) { }
    }

    [DebuggerStepThrough]
    public class RelayCommand<T> : ObservableObject, ICommand
    {
        #region Constructs
        /// <summary>
        /// Constructor de RelayCommand
        /// </summary>
        /// <param name="execute">Función sin parámetros que ejecuta el comando</param>
        /// <param name="canExecute">Función que evalúa si se han cumplido las condiciones para ejecutar el comando</param>
        /// <param name="autoDisable">Deshabilita el comando mientrase se esté ejecutando</param>
        /// <param name="dependencies">Propiedades que debe monitorear sus cambios para notificar que las reglas de ejecución han cambiado</param>
        public RelayCommand(Action execute, Func<bool> canExecute = null, bool autoDisable = true, (INotifyPropertyChanged owner, string[] properties) dependencies = default)
        {
            Action = execute;
            CanExecuteFunc = canExecute;
            AutoDisableWhenProcessing = autoDisable;
            if (dependencies.owner != null) RaiseCanExecuteDependencies(dependencies.owner, dependencies.properties);
        }
        /// <summary>
        /// Constructor de RelayCommand asíncrono
        /// </summary>
        /// <param name="execute">Función asíncrona sin parámetros que ejecuta el comando</param>
        /// <param name="canExecute">Función que evalúa si se han cumplido las condiciones para ejecutar el comando</param>
        /// <param name="autoDisable">Deshabilita el comando mientrase se esté ejecutando</param>
        /// <param name="dependencies">Propiedades que debe monitorear sus cambios para notificar que las reglas de ejecución han cambiado</param>
        public RelayCommand(Func<Task> execute, Func<bool> canExecute = null, bool autoDisable = true, (INotifyPropertyChanged owner, string[] properties) dependencies = default)
        {
            ActionAsync = execute;
            CanExecuteFunc = canExecute;
            AutoDisableWhenProcessing = autoDisable;
            if (dependencies.owner != null) RaiseCanExecuteDependencies(dependencies.owner, dependencies.properties);
        }
        /// <summary>
        /// Constructor de RelayCommand con parámetros
        /// </summary>
        /// <param name="execute">Función con parámetros que ejecuta el comando</param>
        /// <param name="canExecute">Función que evalúa si se han cumplido las condiciones para ejecutar el comando</param>
        /// <param name="autoDisable">Deshabilita el comando mientrase se esté ejecutando</param>
        /// <param name="dependencies">Propiedades que debe monitorear sus cambios para notificar que las reglas de ejecución han cambiado</param>
        public RelayCommand(Action<T> execute, Func<T, bool> canExecute = null, bool autoDisable = true, (INotifyPropertyChanged owner, string[] properties) dependencies = default)
        {
            ActionGeneric = execute;
            CanExecuteFuncGeneric = canExecute;
            AutoDisableWhenProcessing = autoDisable;
            if (dependencies.owner != null) RaiseCanExecuteDependencies(dependencies.owner, dependencies.properties);
        }
        /// <summary>
        /// Constructor de RelayCommand asíncrono con parámetros
        /// </summary>
        /// <param name="execute">Función asíncrona con parámetros que ejecuta el comando</param>
        /// <param name="canExecute">Función que evalúa si se han cumplido las condiciones para ejecutar el comando</param>
        /// <param name="autoDisable">Deshabilita el comando mientrase se esté ejecutando</param>
        /// <param name="dependencies">Propiedades que debe monitorear sus cambios para notificar que las reglas de ejecución han cambiado</param>
        public RelayCommand(Func<T, Task> execute, Func<T, bool> canExecute = null, bool autoDisable = true, (INotifyPropertyChanged owner, string[] properties) dependencies = default)
        {
            ActionGenericAsync = execute;
            CanExecuteFuncGeneric = canExecute;
            AutoDisableWhenProcessing = autoDisable;
            if (dependencies.owner != null) RaiseCanExecuteDependencies(dependencies.owner, dependencies.properties);
        }
        #endregion

        #region Properties
        private Action Action { get; set; }
        private Func<Task> ActionAsync { get; set; }
        private Action<T> ActionGeneric { get; set; }
        private Func<T, Task> ActionGenericAsync { get; set; }

        private Func<bool> CanExecuteFunc { get; set; }
        private Func<T, bool> CanExecuteFuncGeneric { get; set; }

        private bool AutoDisableWhenProcessing { get; set; }

        private bool processing;
        /// <summary>
        /// Indica si está ejecutando el comando
        /// </summary>
        public bool Processing
        {
            get => processing;
            set
            {
                Set(ref processing, value);
                if (AutoDisableWhenProcessing) RaiseCanExecuteChanged();
            }
        }
        /// <summary>
        /// Informa si se han cumplido las reglas de negocio para ejecutar o no el comando
        /// </summary>
        public bool CanExecuteIsEnabled { get => CanExecute(null); }
        #endregion

        #region Methods
        /// <summary>
        /// Notifica que las reglas de negocio han cambiado
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            RaisePropertyChanged(() => CanExecuteIsEnabled);
            CanExecuteChanged?.Invoke(this, new EventArgs());
        }
        /// <summary>
        /// Función sin parámetros que evalúa si se cumplen las condicioens para ejecutar el comando
        /// </summary>
        /// <returns>Si se puede o no ejecutar el comando</returns>
        public bool CanExecute() => CanExecute(null);
        /// <summary>
        /// Ejecuta sin parámetros el comando
        /// </summary>
        public virtual void Execute() => Execute(null);

        public void RaiseCanExecuteDependencies(INotifyPropertyChanged owner, string[] properties)
        {
            owner.PropertyChanged += (s, e) =>
            {
                if (properties.Contains(e.PropertyName)) RaiseCanExecuteChanged();
            };
        }
        #endregion

        #region ICommand
        /// <summary>
        /// Evento que se lanza al cambiar las reglas de negocio
        /// </summary>
        public event EventHandler CanExecuteChanged;
        /// <summary>
        /// Función para evaluar si se cumplen las condiciones para ejecutar el comando
        /// </summary>
        /// <param name="parameter">Parámetro opcional del comando</param>
        /// <returns></returns>
        public bool CanExecute(object parameter)
        {
            if (CanExecuteFuncGeneric != null)
            {
                if (AutoDisableWhenProcessing)
                {
                    if (CanExecuteFuncGeneric == null) return !Processing;
                    return !Processing && CanExecuteFuncGeneric.Invoke((T)parameter);
                }
                else
                {
                    if (CanExecuteFuncGeneric == null) return true;
                    return CanExecuteFuncGeneric.Invoke((T)parameter);
                }
            }

            if (AutoDisableWhenProcessing)
            {
                if (CanExecuteFunc == null) return !Processing;
                return !Processing && CanExecuteFunc.Invoke();
            }
            else
            {
                if (CanExecuteFunc == null) return true;
                return CanExecuteFunc.Invoke();
            }
        }
        /// <summary>
        /// Ejecuta el comando
        /// </summary>
        /// <param name="parameter">Parámetro opcional del comando</param>
        public virtual async void Execute(object parameter)=>
            await ExecuteAsync(parameter);

        public virtual async Task ExecuteAsync(object parameter = null)
        {
            Processing = true;
            Action?.Invoke();
            ActionGeneric?.Invoke((T)parameter);
            if (ActionAsync != null) await ActionAsync.Invoke();
            if (ActionGenericAsync != null) await ActionGenericAsync.Invoke((T)parameter);
            Processing = false;
        }
        #endregion
    }
}