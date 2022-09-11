using sdl;
using System.Windows;
using System.Windows.Controls;

namespace sdl_csharp.Resource.Control
{
    /// <summary>
    /// Interaction logic for URLEntry.xaml
    /// </summary>
    public partial class URLEntry : UserControl
    {
        public static readonly DependencyProperty SDLWindowReferenceProperty =
            DependencyProperty.Register("SDLWindowReference", typeof(SDLWindow), typeof(URLEntry), new PropertyMetadata(null));

        public SDLWindow SDLWindowReference
        {
            get { return (SDLWindow)GetValue(SDLWindowReferenceProperty); }
            set { SetValue(SDLWindowReferenceProperty, value); }
        }

        public URLEntry()
        {
            InitializeComponent();
        }

        private void ForwardToInitIndividualDownload(object sender, RoutedEventArgs e)
        {
            SDLWindowReference.InitIndividualDownload(sender, e);
        }
    }
}
