using System;
using HorizonEngine;

namespace Project_Horizon
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new Scene())
                game.Run();
        }
    }
}
