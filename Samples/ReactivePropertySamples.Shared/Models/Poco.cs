using System;
using System.Collections.Generic;
using System.Text;

namespace ReactivePropertySamples.Models
{
    public class Poco : BindableBase
    {
        private string _firstName;
        public string FirstName
        {
            get { return _firstName; }
            set { SetProperty(ref _firstName, value); }
        }

        private string _lastName;
        public string LastName
        {
            get { return _lastName; }
            set { SetProperty(ref _lastName, value); }
        }

    }
}
