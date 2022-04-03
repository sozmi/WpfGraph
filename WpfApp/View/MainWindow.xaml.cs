using System.Windows;
using System.Windows.Input;

namespace WpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Items_MouseMove(object sender, MouseEventArgs e)
        {
            MousePoint = e.GetPosition((IInputElement)sender);
        }

        public static readonly DependencyProperty MousePointProperty
  = DependencyProperty.Register(nameof(MousePoint), typeof(Point), typeof(MainWindow));

        public Point MousePoint
        {
            get => (Point)GetValue(MousePointProperty);
            set => SetValue(MousePointProperty, value);
        }
    }
}
