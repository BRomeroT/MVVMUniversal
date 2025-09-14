using Core.Lib.OS;
using Codeland.Core.MVVM;
using Codeland.Core.OS;
using System;
using System.Runtime.CompilerServices;

namespace Core.Helpers
{
    public class Settings : ObservableObject
    {
        readonly ISettingsStorage settingsStorage;
        private Settings() => settingsStorage = DependencyService.Get<ISettingsStorage>();

        [ThreadStatic]
        static Settings current;
        public static Settings Current => current ??= new Settings();

        private string webAPilUrl;
        public string WebAPIUrl { get => GetValue<string>(); set => SetValue(ref webAPilUrl, value); }

        void SetValue<T>(ref T field, T newValue = default, [CallerMemberName] string propertyName = null)
        {
            settingsStorage.SetValue(newValue, propertyName);
            Set(ref field, newValue, propertyName);
        }

        T GetValue<T>([CallerMemberName] string propertyName = null) =>
            settingsStorage.GetValue<T>(propertyName);
    }
}