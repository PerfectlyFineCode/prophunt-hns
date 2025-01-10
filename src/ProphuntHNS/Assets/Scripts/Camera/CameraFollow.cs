using System;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform _player;
    
    [SerializeField] private float _smoothSpeed = 0.5f;
    [SerializeField] private Vector3 _offset;
    
    private static CameraFollow _instance;
    
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public static void SetPlayer(GameObject player)
    {
        _instance._player = player.transform;
    }
    
    private void LateUpdate()
    {
        if (_player == null) return;
        transform.position = Vector3.Lerp(transform.position, 
            new Vector3(_player.position.x, transform.position.y, _player.position.z) + _offset, 
            Time.deltaTime * _smoothSpeed);
    }
}
