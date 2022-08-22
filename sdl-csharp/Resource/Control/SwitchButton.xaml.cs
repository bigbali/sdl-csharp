using System.Windows;
using System.Windows.Controls;

namespace sdl_csharp.Resource.Control
{
    public partial class SwitchButton : Button
    {
        public SwitchButton()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty StateProperty =
            DependencyProperty.Register("State", typeof(bool), typeof(SwitchButton), new PropertyMetadata(false));
        public static readonly DependencyProperty Content1Property =
            DependencyProperty.Register("Content1", typeof(string), typeof(SwitchButton), new PropertyMetadata(null));
        public static readonly DependencyProperty Content2Property =
            DependencyProperty.Register("Content2", typeof(string), typeof(SwitchButton), new PropertyMetadata(null));

        public bool State
        {
            get { return (bool)GetValue(StateProperty); }
            set { SetValue(StateProperty, value); }
        }

        public string Content1
        {
            get { return (string)GetValue(Content1Property); }
            set { SetValue(Content1Property, value); }
        }

        public string Content2
        {
            get { return (string)GetValue(Content2Property); }
            set { SetValue(Content2Property, value); }
        }
    }
}
