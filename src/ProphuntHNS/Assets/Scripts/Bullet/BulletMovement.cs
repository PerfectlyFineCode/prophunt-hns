using System;
using Unity.Netcode;
using UnityEngine;

public class BulletMovement : NetworkBehaviour
{
    private readonly RaycastHit[] _hits = new RaycastHit[5];
    
    public Vector3 Direction { get; set; }
    
    public NetworkObject Sender { get; set; }
    
    /// <inheritdoc />
    public override void OnNetworkSpawn()
    {
        if (!IsServer)
        {
            enabled = false;
        }
    }
    
    private float _currentLifeTime = 0;

    // Update is called once per frame
    private void Update()
    {
        if (!IsServer) return;
        _currentLifeTime += Time.deltaTime;
        if (_currentLifeTime > 5)
        {
            NetworkObject.Despawn();
        }
    }

    private void FixedUpdate()
    {
        Vector3 previousPosition = transform.position;
        transform.position += Direction * (Time.fixedTime * 0.05f);
        int result = Physics.RaycastNonAlloc(previousPosition,
            (previousPosition - transform.position).normalized,
            _hits, 
            (previousPosition - transform.position).magnitude
        );

        if (result > 0)
        {
            OnBulletHit(result);
        }
        
    }

    private void OnBulletHit(int result)
    {
        if (!IsServer) return;
        // Ignore hits on the sender
        bool hasHitNonSender = false;
        
        Debug.Log($"Bullet hit  {_hits[0].transform.name}");
        for (int i = 0; i < result; i++)
        {
            RaycastHit hit = _hits[i];
            if (hit.transform.parent != null 
                && hit.transform.parent.TryGetComponent(out PlayerHealth health)
                && health.NetworkObject != Sender)
            {
                health.TakeDamage(10);
                hasHitNonSender = true;
            }
        }
        
        if (hasHitNonSender)
        {
            NetworkObject.Despawn();
        }
    }
}