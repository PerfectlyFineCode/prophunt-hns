using System;
using Unity.Netcode;

public enum GameStateKind
{
	MainMenu,
	Playing,
	Paused,
	GameOver
}

public class GameState
{
	public static GameStateKind CurrentState { get; private set; } = GameStateKind.MainMenu;
	
	public static event Action<GameStateKind> GameStateChanged;
	
	public static void SetState(GameStateKind state)
	{
		CurrentState = state;
		OnGameStateChanged(state);
	}
	
	private static void OnGameStateChanged(GameStateKind obj)
	{
		GameStateChanged?.Invoke(obj);
	}
}