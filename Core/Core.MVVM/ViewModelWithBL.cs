using System;
using System.Collections.Generic;
using System.Text;

namespace Sysne.Core.MVVM
{
    public class ViewModelWithBL<T> : ViewModelBase where T : new()
    {
        readonly protected T bl;
        public ViewModelWithBL() => bl = new T();

        public ViewModelWithBL(T bl) => this.bl = bl;
    }
}
