using Unity.Netcode;
using UnityEngine;

[DefaultExecutionOrder(0)] // Ensure this script runs before others
public class PlayerSpawnManager : NetworkBehaviour
{
    [SerializeField]
    private Vector3 _spawnPosition;
    
    /// <inheritdoc />
    public override void OnNetworkSpawn()
    {
        if (!IsServer)
        {
            enabled = false;
            return;
        }
        
        SpawnPlayer();
        base.OnNetworkSpawn();
    }
    
    private void SpawnPlayer()
    {
        transform.position = _spawnPosition;
    }
}
