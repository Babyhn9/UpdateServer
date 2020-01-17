using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ML
{
    public class VMBase : INotifyPropertyChanged

    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void PropertyChange([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
