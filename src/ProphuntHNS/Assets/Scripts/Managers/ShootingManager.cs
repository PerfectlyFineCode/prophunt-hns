// https://discussions.unity.com/t/distributed-authority-and-shared-network-object-pools/1583127/7 - Distributed Authority and Shared Network Object Pools

using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

public class ShootingManager : NetworkBehaviour
{
    [SerializeField]
    private NetworkObject _bulletPrefab;
    
    #region Singleton
    public static ShootingManager Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
    }
    
    #endregion
        
    private void SpawnBullet(PlayerShooting owner)
    {
        // Spawn bullet
        var networkObject = NetworkManager.SpawnManager.InstantiateAndSpawn(_bulletPrefab);
    }
}
