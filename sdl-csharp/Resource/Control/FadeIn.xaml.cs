using System.Windows;
using System.Windows.Controls;

namespace sdl_csharp.Resource.Control
{
    /// <summary>
    /// Interaction logic for FadeIn.xaml
    /// </summary>
    public partial class FadeIn : UserControl
    {
        public FadeIn()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(object), typeof(FadeIn), new PropertyMetadata(null));

        public object Value
        {
            get => GetValue(ValueProperty);
            set { SetValue(ValueProperty, value); }
        }
    }
}
