using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define
{
    public const int SC_REFRESH = 1;
}
/// <summary>
/// 消息中心
/// </summary>
public class MessageManager:Singleton<MessageManager>
{
    public Dictionary<int,Action<object>> msgDic=new Dictionary<int, Action<object>>();

    public void OnAddListen(int id,Action<object> action)
    {
        if(msgDic.ContainsKey(id))
        {
            msgDic[id] += action;
        }
        else
        {
            msgDic.Add(id, action);
        }
    }

    public void OnRemoveListen(int id, Action<object> action)
    {
        if(msgDic.ContainsKey(id))
        {
            msgDic[id] -= action;
            if (msgDic[id]==null)
            {
                msgDic.Remove(id);
            }
        }
    }

    public void OnBroadCast(int id,params object[] arr)
    {
        if(msgDic.ContainsKey(id))
        {
            msgDic[id](arr);
        }
    }
}
