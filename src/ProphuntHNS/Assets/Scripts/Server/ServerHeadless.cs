using System;
using Unity.Netcode;
using UnityEngine;

public class ServerHeadless : MonoBehaviour
{
    private NetworkManager _networkManager;
    
    private void Awake()
    {
        bool isServer = Application.isBatchMode;
        if (!isServer)
        {
            Destroy(this);
            return;
        }
        
        _networkManager = gameObject.GetComponent<NetworkManager>();
        _networkManager.StartServer();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
