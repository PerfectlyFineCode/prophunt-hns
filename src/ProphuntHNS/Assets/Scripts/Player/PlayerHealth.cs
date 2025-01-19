using System;
using Unity.Netcode;

public class PlayerHealth : NetworkBehaviour
{
	public readonly NetworkVariable<float> Health = new(100f);

	public event Action<bool> Died;

	private bool IsDead => Health.Value <= 0f;
	
	public bool TakeDamage(float damage)
	{
		if (!IsServer) return false;
		InternalTakeDamage(damage);
		return true;
	}

	private void InternalTakeDamage(float damage)
	{
		if (!IsServer) return;
		if (IsDead) return;
		Health.Value -= damage;
		if (Health.Value <= 0)
		{
			Health.Value = 0;
			Die();
		}
	}
	
	private void Die()
	{
		// Implement death logic here
		OnDied(true);
	}

	protected virtual void OnDied(bool obj)
	{
		Died?.Invoke(obj);
	}
}