#if TCP
//using Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Timers;

/**
 * 负责网络通信底层（TCP/IP协议，ProtocolBuf）
 */
public class Network : MonoBehaviour
{
    public const uint DisconnectionEvent = 1500;//当断开连接 事件

    public static int index=0;

    private byte[] headBuffer = new byte[8] { 0x7D, 0, 0, 0, 0, 1, 0, 0x7F };//125 127

    public string host = "192.168.2.250";
    public int port = 101;//101 201

    private byte[] receiveBuffer = new byte[0xffff];//设置一个缓冲区，用来保存数据
    private TcpClient tcp;
    private NetworkStream stream;

    public delegate void ProtocolEvent(MemoryStream body);
    private Dictionary<uint, ProtocolEvent> listeners = new Dictionary<uint, ProtocolEvent>();
    
    private List<NetworkEvent> dispatchEvents = new List<NetworkEvent>();
    private List<NetworkEvent> eventPool = new List<NetworkEvent>();

    /// <summary>
    /// 将协议和对象类型进行绑定
    /// </summary>
    private Dictionary<uint, Type> idToType=new Dictionary<uint,Type>();
    private Dictionary<Type, uint> typeToId=new Dictionary<Type,uint>();

    //计时器
    private const int KeepAliveInterval = 40000;
    private Timer keepAliveTimer = new Timer();

    void Start()
    {
        /*
        //开启计时器
        keepAliveTimer.Elapsed += KeepAlive;
        keepAliveTimer.Interval = KeepAliveInterval;
        keepAliveTimer.Enabled = true;
    
         */
    }

    public void OnDestroy()
    {
        Close();
    }

    void Update()
    {
        while (dispatchEvents.Count > 0)
        {
            NetworkEvent evt = dispatchEvents[0];
            dispatchEvents.RemoveAt(0);
            eventPool.Add(evt);

            if (listeners.ContainsKey(evt.protocol))
                listeners[evt.protocol](evt.stream);
        }
    }

    /// <summary>
    /// OnDestroy() and Kickout
    /// </summary>
    public void Close()
    {
        if (tcp != null && tcp.Connected)
        {
            tcp.Close();
            tcp = null;
        }
    }


    public void Connect(string host,int port,Action callback)
    {
        this.host = host;
        this.port = port;

        tcp = new TcpClient();
        tcp.BeginConnect(host, port, new AsyncCallback(ConnectCallback), callback);
        Debug.Log("正在连接服务器！ Host:" + host + " Port:" + port);
    }
    private void ConnectCallback(IAsyncResult ar)
    {
        try
        {
            tcp.EndConnect(ar);//出错会抛出异常
            
            stream = tcp.GetStream();
            stream.BeginRead(receiveBuffer, 0, receiveBuffer.Length, Recevie, null);
            Debug.Log("服务器连接成功！ Host:" + host + " Port:" + port);
            Action callback = ar.AsyncState as Action;
            callback();
        }
        catch
        {
            Debug.LogError("连接服务器失败！ Host:" + host + " Port:" + port);
        }
    }


    //发送接口
    public void Send(uint protocolId, IExtensible body)
    {
        MemoryStream bodyStream;
        int bodyLength;
        if (body != null)
        {
            //序列化包体
            bodyStream = new MemoryStream();
            Serializer.Serialize(bodyStream, body);
            bodyLength = (int)bodyStream.Length;
        }
        else
        {
            bodyLength = 0;
            bodyStream = null;
        }

        int totalLength = 8 + 4 + bodyLength;//协议头 + 4字节协议id + 包体
        Array.Copy(BitConverter.GetBytes(totalLength), 0, headBuffer, 1, 4); //包头写入长度

        byte[] buffer = new byte[totalLength];
        Array.Copy(headBuffer, 0, buffer, 0, 8);
        Array.Copy(BitConverter.GetBytes(protocolId), 0, buffer, 8, 4);
        if(bodyLength > 0) Array.Copy(bodyStream.GetBuffer(), 0, buffer, 12, bodyLength);
        stream.Write(buffer, 0, totalLength);

        Debug.Log("发送协议：" + protocolId);
    }

    //
    
    //异步接受

    //缓存上一个包的数据（断包情况）
    private uint protocol;//协议号
    private int bodyWriteIndex;//包写入索引
    private int bodyLength;//包长
    private byte[] bodyBuffer;//包体

    /*
     * 解包流程：
     */

    /// <summary>
    /// 
    /// 粘包的典型四种情况
    /// A.先接收到data1,然后接收到data2.（正常情况）
    /// B.先接收到data1的部分数据,然后接收到data1余下的部分以及data2的全部. （属于半包+粘包）
    /// C.先接收到了data1的全部数据和data2的部分数据,然后接收到了data2的余下的数据.（属于粘包+半包）
    /// D.一次性接收到了data1和data2的全部数据.（属于粘包）
    /// </summary>
    /// <param name="ar"></param>
    private void Recevie(IAsyncResult ar)
    {
        int numberOfBytesRead = 0;
        try
        {
            numberOfBytesRead = stream.EndRead(ar);//读取流长度
        }
        catch
        {
            Debug.LogError("连接关闭！");//比如本机网络断开

            //因为要显示断线面板，所以需要在主线程中派发
            NetworkEvent evt = GetEvent();
            evt.protocol = DisconnectionEvent;
            evt.stream = null;
            dispatchEvents.Add(evt);
            return;
        }

        int readIndex = 0;//当前数据包读取索引
        while(readIndex < numberOfBytesRead)
        {
			//断包处理
            if (bodyWriteIndex > 0)
            {
				int leftLength = bodyLength - bodyWriteIndex;
				int writeLength = Mathf.Min(leftLength, numberOfBytesRead);
                Array.Copy(receiveBuffer, 0, bodyBuffer, bodyWriteIndex, writeLength);

                readIndex += writeLength;
                bodyWriteIndex += writeLength;
            }
            //新包/粘包
            else
            {
                byte startFlag = receiveBuffer[readIndex]; readIndex += 1;
                if (startFlag != 0x7D)
                {
                    Debug.LogError("协议头错误！startFlag:" + startFlag + " readIndex:" + readIndex + " numberOfBytesRead:" + numberOfBytesRead);
                    return;
                }
                int totalLength = BitConverter.ToInt32(receiveBuffer, readIndex); readIndex += 4;  //eader.ReadInt32();
                if (totalLength == 0)
                {
                    Debug.LogError("协议长度为0");
                    return;
                }
                byte type = receiveBuffer[readIndex]; readIndex += 1;
                byte key = receiveBuffer[readIndex]; readIndex += 1;
                byte endFlag = receiveBuffer[readIndex]; readIndex += 1;
                protocol = BitConverter.ToUInt32(receiveBuffer, readIndex); readIndex += 4;

                bodyLength = totalLength - 12;//包体长度
                int writeLength = Mathf.Min(bodyLength, numberOfBytesRead - readIndex);//写入包的长度
                bodyBuffer = new byte[bodyLength];
                Array.Copy(receiveBuffer, readIndex, bodyBuffer, 0, writeLength);

                readIndex += writeLength;
                bodyWriteIndex += writeLength;
            }

            //完成一个包
            if(bodyWriteIndex == bodyLength)
            {
                NetworkEvent evt = GetEvent();
                evt.protocol = protocol;
                evt.stream = new MemoryStream(bodyBuffer);
                dispatchEvents.Add(evt);
                Debug.Log("收到协议：" + protocol + " 包长：" + bodyLength);

                //清空缓存
                protocol = 0;
                bodyWriteIndex = 0;
                bodyLength = 0;
                bodyBuffer = null;
            }
        }

        stream.BeginRead(receiveBuffer, 0, receiveBuffer.Length, Recevie, null);
    }

    private NetworkEvent GetEvent()
    {
        NetworkEvent evt;
        if (eventPool.Count > 0)
        {
            evt = eventPool[0];
            eventPool.RemoveAt(0);
        }
        else
        {
            evt = new NetworkEvent();
        }
        return evt;
    }

    //侦听
    public void Listen(uint protocolId, ProtocolEvent call)
    {
        if(!listeners.ContainsKey(protocolId))
            listeners.Add(protocolId, delegate(MemoryStream body) { });
       
        
        listeners[protocolId] += call;
    }

    //取消侦听
    public void UnListen(uint protocolId, ProtocolEvent call)
    {
        if (listeners.ContainsKey(protocolId))
        {
            listeners[protocolId] -= call;
        }
    }

    /*
    /// <summary>
    /// 发送心跳协议
    /// </summary>
    private void KeepAlive(object send,ElapsedEventArgs args)
    {
        if (tcp != null && tcp.Connected)
        {
            Debug.Log("心跳协议！");
            Send((uint)Protocol.SystemProtocolID.C2S_NtfHeartBeat, null);
        }
    }
     */

}
internal class NetworkEvent
{
    public uint protocol;
    public MemoryStream stream;
}
#endif