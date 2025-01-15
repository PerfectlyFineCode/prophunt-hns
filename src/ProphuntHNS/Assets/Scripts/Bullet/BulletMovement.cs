using System;
using Unity.Netcode;
using UnityEngine;

public class BulletMovement : NetworkBehaviour
{
    private readonly RaycastHit[] _hits = new RaycastHit[5];
    
    /// <inheritdoc />
    public override void OnNetworkSpawn()
    {
        if (!IsServer)
        {
            enabled = false;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    private void FixedUpdate()
    {
        var previousPosition = transform.position;
        transform.position += transform.forward * (Time.deltaTime * 10);
        int result = Physics.RaycastNonAlloc(previousPosition,
            (previousPosition - transform.position).normalized,
            _hits, (previousPosition - transform.position).magnitude);

        if (result > 0)
        {
            OnBulletHit(result);
        }
        
    }

    private void OnBulletHit(int result)
    {
        
        Debug.Log("Bullet hit ");
    }
}
