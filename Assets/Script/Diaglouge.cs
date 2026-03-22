using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;

public class Diaglouge : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public string[] lines;
    public float textSpeed;
    private int index;

    void Start()
    {
        if (textComponent == null)
        {
            Debug.LogError("Diaglouge: textComponent is not assigned in the Inspector!");
            return;
        }

        gameObject.SetActive(false);
        textComponent.text = string.Empty;
    }

    public void StartDialogue()
    {
        if (textComponent == null) return;

        index = 0;
        textComponent.text = string.Empty;
        if (lines != null && lines.Length > 0)
        {
            StartCoroutine(TypeLine());
        }
    }

    public void SetLines(string[] newLines)
    {
        lines = newLines;
    }

    IEnumerator TypeLine()
    {
        foreach (char c in lines[index].ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    void Update()
    {
        if (textComponent == null) return;
        if (lines == null || lines.Length == 0) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (textComponent.text == lines[index])
            {
                NextLine();
            }
            else
            {
                StopAllCoroutines();
                textComponent.text = lines[index];
            }
        }
    }

    void NextLine()
    {
        if (index < lines.Length - 1)
        {
            index++;
            textComponent.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}