using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

[System.Serializable]
public class SavedMessage
{
    public string role;
    public string content;
}

[System.Serializable]
public class ConversationData
{
    public Dictionary<string, List<SavedMessage>> characters
        = new Dictionary<string, List<SavedMessage>>();
}

public static class ConversationStore
{
    private static string FilePath =>
        Path.Combine(Application.persistentDataPath, "conversations.json");

    //Load
    public static ConversationData Load()
    {
        if (!File.Exists(FilePath))
        {
            Debug.Log("No saved conversations found. Starting fresh.");
            return new ConversationData();
        }

        string json = File.ReadAllText(FilePath);
        ConversationData data = JsonConvert.DeserializeObject<ConversationData>(json);
        return data ?? new ConversationData();
    }

    //Save
    public static void Save(ConversationData data)
    {
        string json = JsonConvert.SerializeObject(data, Formatting.Indented);
        File.WriteAllText(FilePath, json);
        Debug.Log("Conversations saved to: " + FilePath);
    }

    //Get history for one character 
    public static List<SavedMessage> GetHistory(ConversationData data, string characterName)
    {
        if (!data.characters.ContainsKey(characterName))
            data.characters[characterName] = new List<SavedMessage>();

        return data.characters[characterName];
    }

    //Add a message and save
    public static void AddAndSave(ConversationData data, string characterName,
                                  string role, string content)
    {
        var history = GetHistory(data, characterName);
        history.Add(new SavedMessage { role = role, content = content });
        Save(data);
    }

    //Clear one character's history
    public static void ClearCharacter(ConversationData data, string characterName)
    {
        if (data.characters.ContainsKey(characterName))
        {
            data.characters[characterName] = new List<SavedMessage>();
            Save(data);
            Debug.Log($"Cleared history for {characterName}");
        }
    }

    // Clear ALL conversations + delete the file
    // Call this ONLY from the Main Menu "Start New Game" button
    public static void ClearAll()
    {
        if (File.Exists(FilePath))
        {
            File.Delete(FilePath);
            Debug.Log("Conversation file deleted — fresh start.");
        }
        else
        {
            Debug.Log("No conversation file to delete.");
        }
    }
}
