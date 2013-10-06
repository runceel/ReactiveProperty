using Microsoft.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace Codeplex.Reactive.Interactivity
{
    public abstract class TriggerAction : Behavior, IAction
    {
        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.Register(
                "IsEnabled", 
                typeof(bool), 
                typeof(TriggerAction), 
                new PropertyMetadata(true));

        public bool IsEnabled
        {
            get { return (bool)GetValue(IsEnabledProperty); }
            set { SetValue(IsEnabledProperty, value); }
        }

        internal TriggerAction(Type associatedType) : base(associatedType)
        {
        }

        public object Execute(object sender, object parameter)
        {
            if (!this.IsEnabled)
            {
                return null;
            }

            this.Invoke(parameter);
            return Unit.Default;
        }

        protected abstract void Invoke(object parameter);
    }
}
