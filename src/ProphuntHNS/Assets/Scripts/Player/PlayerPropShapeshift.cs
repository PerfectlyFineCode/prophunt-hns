using System;
using Unity.Netcode;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPropShapeshift : NetworkBehaviour
{
    public NetworkVariable<uint> PropId = new();
    [SerializeField] private GameObject _originalModel;
    private NetworkObject _currentProp;
    
    private MeshFilter _originalFilter;
    private MeshRenderer _originalRenderer;
    private MeshCollider _originalCollider;
    private PlayerTeam _playerTeam;
    private CharacterController _characterController;
    private NetworkedPropObject _closestProp;

    private void Awake()
    {
        _originalFilter = _originalModel.GetComponent<MeshFilter>();
        _originalRenderer = _originalModel.GetComponent<MeshRenderer>();
        _originalCollider = _originalModel.GetComponent<MeshCollider>();
        _characterController = GetComponent<CharacterController>();
        _playerTeam = GetComponent<PlayerTeam>();
    }

    /// <inheritdoc />
    public override void OnNetworkSpawn()
    {
        if (!IsLocalPlayer)
        {
            enabled = false;
        }
    }

    private void FixedUpdate()
    {
        if (!IsLocalPlayer) return;
        if (_playerTeam.Team.Value != PlayerTeamKind.Hider) return;
        UpdateCheckClosestProp();
    }

    private void UpdateCheckClosestProp()
    {
        if (!IsLocalPlayer) return;
        if (_playerTeam.Team.Value != PlayerTeamKind.Hider) return;
        
        var props = FindObjectsByType<NetworkedPropObject>(FindObjectsSortMode.InstanceID);
        if (props.Length == 0) return;
        var oldClosestProp = _closestProp;
        _closestProp = props[0];
        foreach (var prop in props)
        {
            if (Vector3.Distance(transform.position, prop.transform.position) <
                Vector3.Distance(transform.position, _closestProp.transform.position))
            {
                _closestProp = prop;
            }
        }
        
        if (oldClosestProp != _closestProp)
        {
            Debug.Log($"Closest prop: {_closestProp.name}");
            _closestProp.Highlight();
            oldClosestProp?.Unhighlight();
        }
    }
    
    private void ChangePropServer(uint propId)
    {
        if (!IsServer) return;
        var prop = PropsManager.Instance.GetProp(propId);
        if (prop == null)
        {
            Debug.LogWarning("Prop not found");
            return;
        }
        
        MeshFilter filter = prop.GetComponent<MeshFilter>();
        
        FitCharacterControllerToMesh(filter.sharedMesh);
        
        ChangeMeshPropClientRpc(propId);
    }
    
    [ClientRpc]
    private void ChangeMeshPropClientRpc(uint propId)
    {
        ChangeMeshProp(propId);
    }
    
    private void ChangeMeshProp(uint propId)
    {
        var prop = PropsManager.Instance.GetProp(propId);
        if (prop == null)
        {
            Debug.LogWarning("Prop not found");
            return;
        }
        
        var _filter = prop.GetComponent<MeshFilter>();
        var _renderer = prop.GetComponent<MeshRenderer>();
        
        
        if (_filter != null && _renderer != null)
        {
            var originalFilter = _originalModel.GetComponent<MeshFilter>();
            var originalRenderer = _originalModel.GetComponent<MeshRenderer>();
            originalFilter.mesh = _filter.sharedMesh;
            originalRenderer.material = _renderer.sharedMaterial;
            _originalCollider.sharedMesh = _filter.sharedMesh;
            
            // change character controller dimensions to match new prop
            FitCharacterControllerToMesh(_filter.sharedMesh);
        }
    }
    
    private void FitCharacterControllerToMesh(Mesh mesh)
    {
        // radius of the mesh (cyllinder)
        _characterController.radius = 0.05f;
        _characterController.height = mesh.bounds.size.y;
        _characterController.center = new Vector3(0, mesh.bounds.size.y / 2, 0);
    }
    
    public void Shapeshift(uint propId)
    {
        ChangePropServerRpc(propId);
    }
    
    [ServerRpc]
    private void ChangePropServerRpc(uint propId)
    {
        PropId.Value = propId;
        ChangePropServer(propId);
    }

    public void OnShapeshift(InputValue inputValue)
    {
        if (!IsLocalPlayer) return;
        if (_playerTeam.Team.Value != PlayerTeamKind.Hider) return;
        if (_closestProp == null) return;
        Shapeshift(_closestProp.NetworkObject.PrefabIdHash);
    }

    #if !UNITY_SERVER
    private void OnGUI()
    {
        if (!IsLocalPlayer) return;
        if (_playerTeam.Team.Value == PlayerTeamKind.Seeker) return;
        if (GUI.Button(new Rect(Screen.width - 160, Screen.height - 160, 150, 50), "Shapeshift"))
        {
            Shapeshift(PropsManager.Instance.GetRandomProp().PrefabIdHash);
        }
    }
    #endif
}
