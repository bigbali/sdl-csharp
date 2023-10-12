using sdl_csharp.Model;
using sdl_csharp.Utility;
using System.Windows;

namespace sdl_csharp
{
    public partial class SDLWindow : Window
    {
        public Settings WindowSettings { get; set; } = Settings.Instance;
        public SDLWindow()
        {
            InitializeComponent();
        }
    }
}
