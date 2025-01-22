using System;
using Unity.Netcode;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Random = UnityEngine.Random;

public class PropSpawner : NetworkBehaviour
{
    [HideInInspector] public Bounds SpawnArea = new(Vector3.zero, new Vector3(10, 0, 10));
    [SerializeField] private int _propsToSpawn = 10;
    private PropsManager _propsManager;
    
    private void Awake()
    {
        _propsManager = FindFirstObjectByType<PropsManager>();
    }
    
    /// <inheritdoc />
    public override void OnNetworkSpawn()
    {
        if (!IsServer)
        {
            enabled = false;
            return;
        }
        
        _propsManager.InitializePropsSet();
        PopulateProps();
    }
    
    private void PopulateProps()
    {
        for (int i = 0; i < _propsToSpawn; i++)
        {
            SpawnRandomProp();
        }
    }
    
    public void SpawnRandomProp()
    {
        if (!IsServer) return;
        if (_propsManager.Props.Count == 0)
        {
            Debug.LogWarning("No props found");
            return;
        }

        NetworkObject prop = _propsManager.Props.GetRandom().Value;
        
        Vector3 spawnPosition = new(
            Random.Range(SpawnArea.min.x, SpawnArea.max.x),
            SpawnArea.center.y,
            Random.Range(SpawnArea.min.z, SpawnArea.max.z)
        );
        
        // raycast down
        if (Physics.Raycast(spawnPosition + Vector3.up * 10, 
                Vector3.down,
                out RaycastHit hit))
        {
            spawnPosition = hit.point;
        }
        
        NetworkObject spawnedProp = Instantiate(prop, spawnPosition, Quaternion.identity);
        spawnedProp.Spawn();
    }
}

#if UNITY_EDITOR

[UnityEditor.CustomEditor(typeof(PropSpawner))]
public class PropSpawnerEditor : UnityEditor.Editor
{
    private PropSpawner _propSpawner;
    
    private void OnEnable()
    {
        _propSpawner = (PropSpawner) target;
    }
    
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Calculate area"))
        {
            CalculateArea();
        }
    }
    
    private void CalculateArea()
    {
        Bounds bounds = new(Vector3.zero, Vector3.zero);
        foreach (Renderer renderer in FindObjectsByType<Renderer>(FindObjectsSortMode.None))
        {
            bounds.Encapsulate(renderer.bounds);
        }
        _propSpawner.SpawnArea = bounds;
        
        EditorUtility.SetDirty(_propSpawner);
    }
    
    private void OnSceneGUI()
    {
        Handles.color = Color.green;
        Handles.DrawWireCube(_propSpawner.SpawnArea.center, _propSpawner.SpawnArea.size);
    }
}

#endif