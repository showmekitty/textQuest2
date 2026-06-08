#nullable disable
using System;
using System.Windows.Forms;
using Game.Forms;

namespace Game
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new GameForm());
        }
    }
}