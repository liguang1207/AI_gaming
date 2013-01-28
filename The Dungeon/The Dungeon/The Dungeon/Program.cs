using System;

namespace The_Dungeon
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (TDGame game = new TDGame())
            {
                game.Run();
            }
        }
    }
#endif
}
