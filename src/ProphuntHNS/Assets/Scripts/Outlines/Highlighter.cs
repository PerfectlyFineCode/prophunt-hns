using Linework.Common.Attributes;
using Sirenix.OdinInspector;
using UnityEngine;

public class Highlighter : MonoBehaviour
{
    [SerializeField, RenderingLayerMask] private int _highlightLayer;
    private Renderer[] _renderers;
    private uint _originalLayer;

    private void Awake()
    {
        _renderers = TryGetComponent<Renderer>(out var meshRenderer)  
            ? new[] {meshRenderer}  
            : GetComponentsInChildren<Renderer>();
        
        _originalLayer = _renderers[0].renderingLayerMask;
    }

    public void SetHighlight(bool highlighted)
    {
        foreach (Renderer mesh in _renderers)
        {
            mesh.renderingLayerMask = (uint)(_originalLayer | (_highlightLayer & (highlighted ? ~0u : 0u)));
        }
    }

    public void SetHighlightLayer(int highlightLayer)
    {
        _highlightLayer = highlightLayer;
    }
}
