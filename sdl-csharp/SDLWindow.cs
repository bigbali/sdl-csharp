using sdl_csharp.Resource.Model;
using System;
using System.Windows;

namespace sdl_csharp
{
    public partial class SDLWindow : Window
    {
        public Settings WindowSettings { get; set; } = Settings.Instance;
        public SDLWindow()
        {
            InitializeComponent();

            Console.WriteLine("MIAPICSA");

            // We use this to keep track of our main window from across multiple threads
            Download.SDLWindowReference = this; 
        }
    }
}
