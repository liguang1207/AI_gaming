using System;

namespace detector
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (detector game = new detector())
            {
                game.Run();
            }
        }
    }
#endif
}

