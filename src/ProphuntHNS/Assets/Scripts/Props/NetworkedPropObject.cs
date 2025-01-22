using System;
using Linework.Common.Attributes;
using Unity.Netcode;
using UnityEngine;


public class NetworkedPropObject : NetworkBehaviour
{
	[SerializeField, RenderingLayerMask] private int _highlightLayer;
	
	private Highlighter _highlighter;
	private void Awake()
	{
		_highlighter = gameObject.AddComponent<Highlighter>();
		_highlighter.SetHighlightLayer(_highlightLayer);
	}
	
	public void Highlight()
	{
		_highlighter.SetHighlight(true);
	}
	
	public void Unhighlight()
	{
		_highlighter.SetHighlight(false);
	}
}