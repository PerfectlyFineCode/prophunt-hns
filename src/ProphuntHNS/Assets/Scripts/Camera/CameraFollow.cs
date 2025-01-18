using System;
using LitMotion;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class CameraFollow : MonoBehaviour
{
    private Transform _player;
    
    [SerializeField] private InputActionAsset _inputActions;
    [SerializeField] private float _smoothSpeed = 0.5f;
    [SerializeField] private float _rotationSpeed = 5f;
    [SerializeField] private float _rotationAngle = 0f;
    [SerializeField] private float _zoomSpeed = 5f;
    [SerializeField] private float _zoomDistance = 5f;
    
    private bool _isRotating = false;
    private bool _isHoveringUI;
    private Camera _camera;
    
    private static CameraFollow _instance;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            _camera = GetComponentInChildren<Camera>();
            _zoomDistance = _camera.orthographicSize;
        }
    }

    private void OnEnable()
    {
        _inputActions.Enable();
        _inputActions["Zoom"].performed += OnZoom;
        _inputActions["RotateLeft"].performed += OnRotateLeft;
        _inputActions["RotateRight"].performed += OnRotateRight;
    }
    
    private void OnDisable()
    {
        _inputActions.Disable();
        _inputActions["Zoom"].performed -= OnZoom;
        _inputActions["RotateLeft"].performed -= OnRotateLeft;
        _inputActions["RotateRight"].performed -= OnRotateRight;
    }

    public static void SetPlayer(PlayerMovement player)
    {
        _instance._player = player.transform;
    }
    
    private void Update()
    {
        UpdateOverUI();
        if (_player == null) return;
        transform.position = Vector3.Lerp(transform.position, 
            _player.position, 
            Time.deltaTime * _smoothSpeed);

        transform.rotation = Quaternion.Euler(0, _rotationAngle, 0);
        
        _camera.orthographicSize = Mathf.Lerp(_camera.orthographicSize, 
            _zoomDistance,
            Time.deltaTime * _zoomSpeed);
    }
    
    private void UpdateOverUI()
    {
        _isHoveringUI = EventSystem.current.IsPointerOverGameObject();
    }

    public void OnRotateLeft(InputAction.CallbackContext callbackContext)
    {
        Debug.Log("Rotate left");
        if (_isRotating || _isHoveringUI) return;
        _isRotating = true;
        LMotion.Create(_rotationAngle, (_rotationAngle - 90) % 360.0f, 1f)
            .WithOnComplete(() => _isRotating = false)
            .Bind(v => _rotationAngle = v)
            .AddTo(gameObject);
    }
    
    public void OnRotateRight(InputAction.CallbackContext callbackContext)
    {
        Debug.Log("Rotate right");
        if (_isRotating || _isHoveringUI) return;
        _isRotating = true;
        LMotion.Create(_rotationAngle, (_rotationAngle + 90) % 360.0f, 1f)
            .WithOnComplete(() => _isRotating = false)
            .Bind(v => _rotationAngle = v)
            .AddTo(gameObject);
    }
    
    public void OnZoom(InputAction.CallbackContext callbackContext)
    {
        if (_isHoveringUI) return;
        Debug.Log("Zoom");
        var zoom = callbackContext.ReadValue<float>();
        if (zoom < 0)
        {
            _zoomDistance += 0.25f;
        } 
        else
        {
            _zoomDistance -= 0.25f;
        }
        
        _zoomDistance = Mathf.Clamp(_zoomDistance, 1, 10);
    }
}
