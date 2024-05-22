using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlaneInfo;
using Google.Protobuf;
using System;

/// <summary>
/// 玩家控制类
/// </summary>
public class PlaneController : MonoBehaviour
{
    public Button automaticbtn;
    public Button manualbtn;
    public bool isState = false;
    public List<Vector3> waypoint = new List<Vector3>();
    public float speed = 3;
    public int index = 0;
    public bool isTurnacorner = false;
    PlayerData playerData = new PlayerData();
    Transform camera_main;
    Vector3 cameraspeed = Vector3.zero;
    float timer = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        waypoint.Add(new Vector3(0,10,0));
        waypoint.Add(new Vector3(63,10,140));
        waypoint.Add(new Vector3(150,10,60));
        automaticbtn = GameObject.Find("Canvas/Automatic").GetComponent<Button>();
        manualbtn = GameObject.Find("Canvas/Manual").GetComponent<Button>();
        automaticbtn.onClick.AddListener(() =>
        {
            isState = false;
            float dis = 1000;
            for (int i = 0; i < waypoint.Count; i++)
            {
                if (dis > Vector3.Distance(transform.position, waypoint[i]))
                {
                    dis = Vector3.Distance(transform.position, waypoint[i]);
                    index = i;
                }
            }
            isTurnacorner = true;
        });
        manualbtn.onClick.AddListener(() =>
        {
            isState = true;
        });
        camera_main = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        Pitchofpitch();
        Move();
        Level();
        CameraFollow();
        playerData.PosX = transform.position.x;
        playerData.PosY=transform.position.y;
        playerData.PosZ = transform.position.z;
        playerData.RotX= transform.eulerAngles.x;
        playerData.RotY= transform.eulerAngles.y;
        playerData.RotZ=transform.eulerAngles.z;
        playerData.IsState= isState;
        byte[] messageBytes = playerData.ToByteArray();
        if (NetManager.Instance.allclis.Count>0)
        {
            Debug.Log("发送消息");
            for (int i = 0; i < NetManager.Instance.allclis.Count; i++)
            {
                NetManager.Instance.OnSendToClient(Define.SC_REFRESH, messageBytes, NetManager.Instance.allclis[i]);
            }
        }
        Console.WriteLine();
    }


    void CameraFollow()
    {
        camera_main.transform.LookAt(transform.position);
        Vector3 targetPos = transform.position+ transform.up * 10-transform.forward*10;
        camera_main.transform.position = Vector3.SmoothDamp(camera_main.position,targetPos,ref cameraspeed,timer);
    }

    /// <summary>
    /// 横滚
    /// </summary>
    void Level()
    {
        if (!isState) return;
        float x = Input.GetAxis("Vertical");
        if (x > 0)
        {
            transform.eulerAngles = new Vector3(transform.eulerAngles.x + 1, transform.eulerAngles.y, transform.eulerAngles.z);
        }
        else if (x < 0)
        {
            transform.eulerAngles = new Vector3(transform.eulerAngles.x - 1, transform.eulerAngles.y, transform.eulerAngles.z);
        }
        Vector3 currentEulerAngles = transform.eulerAngles;
        float pitch = currentEulerAngles.x;
        if (pitch > 180)
        {
            pitch -= 360;
        }
        pitch = Mathf.Clamp(pitch, -30, 30);
        currentEulerAngles.x = pitch;
        if (currentEulerAngles.x < 0)
        {
            currentEulerAngles.x += 360;
        }
        transform.eulerAngles = currentEulerAngles;
    }

    /// <summary>
    /// 横滚
    /// </summary>
    void Pitchofpitch()
    {
        if (isState)
        {
            float y = Input.GetAxis("Horizontal");
            if (y > 0)
            {
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y+1, transform.eulerAngles.z - 1);
            }
            if (y < 0)
            {
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y-1, transform.eulerAngles.z + 1);
            }
        }
        Vector3 currentEulerAngles = transform.eulerAngles;
        float pitch = currentEulerAngles.z;
        if (pitch > 180)
        {
            pitch -= 360;
        }
        pitch = Mathf.Clamp(pitch, -30, 30);
        currentEulerAngles.z = pitch;
        if (currentEulerAngles.z < 0)
        {
            currentEulerAngles.z += 360;
        }
        transform.eulerAngles = currentEulerAngles;
    }

    /// <summary>
    /// 移动
    /// </summary>
    void Move()
    {
        speed = 3;
        if(isTurnacorner)
        {
            Vector3 v3 = waypoint[index] - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(v3);
            float rotationSpeed = 2f;
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation,Time.deltaTime * rotationSpeed);
            if(transform.rotation==targetRotation)
            {
                isTurnacorner = false;
            }
        }
        if (isState)
        {
            if (Input.GetKey(KeyCode.LeftShift)) { speed = 8; }
            if (Input.GetKey(KeyCode.Space)) { speed = 1; }
        }
        else
        {
            if (Vector3.Distance(waypoint[index],transform.position)<=5f)
            {
                index++;
                if(index>2)
                {
                    index = 0;
                }
                isTurnacorner = true;
            }
        }
        transform.Translate(Vector3.forward*Time.deltaTime*speed);
    }
}
