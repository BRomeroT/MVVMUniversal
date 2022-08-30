using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Core.Lib.OS
{
    interface ISettingsStorage
    {
        void SetValue<T>(T newValue = default, [CallerMemberName] string propertyName = null);

        T GetValue<T>([CallerMemberName] string propertyName = null);
    }
}
