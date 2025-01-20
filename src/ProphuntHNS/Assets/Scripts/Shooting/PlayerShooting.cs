using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerShooting : NetworkBehaviour
{
	public NetworkVariable<bool> IsShooting = new();
	public NetworkVariable<float> FireRate = new(0.5f);
	public NetworkVariable<Vector3> ShootDirection = new();
	
	[SerializeField] private NetworkObject _bulletPrefab;
	
	private float _previousTime;
	private Camera _camera;
	private Vector3 _mousePlanePosition;
	private bool _isHoveringUI;
	private PlayerTeam _playerTeam;
	private Plane _plane = new Plane(Vector3.up, Vector3.zero);

	private void Awake()
	{
		_camera = Camera.main;
		_playerTeam = GetComponent<PlayerTeam>();
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
		if (_playerTeam.Team.Value != PlayerTeamKind.Seeker) return; // Only seekers can shoot
		IsShooting.Value = isShooting;
	}

	private void Update()
	{
		UpdateOverUI();
		ClientPlanecasting();
		UpdateShootDirection();
		ServerShooting();
	}

	private void UpdateShootDirection()
	{
		var direction = _mousePlanePosition - transform.position;
		direction.y = 0;
		if (direction == Vector3.zero) return;
		RequestChangeShootingDirectionServerRpc(direction);
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
	
	[ServerRpc]
	public void RequestChangeShootingDirectionServerRpc(Vector3 direction)
	{
		if (!IsServer) return;
		ShootDirection.Value = direction.normalized;
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
		Vector3 direction = ShootDirection.Value.normalized;
		
		// Spawn bullet
		NetworkObject networkObject = NetworkManager.SpawnManager.InstantiateAndSpawn(_bulletPrefab, position:
			transform.position + direction,
			rotation: transform.rotation);
		
		BulletMovement bulletMovement = networkObject.GetComponent<BulletMovement>();
		bulletMovement.Sender = NetworkObject; // Ignore hits on the sender
		bulletMovement.Direction = direction; // Set bullet direction
	}

	public void OnShoot(InputValue value)
	{
		if (_playerTeam.Team.Value != PlayerTeamKind.Seeker) return;
		if (_isHoveringUI && value.isPressed) return;
		ChangeShootingState(value.isPressed);
	}
}