using System;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

using AE_NetWork;
using NetGameRunning;

public class TesNetMgrAsync : MonoBehaviour
{
    [SerializeField] Button NormalButton;
    [SerializeField] Button SplitButton;
    [SerializeField] Button JoinButton;
    [SerializeField] Button SplitJoinButton;
    [SerializeField] InputField inputField;

    public float ConnectIntervalTime = 1f;
    public float ConnectTime = 0;

    private void Update()
    {
        if (!NetAsyncMgr.Instance.IsConnected)
        {
            ConnectTime -= Time.deltaTime;
            if (ConnectTime <= 0)
            {
                NetAsyncMgr.Instance.Connect("127.0.0.1", 8080);
                ConnectTime = ConnectIntervalTime;
            }
        }
    }

    private void Start()
    {
        if (NetAsyncMgr.Instance == null)
        {
            var obj = new GameObject("NetAsyncMgr");
            obj.AddComponent<NetAsyncMgr>();
        }
        NetAsyncMgr.Instance.Connect("127.0.0.1", 8080);

        NormalButton.onClick.AddListener(() =>
        {
            PlayerMessage playerMassage = new PlayerMessage()
            {
                data = new PlayerData()
                {
                    Position = new PositionData()
                    {
                        X = 10026,
                        Y = 10035,
                    }
                }
            };
            NetAsyncMgr.Instance.Send(playerMassage);
        });

        //分包
        SplitButton.onClick.AddListener(async () =>
        {
            PlayerMessage playerMassage1 = new PlayerMessage()
            {
                data = new PlayerData()
                {
                    Position = new PositionData()
                    {
                        X = 222,
                        Y = 222,
                    }
                }
            };

            byte[] bytes = playerMassage1.GetBytes();

            byte[] newbytes1 = new byte[10];
            byte[] newbytes2 = new byte[playerMassage1.GetByteLength() - 10];

            Array.Copy(bytes, 0, newbytes1, 0, 10);
            Array.Copy(bytes, 10, newbytes2, 0, playerMassage1.GetByteLength() - 10);

            NetAsyncMgr.Instance.SendTest(newbytes1);
            await Task.Delay(500);
            NetAsyncMgr.Instance.SendTest(newbytes2);
        });

        //粘包
        JoinButton.onClick.AddListener(() =>
        {
            PlayerMessage playerMassage1 = new PlayerMessage()
            {
                data = new PlayerData()
                {
                    Position = new PositionData()
                    {
                        X = 333,
                        Y = 333,
                    }
                }
            };
            PlayerMessage playerMassage2 = new PlayerMessage()
            {
                data = new PlayerData()
                {
                    Position = new PositionData()
                    {
                        X = 444,
                        Y = 444,
                    }
                }
            };

            byte[] newbytes = new byte[playerMassage1.GetByteLength() + playerMassage2.GetByteLength()];
            playerMassage1.GetBytes().CopyTo(newbytes, 0);
            playerMassage2.GetBytes().CopyTo(newbytes, playerMassage1.GetByteLength());
            NetAsyncMgr.Instance.SendTest(newbytes);
        });

        //分包粘包
        SplitJoinButton.onClick.AddListener(async () =>
        {
            PlayerMessage playerMassage1 = new PlayerMessage()
            {
                data = new PlayerData()
                {
                    Position = new PositionData()
                    {
                        X = 555,
                        Y = 555,
                    }
                }
            };
            PlayerMessage playerMassage2 = new PlayerMessage()
            {
                data = new PlayerData()
                {
                    Position = new PositionData()
                    {
                        X = 666,
                        Y = 666,
                    }
                }
            };

            byte[] bytes1 = playerMassage1.GetBytes();
            byte[] bytes2 = playerMassage2.GetBytes();


            byte[] newbytes1 = new byte[playerMassage1.GetByteLength() + 10];

            byte[] newbytes2 = new byte[playerMassage2.GetByteLength() - 10];

            Array.Copy(bytes1, 0, newbytes1, 0, playerMassage1.GetByteLength());
            Array.Copy(bytes2, 0, newbytes1, playerMassage1.GetByteLength(), 10);

            Array.Copy(bytes2, 10, newbytes2, 0, playerMassage2.GetByteLength() - 10);

            NetAsyncMgr.Instance.SendTest(newbytes1);
            await Task.Delay(500);
            NetAsyncMgr.Instance.SendTest(newbytes2);

        });
    }
}
