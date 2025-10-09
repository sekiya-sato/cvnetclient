using CvnetClient.Models;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CvnetClient.Views
{
    /// <summary>
    /// Interaction logic for CvnetBtListView.xaml
    /// </summary>
    public partial class CvnetBtListView : UserControl
    {
        public CvnetBtListView()
        {
            InitializeComponent();
        }

        public int StyleDesign
        {
            get => (int)GetValue(ModeProperty);
            set => SetValue(ModeProperty, value);
        }

        public static readonly DependencyProperty ModeProperty =
            DependencyProperty.Register(
                nameof(StyleDesign),
                typeof(int),
                typeof(CvnetBtListView),
                new PropertyMetadata(1, OnModeChanged));

        private static void OnModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CvnetBtListView control)
            {
                control.ApplyMode();
            }
        }

        private void ApplyMode()
        {
            if (StyleDesign == 1)
                MyButton.Style = (Style)FindResource("ComboLabelBtn");
            else if (StyleDesign == 2)
                MyButton.Style = (Style)FindResource("ComboButton");
            else if (StyleDesign == 3)
                MyButton.Style = (Style)FindResource("ComboButton2");
            else if (StyleDesign == 4)
                MyButton.Style = (Style)FindResource("ComboButton3");
            
        }


        // DependencyProperty for Button Title
        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(CvnetBtListView));

        // DependencyProperty for mstname
        public string MstName
        {
            get => (string)GetValue(MstNameProperty);
            set => SetValue(MstNameProperty, value);
        }

        public static readonly DependencyProperty MstNameProperty =
            DependencyProperty.Register("MstName", typeof(string), typeof(CvnetBtListView));

        // DependencyProperty for SelectedValue
        public string SelectedValue
        {
            get => (string)GetValue(SelectedValueProperty);
            set => SetValue(SelectedValueProperty, value);
        }
        public static readonly DependencyProperty SelectedValueProperty =
            DependencyProperty.Register("SelectedValue", typeof(string), typeof(CvnetBtListView));

        // DependencyProperty for v_para2
        public object Param2
        {
            get => (object)GetValue(Param2Property);
            set => SetValue(Param2Property, value);
        }
        public static readonly DependencyProperty Param2Property =
            DependencyProperty.Register("Param2", typeof(object), typeof(CvnetBtListView));

        // === Command Property ===
        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(CvnetBtListView));


        // === CommandParameter Property (optional) ===
        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register(nameof(CommandParameter), typeof(object), typeof(CvnetBtListView));

        private void BtList_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(MstName)) return;
            Type? type = null;

            string default_view = typeof(SubDlgSel00View).FullName ?? string.Empty;
            type = Type.GetType(default_view);
              
            if (AppData.ClassCvnet.MstDialog.TryGetValue(MstName, out var mst)) {
                if (!string.IsNullOrEmpty(mst.v_mstname))
                    type = Type.GetType(mst.v_mstname); 
            }
            if (type == null) return;
             
            if (Activator.CreateInstance(type) is Window win)
            {
                var wrk_para = new string[] { SelectedValue ?? string.Empty };
                // Get DataContext (ViewModel)
                var vm = win.DataContext;
                if (vm != null)
                { 
                    var method = vm.GetType().GetMethod("OnInit", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                        null,
                        new Type[] { typeof(string), typeof(string[]), typeof(string[]) },
                        null); 

                    string[] v_para2 = null;
                    if (Param2 != null)
                    {
                        if (Param2 is string s)
                        {
                            v_para2 = new string[] { s };
                        }
                        else if (Param2 is string[] arr)
                        {
                            v_para2 = arr;
                        }
                    }

                    if (method != null) {
                        method?.Invoke(vm, new object?[] { MstName, wrk_para, v_para2 });
                    }
                }

                if (win.ShowDialog() == true)
                {
                    var selectedProp = vm?.GetType().GetProperty("SelectedValue");
                    var selectedValue = selectedProp?.GetValue(vm);

                    var result = selectedValue;
                    if (Param2 != null)
                    {
                        result = (selectedValue, Param2);
                    } 

                    if (Command?.CanExecute(result) == true)
                    Command.Execute(result);
                }
            }
        }
    }
}
