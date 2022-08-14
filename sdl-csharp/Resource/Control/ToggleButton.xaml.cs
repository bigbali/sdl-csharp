using System.Windows;
using System.Windows.Controls;

namespace sdl_csharp.Resource.Control
{
    /// <summary>
    /// Interaction logic for Toggle.xaml
    /// </summary>
    public partial class ToggleButton : Button
    {
        public ToggleButton()
        {
            InitializeComponent();
            DataContext = this;
        }

        public static readonly DependencyProperty StateProperty = 
            DependencyProperty.Register("State", typeof(bool), typeof(ToggleButton), new PropertyMetadata(false));

        public bool State
        {
            get { return (bool)GetValue(StateProperty); }
            set { SetValue(StateProperty, value); }
        }
    }
}
