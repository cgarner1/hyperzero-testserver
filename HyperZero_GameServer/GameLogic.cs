using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperZero_GameServer
{
    class GameLogic
    {
        public static void Update()
        {
            foreach(Client client in Server.players.Values)
            {
                if (client.playerRef != null) client.playerRef.Update();
            }
            ThreadManager.UpdateMain();
        }
    }
}
