using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;
using System.Linq;

public class NetManager : Singleton<NetManager>
{
    Socket mainsocket;
    public List<Client> allclis=new List<Client>();

    public void InitServer()
    {
        mainsocket = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
        mainsocket.Bind(new IPEndPoint(IPAddress.Any,10086));
        mainsocket.Listen(1000);
        Debug.Log("服务器创建成功");
        mainsocket.BeginAccept(OnAcceptCall,null);
    }

    private void OnAcceptCall(IAsyncResult ar)
    {
        try
        {
            Socket socket = mainsocket.EndAccept(ar);
            IPEndPoint iP=socket.RemoteEndPoint as IPEndPoint;
            Debug.Log("用户："+iP.Port+"已经连接服务器");

            Client cli=new Client();
            cli.socket= socket;
            cli.port=iP.Port;
            allclis.Add(cli);

            mainsocket.BeginAccept(OnAcceptCall,null);
            cli.socket.BeginReceive(cli.data,0,cli.data.Length,SocketFlags.None, OnReceiveCall,cli);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
            throw;
        }
    }

    private void OnReceiveCall(IAsyncResult ar)
    {
        try
        {
            Client cli=ar.AsyncState as Client;
            int len=cli.socket.EndReceive(ar);
            if (len>0)
            {
                byte[] rdata = new byte[len];
                Buffer.BlockCopy(cli.data,0,rdata,0,len);
                while(rdata.Length>4)
                {
                    int bodylen=BitConverter.ToInt32(rdata, 0);
                    byte[] bodydata=new byte[bodylen];
                    Buffer.BlockCopy(rdata,4,bodydata,0,bodylen);

                    int id = BitConverter.ToInt32(bodydata,0);
                    byte[] info = new byte[bodydata.Length-4];
                    Buffer.BlockCopy(bodydata,4,info,0,info.Length);
                    MessageManager.Instance.OnBroadCast(id,info,cli);

                    int sylen = rdata.Length - 4 - bodylen;
                    byte[] sydata=new byte[sylen];
                    Buffer.BlockCopy(rdata,4+bodylen,sydata,0,sylen);
                    rdata = sydata;
                }
                cli.socket.BeginReceive(cli.data,0,cli.data.Length,SocketFlags.None,OnReceiveCall,cli) ;
            }
            else
            {
                Debug.Log("用户："+cli.port+"已经断开连接");
                cli.socket.Shutdown(SocketShutdown.Both) ;
                cli.socket.Close();
                allclis.Remove(cli) ;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
            throw;
        }
    }

    private byte[] MakeData(int id, byte[] data)
    {
        byte[] temp = new byte[0];
        int len = data.Length + 4;
        temp = temp.Concat(BitConverter.GetBytes(len)).Concat(BitConverter.GetBytes(id)).Concat(data).ToArray();
        return temp;
    }

    public void OnSendToClient(int id, byte[] data,Client cli)
    {
        byte[] temp = MakeData(id, data);
        cli.socket.BeginSend(temp, 0, temp.Length, SocketFlags.None, OnSendCall, cli);
    }

    private void OnSendCall(IAsyncResult ar)
    {
        try
        {
            Client cli = ar.AsyncState as Client;
            int len = cli.socket.EndSend(ar);
            Debug.Log("服务器向客户端发送：" + len + "字节的消息");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }
}
