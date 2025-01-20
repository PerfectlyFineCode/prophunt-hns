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
        var id = NetworkBehaviourId;
        // use ID as seed for random position
        var random = new System.Random(id);
        
        _spawnPosition += new Vector3(
            random.Next(-20, 20),
            0,
            random.Next(-20, 20)
        );
        
        transform.position = _spawnPosition;
    }
}
