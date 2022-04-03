using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WpfApp.ViewModels;

namespace WpfApp.View
{

    /// <summary>
    /// Логика взаимодействия для Vertex.xaml
    /// </summary>
    public partial class VertexUC : UserControl
    {
        public static readonly DependencyProperty MyTextProperty
            = DependencyProperty.Register(nameof(MyText), typeof(string), typeof(VertexUC));


        public static readonly DependencyProperty MyMarginProperty
            = DependencyProperty.Register(nameof(MyMargin), typeof(Thickness), typeof(VertexUC));

        public static readonly DependencyProperty MyIndexProperty
    = DependencyProperty.Register(nameof(Index), typeof(int), typeof(VertexUC));

        public static readonly DependencyProperty MyColorProperty
    = DependencyProperty.Register(nameof(MyColor), typeof(SolidColorBrush), typeof(VertexUC));

        public VertexUC()
        {
            InitializeComponent();
        }

        public string MyText
        {
            get => (string)GetValue(MyTextProperty);
            set => SetValue(MyTextProperty, value);
        }
        public Thickness MyMargin
        {
            get => (Thickness)GetValue(MyMarginProperty);
            set => SetValue(MyMarginProperty, value);
        }
        public int Index
        {
            get => (int)GetValue(MyIndexProperty);
            set => SetValue(MyIndexProperty, value);
        }
        public SolidColorBrush MyColor
        {
            get => (SolidColorBrush)GetValue(MyColorProperty);
            set => SetValue(MyColorProperty, value);
        }

        private void Ellipse_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Selected?.Invoke(Index);
        }

        public static event Action<int> Selected;
    }
}
