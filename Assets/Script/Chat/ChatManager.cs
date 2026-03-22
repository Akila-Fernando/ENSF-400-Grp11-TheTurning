using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class ChatManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject messagePrefab;
    public Transform content;
    public TMP_InputField inputField;
    public ScrollRect scrollRect;

    [Header("References")]
    public APIManager apiManager;
    public Diaglouge dialogue;

    private string activeContact;

    private static readonly HashSet<string> scriptedCharacters = new HashSet<string>
    {
        "Alex"
    };

    private string[] alexLines = new string[]
    {
        "Hey! Nice to meet you.",
        "So what do you do?",
        "That's really interesting!",
        "Haha yeah I get that.",
        "Where are you from?",
        "Cool, I've always wanted to go there.",
        "What kind of music are you into?",
        "Same honestly, good taste.",
        "This is fun, we should chat more."
    };
    private int alexLineIndex = 0;
    void Start()
    {
        messagePrefab = Resources.Load<GameObject>("MessageBuble");

        // Default charachter is Sam If something bad happends

        activeContact = PlayerPrefs.GetString("ActiveContact", "Sam");

        if (string.IsNullOrEmpty(activeContact))
            activeContact = "Sam";

        Debug.Log("Active contact: " + activeContact);
        LoadChatHistoryToUI();
    }

    // Reload previous messages into the UI when returning to this scene
    void LoadChatHistoryToUI()
    {
        if (messagePrefab == null) return;

        var history = apiManager.GetHistoryForUI(activeContact);
        foreach (var msg in history)
        {
            string senderLabel = msg.role == "user" ? "You" : activeContact;
            AddMessageToUI(senderLabel, msg.content);
        }
    }

    public void OnSendPressed()
    {
        if (messagePrefab == null) return;

        string userMessage = inputField.text.Trim();
        if (string.IsNullOrEmpty(userMessage)) return;

        AddMessageToUI("You", userMessage);
        inputField.text = "";

        if (scriptedCharacters.Contains(activeContact))
            GetScriptedReply(activeContact);
        else
            GetAIReply(activeContact, userMessage);
    }

    void GetAIReply(string characterName, string userMessage)
    {
        apiManager.GetReply(characterName, userMessage, (reply) =>
        {
            AddMessageToUI(characterName, reply);
            ShowDialogue(reply);
        });
    }

    void GetScriptedReply(string characterName)
    {
        string reply = "";

        if (characterName == "Alex")
        {
            if (alexLineIndex >= alexLines.Length)
                alexLineIndex = 0;
            reply = alexLines[alexLineIndex];
            alexLineIndex++;
        }

        if (!string.IsNullOrEmpty(reply))
        {
            AddMessageToUI(characterName, reply);
            ShowDialogue(reply);
        }
    }

    void ShowDialogue(string reply)
    {
        if (dialogue == null) return;
        dialogue.SetLines(new string[] { reply });
        dialogue.gameObject.SetActive(true);
        dialogue.StartDialogue();
    }

    void AddMessageToUI(string sender, string text)
    {
        if (messagePrefab == null) return;

        GameObject msg = Instantiate(messagePrefab, content);
        TMP_Text tmp = msg.GetComponentInChildren<TMP_Text>();
        if (tmp != null)
            tmp.text = $"{sender}: {text}";

        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;
    }
}
