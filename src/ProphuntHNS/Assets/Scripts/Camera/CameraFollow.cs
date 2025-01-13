using System;
using Unity.Netcode;
using UnityEngine;

public class CameraFollow : NetworkBehaviour
{
    private Transform _player;
    
    [SerializeField] private float _smoothSpeed = 0.5f;
    
    private static CameraFollow _instance;

    /// <inheritdoc />
    public override void OnNetworkSpawn()
    {
        if (!IsLocalPlayer)
        {
            Destroy(this);
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public static void SetPlayer(GameObject player)
    {
        _instance._player = player.transform;
    }
    
    private void Update()
    {
        if (_player == null) return;
        transform.position = Vector3.Lerp(transform.position, 
            _player.position, 
            Time.deltaTime * _smoothSpeed);
    }
}
