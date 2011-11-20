using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using WP7.ViewModels;

namespace WP7.Views
{
    public partial class Validation : PhoneApplicationPage
    {
        public Validation()
        {
            InitializeComponent();
            DataContext = new ValidationViewModel();
        }
    }
}