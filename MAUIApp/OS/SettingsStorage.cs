using Core.Lib.OS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MAUIApp.OS
{
    internal class SettingsStorage : ISettingsStorage
    {
        public T GetValue<T>([CallerMemberName] string propertyName = null) =>
            propertyName switch
            {
                "WebAPIUrl" => (T)(object)Preferences.Get(propertyName, "https://mvvmuniversalwebapis.azurewebsites.net/"), //"https://localhost:44328/"),
                //"IntSetting" => (T)(object)Preferences.Get(propertyName, 10),
                _ => default
            };

        public void SetValue<T>(T newValue = default, [CallerMemberName] string propertyName = null)
        {
            switch (propertyName)
            {
                case "WebAPIUrl":
                    Preferences.Set(propertyName, newValue.ToString());
                    break;
                //case "IntSetting":
                //Preferences.Set(propertyName, int.Parse(newValue.ToString());
                //break;
                default:
                    break;
            }
        }
    }
}
