using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace Enable_Now_Konnektor_Editor.src.helper
{
    public class PairValue : INotifyPropertyChanged
    {

        public PairValue(string key, string val)
        {
            Key = key;
            Value = val;
        }

        private string key;
        public string Key
        {
            get { return key; }
            set { key = value; OnPropertyChanged(); }
        }

        private string val;
        public string Value
        {
            get { return val; }
            set { val = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
