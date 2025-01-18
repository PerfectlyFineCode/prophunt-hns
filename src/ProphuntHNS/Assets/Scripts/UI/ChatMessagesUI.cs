using System;
using UnityEngine;
using UnityEngine.UIElements;

public class ChatMessagesUI : MonoBehaviour
{
	private UIDocument _uiDocument;
	
	private ListView _chatView;
	private Button _sendButton;
	private string _chatMessage;
	private TextField _chatMessageField;
	
	private void Awake()
	{
		_uiDocument = GetComponent<UIDocument>();
	}
	
	private void Start()
	{
		_chatView = _uiDocument.rootVisualElement.Q<ListView>("ChatView");
		
		_sendButton = _uiDocument.rootVisualElement.Q<Button>("SendBtn");
		_sendButton.clicked += SendButtonOnClicked;
		
		_chatMessageField = _uiDocument.rootVisualElement.Q<TextField>("ChatMessage");
		_chatMessageField.RegisterValueChangedCallback(evt =>
		{
			_chatMessage = evt.newValue;
		});
		
		_chatView.makeItem = () =>
		{
			var label = new Label();
			label.AddToClassList("chat-item");
			return label;
		};
		_chatView.bindItem = (element, i) =>
		{
			((Label)element).text = ChatMessenger.ChatMessages[i];
			
		};
		_chatView.itemsSource = ChatMessenger.ChatMessages;
	}

	private void SendButtonOnClicked()
	{
		ChatMessenger.SendChatMessage(_chatMessage);
	}

	private void OnEnable()
	{
		ChatMessenger.ChatMessageReceived += OnChatMessageReceived;
	}
	
	private void OnDisable()
	{
		ChatMessenger.ChatMessageReceived -= OnChatMessageReceived;
	}

	private void OnChatMessageReceived(string obj)
	{
		_chatView.RefreshItems();
		_chatView.ScrollToItem(_chatView.itemsSource.Count - 1);
	}
}