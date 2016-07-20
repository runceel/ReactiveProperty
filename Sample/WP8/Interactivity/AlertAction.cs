using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WP8.Interactivity
{
    public class AlertAction : System.Windows.Interactivity.TriggerAction<DependencyObject>
    {
        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Message.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(string), typeof(AlertAction), new PropertyMetadata(null));

        
        protected override void Invoke(object parameter)
        {
            MessageBox.Show(this.Message ?? "");
        }
    }
}
