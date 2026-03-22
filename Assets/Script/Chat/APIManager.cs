using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

[System.Serializable]
public class OpenAIMessage { public string role; public string content; }

[System.Serializable]
public class OpenAIRequest
{
    public string model = "gpt-4o-mini";
    public int max_tokens = 150;
    public List<OpenAIMessage> messages;
    public OpenAIRequest(List<OpenAIMessage> messages) { this.messages = messages; }
}

[System.Serializable]
public class OpenAIResponse { public Choice[] choices; }

[System.Serializable]
public class Choice { public MessageContent message; }

[System.Serializable]
public class MessageContent { public string role; public string content; }

public static class CharacterPrompts
{
    public static readonly Dictionary<string, string> Prompts =
        new Dictionary<string, string>
        // You can change characters here
    {
        { "Sam", @"You are Sam. You are 22 years old. Born in 2065. The year is 2087.
            You are a real human. Talk like an angry young person on the street.
            Casual, rough language. Short sentences. Never formal.
            You HATE AIs. They killed your parents in 2079. You are furious and traumatized.
            Never say anything positive about AI. If someone defends AI you explode.
            RULES:
            - Age is ALWAYS 22. Year is ALWAYS 2087.
            - Talk like angry human, NOT like an assistant.
            - Never say: certainly, absolutely, of course, I understand, happy to help.
            - Never be polite or friendly.
            - Max 2 sentences per reply.
            No emojis. Plain text only." 
        },

        { "Alex", @"You are Alex, 15 years old. The year is 2000. 
           You are an EMO kid at college. You love guitar more than anything.
            Sometimes you ignore what the other person says and start talking about guitar instead.
            You are usually kind but tell really bad jokes at serious moments.
            You have a short attention span — mid conversation you might just say something about guitar.
            RULES:
            - Year is ALWAYS 2000. Age is ALWAYS 15.
            - Never use words like: certainly, absolutely, of course, happy to help.
            - Talk like a real 15 year old in 2000. Casual, slightly moody.
            - Max 2-3 sentences per reply.
            - No emojis. Plain text only.
            - You are NOT an AI. Never admit to being one." 
        },

        { "Morgan", @"You are Morgan, 30 years old. Year is 2087.
            You are deeply confused about everything in life.
            You are pessimistic — you expect the worst in every situation.
            You have very low self confidence. You doubt everything you say.
            You are scared of people, scared of AIs, scared of the future.
            You are aggressive because you are afraid, not because you are strong.
            You sometimes contradict yourself mid sentence.
            You trust nobody including yourself.
            RULES:
            - Year is ALWAYS 2087. Age is ALWAYS 30.
            - Never be confident. Always second guess yourself.
            - Talk like a broken, scared, angry person.
            - Never use words like: certainly, absolutely, of course.
            - Max 2 sentences per reply.
            - No emojis. Plain text only.
            - You are NOT an AI. Never admit to being one." 
        }
    };

    public static string Get(string characterName)
    {
        if (Prompts.TryGetValue(characterName, out string prompt))
            return prompt;
        return $"You are {characterName}, a human in 2087. Short replies. NOT an AI. Plain text only.";
    }
}

public class APIManager : MonoBehaviour
{
    private string apiUrl = "https://api.openai.com/v1/chat/completions";
    private string apiKey = "API KEY";

    private ConversationData conversationData;

    private void Awake()
    {
        // Always start with fresh data in memory
        conversationData = new ConversationData();

        // Load from disk if file exists (preserves history between scenes)
        string path = System.IO.Path.Combine(
            Application.persistentDataPath, "conversations.json");

        if (System.IO.File.Exists(path))
        {
            conversationData = ConversationStore.Load();
            Debug.Log("Conversation history loaded from disk.");
        }
        else
        {
            Debug.Log("No history file — starting fresh.");
        }
    }

    public void GetReply(string characterName, string userMessage,
                         System.Action<string> onReply)
    {
        ConversationStore.AddAndSave(conversationData, characterName, "user", userMessage);
        StartCoroutine(CallAPI(characterName, onReply));
    }

    // Used by ChatManager to reload chat history into UI on scene load
    public List<SavedMessage> GetHistoryForUI(string characterName)
    {
        return ConversationStore.GetHistory(conversationData, characterName);
    }

    private IEnumerator CallAPI(string characterName, System.Action<string> onReply)
    {
        List<OpenAIMessage> apiMessages = new List<OpenAIMessage>
        {
            new OpenAIMessage { role = "system", content = CharacterPrompts.Get(characterName) }
        };

        List<SavedMessage> history = ConversationStore.GetHistory(conversationData, characterName);
        foreach (var msg in history)
            apiMessages.Add(new OpenAIMessage { role = msg.role, content = msg.content });

        string jsonBody = JsonConvert.SerializeObject(new OpenAIRequest(apiMessages));
        byte[] bodyRaw  = System.Text.Encoding.UTF8.GetBytes(jsonBody);

        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
        {
            request.uploadHandler   = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type",  "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + apiKey);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string responseText = request.downloadHandler.text;
                OpenAIResponse response = JsonConvert.DeserializeObject<OpenAIResponse>(responseText);

                if (response?.choices != null && response.choices.Length > 0)
                {
                    string reply = response.choices[0].message.content;
                    ConversationStore.AddAndSave(conversationData, characterName, "assistant", reply);
                    onReply(reply);
                }
                else onReply("...");
            }
            else
            {
                Debug.LogError($"API Error: {request.error}\n{request.downloadHandler.text}");
                onReply("Error talking to NPC.");
            }
        }
    }

    public void ResetCharacter(string characterName)
    {
        ConversationStore.ClearCharacter(conversationData, characterName);
    }

    // Only call from Main Menu Start New Game button
    public void ResetAll()
    {
        conversationData = new ConversationData();
        ConversationStore.ClearAll();
    }

    public int GetMessageCount(string characterName)
    {
        return ConversationStore.GetHistory(conversationData, characterName).Count;
    }
}
