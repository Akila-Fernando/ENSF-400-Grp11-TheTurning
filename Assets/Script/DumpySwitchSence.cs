using UnityEngine;
using UnityEngine.SceneManagement;

public class DumpySwitchSence : MonoBehaviour
{

    public void OnEasyModePressed()
    {
        PlayerPrefs.SetString("ActiveContact", "Sam");
        PlayerPrefs.Save();
        SceneManager.LoadScene("Easy Mode");
    }

    public void OnNormalModePressed()
    {
        PlayerPrefs.SetString("ActiveContact", "Alex");
        PlayerPrefs.Save();
        SceneManager.LoadScene("Normal Mode");
    }

    public void OnHardModePressed()
    {
        PlayerPrefs.SetString("ActiveContact", "Morgan");
        PlayerPrefs.Save();
        SceneManager.LoadScene("Hard Mode");
    }

    public void GoBackToMain()
    {
        //SceneManager.LoadScene("Main Menu");
        SceneManager.LoadScene("Dumny Select");
    }

 
    // Connect this to your Start New Game button on Main Menu ONLY
    public void StartNewGame()
    {
        ConversationStore.ClearAll();
        PlayerPrefs.DeleteKey("ActiveContact");
        PlayerPrefs.Save();
        SceneManager.LoadScene("Dummy Select");
    }
}
