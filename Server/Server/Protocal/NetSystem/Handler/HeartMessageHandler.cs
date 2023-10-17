namespace NetSystem
{
    public class HeartMessageHandler : AE_NetWork.BaseHandler
    {
        public override void Handle()
        {
            Console.WriteLine("接收到心跳消息");
        }
    }
}