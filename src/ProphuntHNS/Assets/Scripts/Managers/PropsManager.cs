using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Unity.Netcode;
using Unity.VisualScripting;
#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
using UnityEditor;
#endif
using UnityEngine;

public class PropsManager : NetworkBehaviour
{
    [SerializeField]
    public List<NetworkObject> PropsList = new();
    
    public Dictionary<uint, NetworkObject> Props = new();
    
    public static PropsManager Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
    }

    /// <inheritdoc />
    public override void OnNetworkSpawn()
    {
        Debug.Log("PropsManager spawned");
        InitializePropsSet();
    }
    
    public void InitializePropsSet()
    {
        if (Props.Count > 0) return;
        Props = PropsList
            .DistinctBy(x => x.PrefabIdHash)
            .ToDictionary(prop => prop.PrefabIdHash, o => o);
    }

    public NetworkObject GetRandomProp()
    {
        return Props.GetRandom().Value;
    }
    
    public NetworkObject GetProp(uint propId)
    {
        return Props[propId];
    }
}

#if UNITY_EDITOR
[UnityEditor.CustomEditor(typeof(PropsManager))]
public class PropsManagerEditor : OdinEditor
{
    private PropsManager _propsManager;

    /// <inheritdoc />
    protected override void OnEnable()
    {
        base.OnEnable();
        _propsManager = (PropsManager) target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Load props"))
        {
            FindProps();
        }
    }

    private void FindProps()
    {
        NetworkedPropObject[] props = LoadAllProps();
        if (props == null)
        {
            return;
        }
        
        var propsList = new List<NetworkObject>();
        foreach (NetworkedPropObject prop in props)
        {
            propsList.Add(prop.GetComponent<NetworkObject>());
        }
        
        _propsManager.PropsList.Clear();
        _propsManager.PropsList = propsList;
    }
    
    private static NetworkedPropObject[] LoadAllProps()
    {
        string[] guids = AssetDatabase.FindAssets("t:prefab", new []{ "Assets/Game/Prefabs/Props" });
        if (guids.Length == 0)
        {
            Debug.LogWarning($"No assets found in Assets/Game/Prefabs/Props");
            return null;
        }
        
        var assets = new NetworkedPropObject[guids.Length];
        for (int i = 0; i < guids.Length; i++)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
            assets[i] = AssetDatabase.LoadAssetAtPath<NetworkedPropObject>(assetPath);
        }
        
        return assets;
    }
}
#endif