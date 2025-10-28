using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;
using Microsoft.Xaml.Behaviors;

namespace CvnetClient.Behaviors
{
    public class HeaderDoubleClickCommandAction : TriggerAction<DependencyObject>
    {
        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(HeaderDoubleClickCommandAction));

        protected override void Invoke(object parameter)
        {
            if (parameter is MouseButtonEventArgs e)
            {
                DependencyObject source = e.OriginalSource as DependencyObject;

                // walk up the visual tree to find a DataGridColumnHeader
                while (source != null && source is not DataGridColumnHeader)
                    source = VisualTreeHelper.GetParent(source);

                if (source is DataGridColumnHeader header)
                {
                    var headerName = header.Column?.Header?.ToString();
                    if (Command?.CanExecute(headerName) == true)
                        Command.Execute(headerName);
                }
            }
        }
    }
}
