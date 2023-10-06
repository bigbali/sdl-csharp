using sdl_csharp.Resource.Model;
using sdl_csharp.Utility;
using System;
using System.Diagnostics;
using System.Windows;

namespace sdl_csharp
{
    public partial class SDLWindow : Window
    {
        public Settings WindowSettings { get; set; } = Settings.Instance;
        public SDLWindow()
        {
            InitializeComponent();

            Logger.Log("Startup");

            // We use this to keep track of our main window from across multiple threads
            Download.SDLWindowReference = this; 
        }
    }
}
