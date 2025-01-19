using Unity.Netcode;
using UnityEngine;

public class PlayerTeam : NetworkBehaviour
{
	public NetworkVariable<PlayerTeamKind> Team = new();

	/// <inheritdoc />
	public override void OnNetworkSpawn()
	{
		if (!IsServer) return;
		int players = NetworkManager.SpawnManager.PlayerObjects.Count;
		bool isHider = players % 5 != 0;
		AssignTeam(isHider ? PlayerTeamKind.Hider : PlayerTeamKind.Seeker);
	}
	
	private void AssignTeam(PlayerTeamKind team)
	{
		Team.Value = team;
	}
}