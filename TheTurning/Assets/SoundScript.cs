using Unity.VisualScripting;
using UnityEngine;

public class SoundScript : MonoBehaviour
{
    public static SoundScript instance;
    private AudioSource audioSource;

    [Header("Sound Effects")]
    public AudioClip buttonClickSound;

    private void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(this.gameObject);
            instance = this;
            audioSource = GetComponent<AudioSource>();
        }
        else
        {
            Destroy(gameObject); // to avoid duplicate
        }
    }

    // play button click sound
    public void PlayButtonClick()
    {
        if (buttonClickSound != null)
        {
            audioSource.PlayOneShot(buttonClickSound);
        }
    }
}