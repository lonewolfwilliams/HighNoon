using System;

namespace WindowsGame1
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (GameContext game = new GameContext())
            {
                game.Run();
            }
        }
    }
#endif
}

