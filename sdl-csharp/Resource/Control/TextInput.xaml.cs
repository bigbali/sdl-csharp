using System.Windows;
using System.Windows.Controls;

namespace sdl_csharp.Resource.Control
{
    public partial class TextInput : TextBox
    {
        public TextInput()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty PlaceholderProperty =
            DependencyProperty.Register("Placeholder", typeof(string), typeof(TextInput), new PropertyMetadata(string.Empty));

        public string Placeholder
        {
            get => (string)GetValue(PlaceholderProperty);
            set => SetValue(PlaceholderProperty, value);
        }
    }
}
