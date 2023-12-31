﻿using AE_NetWork;
using Microsoft.Win32;
using NetGameRunning;
using NetSystem;
using System.Net.Sockets;

namespace TeachTCPAsync
{
    public class ClientSocket
    {
        private static int CLIENT_BEGIN_ID = 1;
        public int clientID;
        public Socket socket;

        private MessagePool messagePool = new MessagePool();

        public bool Connected => socket.Connected;

        //缓存
        private byte[] bufferBytes = new byte[1024 * 1024];

        //缓存长度
        private int bufferLenght = 0;

        public static readonly float TimeOutTime = 5f;

        private long lastHeartMessageTime = -1;

        public ClientSocket(Socket socket)
        {
            this.clientID = CLIENT_BEGIN_ID;
            this.socket = socket;
            ++CLIENT_BEGIN_ID;

            this.socket.BeginReceive(bufferBytes, bufferLenght, bufferBytes.Length - bufferLenght, SocketFlags.None, Recive, null);
            ThreadPool.QueueUserWorkItem(CheckTimeOut);
        }

        private void CheckTimeOut(object? state)
        {
            while (socket.Connected && lastHeartMessageTime != -1)
            {
                Thread.Sleep(5000);

                if ((DateTime.Now.Ticks / TimeSpan.TicksPerSecond - lastHeartMessageTime) > TimeOutTime)
                {
                    Program.socket.CloseClientSocket(this);
                }
            }
        }

        private void Recive(IAsyncResult result)
        {
            try
            {
                if (this.socket != null && this.socket.Connected)
                {

                    int byteLength = this.socket.EndReceive(result);

                    HandleReceiveMessage(byteLength, () =>
                    {
                        this.socket.BeginReceive(bufferBytes, bufferLenght, bufferBytes.Length - bufferLenght, SocketFlags.None, Recive, null);
                    });
                }
                else
                {
                    Console.WriteLine("没有连接，不用再收消息了");
                    Program.socket.CloseClientSocket(this);
                }
            }
            catch (Exception e)
            {
                if (e is SocketException)
                {
                    Console.WriteLine($"接收消息出错 [{socket.RemoteEndPoint}] {(e as SocketException).ErrorCode}:{e.Message}");
                }
                else
                {
                    Console.WriteLine($"接收消息出错 [{socket.RemoteEndPoint}] :{e.Message}");
                }
                Program.socket.CloseClientSocket(this);
            }
        }

        private void HandleReceiveMessage(int bytesLength, Action callback)
        {
            if (bytesLength == 0) return;

            //处理
            int massageID = -1;
            int massageBodyLength = -1;
            int currentIndex = 0;

            bufferLenght += bytesLength;

            while (true)//粘包
            {
                if (bufferLenght >= 8)
                {
                    //ID
                    massageID = BitConverter.ToInt32(bufferBytes, currentIndex);
                    currentIndex += 4;
                    //长度
                    massageBodyLength = BitConverter.ToInt32(bufferBytes, currentIndex) - 8;
                    currentIndex += 4;
                }

                if (bufferLenght - currentIndex >= massageBodyLength && massageBodyLength != -1 && massageID != -1)
                {
                    //消息体 
                    BaseMessage baseMassage = messagePool.GetMessage(massageID);

                    if (massageBodyLength != 0)
                        baseMassage.WriteIn(bufferBytes, currentIndex, massageBodyLength);

                    if (baseMassage != null)
                    {
                        BaseHandler handler = messagePool.GetHandler(massageID);
                        handler.message = baseMassage;

                        if (handler != null)
                            ThreadPool.QueueUserWorkItem(HandleMassage, handler);
                    }

                    currentIndex += massageBodyLength;
                    if (currentIndex == bufferLenght)
                    {
                        bufferLenght = 0;
                        break;
                    }
                }
                else//分包
                {
                    if (massageBodyLength != -1)
                        currentIndex -= 8;
                    Array.Copy(bufferBytes, currentIndex, bufferBytes, 0, bufferLenght - currentIndex);
                    bufferLenght = bufferLenght - currentIndex;
                    break;
                }
            }

            //继续接收
            callback?.Invoke();
        }

        private void HandleMassage(object? state)
        {
            if (state == null)
            {
                Console.WriteLine($"接收消息出错: 消息内容为null");
                return;
            }

            BaseHandler handler = state as BaseHandler;

            if (handler == null)
            {
                Console.WriteLine($"接收消息出错: 消息内容为null");
                return;
            }

            handler.Handle();
        }

        public void Send(BaseMessage info)
        {
            if (!Connected)
            {
                Program.socket.CloseClientSocket(this);
                return;
            }
            try
            {
                this.socket.BeginSend(info.GetBytes(), 0, info.GetByteLength(), SocketFlags.None, SendCallback, null);
            }
            catch (Exception e)
            {
                Console.WriteLine($"发送消息出错: {e.Message}");
                Program.socket.CloseClientSocket(this);
            }
        }

        private void SendCallback(IAsyncResult result)
        {
            try
            {
                this.socket.EndSend(result);
            }
            catch (SocketException e)
            {
                Console.WriteLine($"发送消息出错 {e.SocketErrorCode}:{e.Message}");
            }
        }

        public void Close()
        {
            if (Connected)
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
        }
    }
}
