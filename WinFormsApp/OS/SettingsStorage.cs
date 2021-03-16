using Core.Lib.OS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WinFormsApp.OS
{
    //TODO: Implements Windows Forms Settings
    public class SettingsStorage : ISettingsStorage
    {
        public T GetValue<T>([CallerMemberName] string propertyName = null)
        {
            throw new NotImplementedException();
        }

        public void SetValue<T>(T newValue = default, [CallerMemberName] string propertyName = null)
        {
            throw new NotImplementedException();
        }
    }
}
