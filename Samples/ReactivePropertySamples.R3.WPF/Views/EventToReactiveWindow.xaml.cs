using System;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using ReactivePropertySamples.Migrated.ViewModels;

namespace ReactivePropertySamples.R3.WPF.Views
{
    /// <summary>
    /// Interaction logic for EventToReactiveWindow.xaml
    /// </summary>
    public partial class EventToReactiveWindow : Window
    {
        public EventToReactiveWindow()
        {
            InitializeComponent();
        }

        // Replaces EventToReactiveProperty + MouseEventConverter.
        private void Border_MouseMove(object sender, MouseEventArgs e)
        {
            if (DataContext is not EventToReactiveViewModel vm)
            {
                return;
            }

            var position = e.GetPosition((IInputElement)sender);
            vm.MousePosition.Value = $"({(int)position.X}, {(int)position.Y})";
        }

        // Replaces EventToReactiveCommand + OpenFileConverter.
        private void OpenFileButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is not EventToReactiveViewModel vm)
            {
                return;
            }

            var command = (ICommand)vm.OpenFileCommand;
            if (!command.CanExecute(null))
            {
                return;
            }

            var dialog = new OpenFileDialog();
            if (!(dialog.ShowDialog() ?? false))
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(dialog.FileName))
            {
                return;
            }

            command.Execute(dialog.FileName);
        }
    }
}
