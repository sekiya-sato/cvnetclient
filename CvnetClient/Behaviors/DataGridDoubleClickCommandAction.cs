using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CvnetClient.Behaviors
{
    public static class DataGridDoubleClick
    {
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached(
                "Command",
                typeof(ICommand),
                typeof(DataGridDoubleClick),
                new PropertyMetadata(null, OnCommandChanged));

        public static void SetCommand(DependencyObject obj, ICommand value) =>
            obj.SetValue(CommandProperty, value);

        public static ICommand GetCommand(DependencyObject obj) =>
            (ICommand)obj.GetValue(CommandProperty);

        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DataGrid dg)
            {
                dg.MouseDoubleClick -= Dg_MouseDoubleClick;
                dg.MouseDoubleClick += Dg_MouseDoubleClick;
            }
        }

        private static void Dg_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is not DataGrid dg) return;

            var command = GetCommand(dg);
            var selectedItem = dg.SelectedItem;

            if (command != null && selectedItem != null && command.CanExecute(selectedItem))
            {
                command.Execute(selectedItem);
            }
        }
    }
}
