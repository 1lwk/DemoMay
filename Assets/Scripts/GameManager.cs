using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject plane;
    private void Awake()
    {
        NetManager.Instance.InitServer();
    }
    // Start is called before the first frame update
    void Start()
    {
        plane.AddComponent<PlaneController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
