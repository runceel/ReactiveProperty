using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWP
{
    class Item : INotifyPropertyChanged
    {
        private static readonly PropertyChangedEventArgs ValuePropertyChangedEventArgs = new PropertyChangedEventArgs(nameof(Value));

        private string value;

        public string Value
        {
            get { return this.value; }
            set
            {
                if (this.value == value) { return; }
                this.value = value;
                this.PropertyChanged?.Invoke(this, ValuePropertyChangedEventArgs);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public override string ToString() => this.Value;
    }
}
