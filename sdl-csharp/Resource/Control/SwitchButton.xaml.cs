using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace sdl_csharp.Resource.Control
{
    /// <summary>
    /// Interaction logic for SwitchButton.xaml
    /// </summary>
    public partial class SwitchButton : Button
    {
        public SwitchButton()
        {
            InitializeComponent();
            DataContext = this;
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
