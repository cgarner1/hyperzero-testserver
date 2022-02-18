using System;
using System.Threading;

namespace HyperZero_GameServer
{
    class ServerRunner
    {
        private static bool isRunning;
        static void Main(string[] args)
        {
            Console.WriteLine("Server Starting...");
            Console.Title = "Unity TestServer";
            isRunning = true;

            Thread thread = new Thread(new ThreadStart(MainThread));
            thread.Start();

            Server.Start(4, 26950);

            
        }

        private static void MainThread()
        {
            Console.WriteLine($"Main thread running -- {Constants.TICKS_PER_SECOND} ticks per second");
            DateTime nextLoop = DateTime.Now;

            while (isRunning)
            {
                // check if there's a better way then checking as fast as server will run...
                while (nextLoop < DateTime.Now)
                {
                    GameLogic.Update();

                    nextLoop = nextLoop.AddMilliseconds(Constants.MS_PER_TICK);

                    if (nextLoop > DateTime.Now) Thread.Sleep(nextLoop - DateTime.Now);
                }
            }
        }
        
    }
}
