using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : NetworkBehaviour
{
    private readonly NetworkVariable<Vector2> _movementInput = new(new Vector2(0, 0));


    /// <inheritdoc />
    public override void OnNetworkSpawn()
    {
        if (!IsLocalPlayer) return;
        CameraFollow.SetPlayer(gameObject);
    }

    private void OnMove(InputValue value)
    {
        if (!IsLocalPlayer) return;
        Vector2 movementInput = value.Get<Vector2>();
        MoveServerRpc(movementInput);
    }

    private void Update()
    {
        if (!IsServer) return;
        Vector3 movementDelta = new(_movementInput.Value.x, 0, _movementInput.Value.y);
        transform.position += movementDelta * Time.deltaTime;
    }
    
    [ServerRpc]
    private void MoveServerRpc(Vector2 movementDelta)
    {
        _movementInput.Value = movementDelta;
    }
}
