using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace WTalk.Server.ViewModels
{
    public class ClientViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void Notify(string propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }
        string name;
        public string Name
        {
            get { return this.name}
            set
            {
                if (this.name == value) return;
                this.name = value;
                Notify("Name");
            }
        }

        int age;
        public int Age
        {
            get { return this.age; }
            set
            {
                if (this.age == Age) return;
                this.age = value;
                Notify("Age");
            }
        }
    }

}
