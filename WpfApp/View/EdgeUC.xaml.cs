using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfApp.View
{
    /// <summary>
    /// Логика взаимодействия для EdgeUC.xaml
    /// </summary>
    public partial class EdgeUC : UserControl
    {
        public static readonly DependencyProperty MyColorProperty = DependencyProperty.Register(nameof(Color), typeof(SolidColorBrush), typeof(EdgeUC));
        public SolidColorBrush MyColor
        {
            get => (SolidColorBrush)GetValue(MyColorProperty);
            set => SetValue(MyColorProperty, value);
        }

        public static readonly DependencyProperty PathDataProperty = DependencyProperty.Register(nameof(PathData), typeof(PathGeometry), typeof(EdgeUC));
        public PathGeometry PathData
        {
            get => (PathGeometry)GetValue(PathDataProperty);
            set => SetValue(PathDataProperty, value);
        }


        public static readonly DependencyProperty MyIndexProperty = DependencyProperty.Register(nameof(Index), typeof(string), typeof(EdgeUC));
        public string Index
        {
            get => (string)GetValue(MyIndexProperty);
            set => SetValue(MyIndexProperty, value);
        }

        public static readonly DependencyProperty MyMarginProperty = DependencyProperty.Register(nameof(MyMargin), typeof(Thickness), typeof(EdgeUC));
        /// <summary>
        /// координаты находящиеся ближе к системе координат
        /// </summary>
        public Thickness MyMargin
        {
            get => (Thickness)GetValue(MyMarginProperty);
            set => SetValue(MyMarginProperty, value);
        }

        public EdgeUC()
        {
            InitializeComponent();
        }
    }
}
