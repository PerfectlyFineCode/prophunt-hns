using Unity.Netcode;
using UnityEngine;

public class PropSpawner : NetworkBehaviour
{
    [SerializeField]
    private Bounds _spawnArea;
    
    /// <inheritdoc />
    public override void OnNetworkSpawn()
    {
        if (!IsServer)
        {
            enabled = false;
        }
    }
    
    public void SpawnProp()
    {
        if (!IsServer) return;
        Vector3 spawnPosition = new(
            Random.Range(_spawnArea.min.x, _spawnArea.max.x),
            Random.Range(_spawnArea.min.y, _spawnArea.max.y),
            Random.Range(_spawnArea.min.z, _spawnArea.max.z)
        );
    }
}
