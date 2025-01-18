using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerShooting : NetworkBehaviour
{
	public NetworkVariable<bool> IsShooting = new();
	public NetworkVariable<float> FireRate = new(0.5f);
	private float _previousTime;
	private Camera _camera;
	private Vector3 _mousePlanePosition;
	private bool _isHoveringUI;
	
	private Plane _plane = new Plane(Vector3.up, Vector3.zero);

	[SerializeField]
	private NetworkObject _bulletPrefab;

	private void Awake()
	{
		_camera = Camera.main;
	}

	public void ChangeShootingState(bool isShooting)
	{
		if (!IsLocalPlayer) return;
		RequestShootServerRpc(isShooting);
	}
	
	[ServerRpc]
	public void RequestShootServerRpc(bool isShooting)
	{
		if (!IsServer) return;
		IsShooting.Value = isShooting;
	}

	private void Update()
	{
		UpdateOverUI();
		ClientPlanecasting();
		ServerShooting();
	}
	
	private void UpdateOverUI()
	{
		if (!IsLocalPlayer) return;
		_isHoveringUI = EventSystem.current.IsPointerOverGameObject();
	}
	
	private void ClientPlanecasting()
	{
		if (!IsLocalPlayer) return;
		Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
		if (_plane.Raycast(ray, out float distance))
		{
			_mousePlanePosition = ray.GetPoint(distance);
		}
	}

	private void ServerShooting()
	{
		if (!IsServer) return;
		if (!IsShooting.Value) return;
		if (Time.time - _previousTime < FireRate.Value) return;
		_previousTime = Time.time;
		SpawnBullet();
	}
	
	private void SpawnBullet()
	{
		if (!IsServer) return;
		// Spawn bullet
		NetworkObject networkObject = NetworkManager.SpawnManager.InstantiateAndSpawn(_bulletPrefab, position:
			transform.position + transform.forward,
			rotation: transform.rotation);
	}

	public void OnShoot(InputValue value)
	{
		if (_isHoveringUI && value.isPressed) return;
		ChangeShootingState(value.isPressed);
	}
}
