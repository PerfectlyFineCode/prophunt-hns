using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : NetworkBehaviour
{
	public NetworkVariable<bool> IsShooting = new();
	public NetworkVariable<float> FireRate = new(0.5f);
	private float _previousTime;
	
	[SerializeField]
	private NetworkObject _bulletPrefab;
	
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
		ChangeShootingState(value.isPressed);
	}
}
