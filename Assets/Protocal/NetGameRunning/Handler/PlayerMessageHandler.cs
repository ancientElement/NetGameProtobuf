using System;
using System.Diagnostics;

namespace NetGameRunning
{
    public class PlayerMessageHandler : AE_NetWork.BaseHandler
    {
        public override void Handle()
        {
            PlayerMessage player = (PlayerMessage)message;
           UnityEngine.Debug.Log("玩家移动到了 " + player.data.Position.X + " " + player.data.Position.Y);
        }
    }
}