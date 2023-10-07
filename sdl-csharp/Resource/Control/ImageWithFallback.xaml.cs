using System.Windows;
using System.Windows.Controls;

namespace sdl_csharp.Resource.Control
{
    /// <summary>
    /// Interaction logic for ImageWithFallback.xaml
    /// </summary>
    public partial class ImageWithFallback : UserControl
    {
        public ImageWithFallback()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(object), typeof(ImageWithFallback), new PropertyMetadata(null));

        // Can't cast to ImageSource because we want to avoid handling 'null'
        // since it's handled in XAML
        public object Source
        {
            get => GetValue(SourceProperty);
            set { SetValue(SourceProperty, value); }
        }
    }
}
