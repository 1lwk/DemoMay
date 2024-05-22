using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

/// <summary>
/// 客户端数据类
/// </summary>
public class Client
{
    public Socket socket;
    public int port;
    public byte[] data=new byte[1024];
}
