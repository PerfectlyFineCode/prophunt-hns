using System;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class ServerHeadless : MonoBehaviour
{
    private NetworkManager _networkManager;
    private UnityTransport _transport;
    
    private void Awake()
    {
        _networkManager = gameObject.GetComponent<NetworkManager>();
        _transport = GetComponent<UnityTransport>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        #if SERVER
            _networkManager.StartServer();
            Debug.Log("Server started");
        #else
            _networkManager.StartClient();
            Debug.Log("Client started");
        #endif
        
    }
}
