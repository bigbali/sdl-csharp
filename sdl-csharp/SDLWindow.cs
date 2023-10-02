using sdl_csharp.Resource.Model;
using System;
using System.Windows;

namespace sdl
{
    public partial class SDLWindow : Window
    {
        public Settings WindowSettings { get; set; } = new();
        public SDLWindow()
        {
            InitializeComponent();

            Console.WriteLine("MIAPICSA");

            // We use this to keep track of our main window from across multiple threads
            sdl_csharp.Download.SDLWindowReference = this; 
        }
    }
}
