using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

// WHY DO I GET 'InvalidOperationException: Nullable object must have a value.' WHEN I TRY TO CHANGE SCENES ?????
public class GameSceneLoader : NetworkBehaviour
{
// #if UNITY_EDITOR
// 	public SceneAsset SceneAsset;
// 	private void OnValidate()
// 	{
// 		if (SceneAsset != null)
// 		{
// 			_sceneName = SceneAsset.name;
// 		}
// 	}
// #endif
// 	[SerializeField, HideInInspector]
// 	private string _sceneName;
//
// 	public override void OnNetworkSpawn()
// 	{
// 		if (!IsServer || string.IsNullOrEmpty(_sceneName)) return;
// 		var status = NetworkManager.SceneManager.LoadScene(_sceneName, LoadSceneMode.Additive);
// 		if (status != SceneEventProgressStatus.Started)
// 		{
// 			Debug.LogWarning($"Failed to load {_sceneName} with {nameof(SceneEventProgressStatus)}: {status}");
// 		}
// 	}
}