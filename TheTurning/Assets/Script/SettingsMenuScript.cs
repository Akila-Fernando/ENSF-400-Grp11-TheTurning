using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToMenuMenuScript : MonoBehaviour
{

    public void Choice_Easy()
    {
        // SetString(KeyName, Value) - stores the user's choice
        PlayerPrefs.SetString("CurrentDifficulty", "Easy");
        PlayerPrefs.Save(); // Saves user's choice

    }
    public void Choice_Normal()
    {
        PlayerPrefs.SetString("CurrentDifficulty", "Normal");
        PlayerPrefs.Save();

    }
    public void Choice_Hard()
    {
        PlayerPrefs.SetString("CurrentDifficulty", "Hard");
        PlayerPrefs.Save();

    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }
}