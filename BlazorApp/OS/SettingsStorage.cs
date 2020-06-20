using Core.Lib.OS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace BlazorApp.OS
{
    public class SettingsStorage : ISettingsStorage
    {
        internal class Values
        {
            public string WebAPIUrl { get; set; }
            //public int IntSetting { get; set; }
        }

        internal static Values CurrentValues { get; set; }

        public T GetValue<T>([CallerMemberName] string propertyName = null) =>
            propertyName switch
            {
                "WebAPIUrl" => (T)(object)CurrentValues.WebAPIUrl,
                //"IntSettings" => (T)(object)CurrentValues.IntSetting,
                _ => default
            };

        public void SetValue<T>(T newValue = default, [CallerMemberName] string propertyName = null)
        {
            //TODO: Implement localstorage https://blazor.tips/localstorage-with-blazor/
            throw new NotImplementedException();
        }
    }
}
