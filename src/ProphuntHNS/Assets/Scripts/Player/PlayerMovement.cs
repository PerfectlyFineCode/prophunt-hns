using System;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerMovement : NetworkBehaviour
{
    private readonly NetworkVariable<Vector2> _movementInput = new(new Vector2(0, 0));
    private readonly NetworkVariable<Vector3> _moveDirection = new(new Vector3(0, 0, 0));
    private readonly NetworkVariable<float> _speed = new(5);
    
    private CameraFollow _cameraFollow;
    private Camera _camera;
    private CharacterController _characterController;
    private bool _isHoveringUI;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
    }


    /// <inheritdoc />
    public override void OnNetworkSpawn()
    {
        if (!IsLocalPlayer) return;
        _cameraFollow = FindFirstObjectByType<CameraFollow>();
        _camera = _cameraFollow.GetComponentInChildren<Camera>();
        CameraFollow.SetPlayer(this);
    }
    
    public void OnMove(InputValue value)
    {
        if (!IsLocalPlayer) return;
        if (_isHoveringUI) return;
        ChatMessenger.SendChatMessage("Player is moving");
        Vector2 movementInput = value.Get<Vector2>();
        var translatedMovementInput = MoveRelativeCamera(new Vector3(movementInput.x, 0, movementInput.y));
        MoveServerRpc(translatedMovementInput, translatedMovementInput);
    }

    private Vector3 MoveRelativeCamera(Vector3 delta)
    {
        Vector3 dir = _camera.transform.TransformDirection(delta);
        dir.y = 0;
        return dir.normalized * delta.magnitude;
    }

    private void Update()
    {
        UpdateOverUI();
        if (!IsServer) return;
        _characterController.SimpleMove(new Vector3(
            _moveDirection.Value.x * _speed.Value,
            0,
            _moveDirection.Value.z * _speed.Value
        ));
    }
    
    private void UpdateOverUI()
    {
        if (!IsLocalPlayer) return;
        _isHoveringUI = EventSystem.current.IsPointerOverGameObject();
    }

    [ServerRpc]
    private void MoveServerRpc(Vector2 inputDelta, Vector3 moveDirection)
    {
        _movementInput.Value = inputDelta.normalized;
        _moveDirection.Value = moveDirection.normalized;
    }
}
