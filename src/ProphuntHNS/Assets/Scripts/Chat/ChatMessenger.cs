using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class ChatMessenger : NetworkBehaviour
{
    public static event Action<string> ChatMessageReceived;
    public static ObservableCollection<string> ChatMessages { get; } = new();
    
    private static readonly Queue<string> _sendQueue = new();
    [SerializeField] private int _maxMessages = 100;
    private int _count; // Interal counter for messages for testing
     
    public static void SendChatMessage(string message)
    {
        _sendQueue.Enqueue(message);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (IsServer) return;
        if (_sendQueue.Count == 0) return;
        while (_sendQueue.Count > 0)
        {
            var message = _sendQueue.Dequeue();
            Debug.Log($"Sending message: {message}");
            SendChatMessageRpc(message);
        }
    }
    
    [Rpc(SendTo.Server, Delivery = RpcDelivery.Reliable)]
    private void SendChatMessageRpc(string message)
    {
        if (!IsServer) return;
        _count++;
        Debug.Log($"Server received message: {message}");
        ReceiveChatMessageRpc($"{message}[{_count}]");
    }
    
    [Rpc(SendTo.NotServer, Delivery = RpcDelivery.Reliable)]
    private void ReceiveChatMessageRpc(string message)
    {
        if (IsServer) return;
        Debug.Log($"Client received message: {message}");
        if (ChatMessages.Count > _maxMessages)
        {
            ChatMessages.RemoveAt(0);
        }
        ChatMessages.Add(message);
        ChatMessageReceived?.Invoke(message);
    }
}
