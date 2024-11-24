using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }
    public CanvasGroup dialogueCanvas;
    public GameObject dialogueContainer;
    public TextMeshProUGUI characterNameText;
    public TextMeshProUGUI dialogueText;
    public Image characterSprite1;
    public Image characterSprite2;
    public Image indicatorSprite;

    public float typingSpeed = 0.05f;
    public float indicatorSpeed = 0.5f;

    private bool isDialogueActive = false;
    private bool isWaitingForInput = false;
    private Sprite lastSprite1 = null;
    private Sprite lastSprite2 = null;
    private bool isEmrysSpeaking = false;
    private string lastDialogueID = "";
    private Dictionary<string, DialogueData> dialogueEvents = new Dictionary<string, DialogueData>();
    public event Action<bool> OnDialogueStateChanged;

private void Awake()
{
    if (Instance == null)
    {
        Instance = this;
    }
    else
    {
        Destroy(gameObject);
        Debug.LogWarning("Duplicate DialogueManager instance destroyed.");
    }
}

    
    public bool GetisActiveDialogue(){
        return isDialogueActive;
    }
    public bool GetEmrysSpeak(){
        return isEmrysSpeaking;
    }

    public void RegisterDialogue(string id, DialogueData dialogueData)
    {
        if (!dialogueEvents.ContainsKey(id))
        {
            dialogueEvents.Add(id, dialogueData);
        }
    }
    public void TriggerDialogue(string id)
    {
        lastDialogueID = id;
        if (dialogueEvents.TryGetValue(id, out DialogueData dialogueData))
        {
            StartDialogue(dialogueData);
        }
        else
        {
            Debug.LogError("dialog not found");
        }
    }
    public void StartDialogue(DialogueData dialogueData)
    {
        if (isDialogueActive) return;


        lastSprite1 = null;
        lastSprite2 = null;
        characterSprite1.sprite = null;
        characterSprite2.sprite = null;
        characterSprite1.CrossFadeAlpha(0f, 0f, true);
        characterSprite2.CrossFadeAlpha(0f, 0f, true);
        characterNameText.text = "";
        dialogueText.text = "";

        StartCoroutine(FadeCanvas(true));
        StartCoroutine(PlayDialogue(dialogueData));
    }

    private IEnumerator PlayDialogue(DialogueData dialogueData)
    {
        isDialogueActive = true;
        dialogueContainer.SetActive(true);

        foreach (var line in dialogueData.dialogueLines)
        {
            characterNameText.text = line.characterName;
            if (line.characterName == "Emrys" || line.characterName == "????")
            {
                isEmrysSpeaking = true;
                OnDialogueStateChanged?.Invoke(isDialogueActive);
            }
            yield return StartCoroutine(UpdateSprites(line.characterSprite1, line.characterSprite2));
            yield return StartCoroutine(TypeText(line.text));
            yield return StartCoroutine(FadeIndicator(true));
            isWaitingForInput = true;
            yield return new WaitUntil(() => Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space));
            isWaitingForInput = false;
            yield return StartCoroutine(FadeIndicator(false));
        }

            if (lastDialogueID == "Level_good_ending")
                {
                    Debug.Log("good ending");
                    SceneManager.LoadScene("HappyEnding");
                }
                else if (lastDialogueID == "Level_bad_ending")
                {
                    Debug.Log("bad ending");
                    SceneManager.LoadScene("BadEnding");
                }

        dialogueContainer.SetActive(false);
        yield return StartCoroutine(FadeCanvas(false));



        isDialogueActive = false;
        isEmrysSpeaking = false;
                
        OnDialogueStateChanged?.Invoke(isDialogueActive);
    }


    private IEnumerator TypeText(string text)
    {
        dialogueText.text = "";
        foreach (char letter in text)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    private IEnumerator FadeCanvas(bool fadeIn)
    {
        float targetAlpha = fadeIn ? 1.0f : 0.0f;
        while (!Mathf.Approximately(dialogueCanvas.alpha, targetAlpha))
        {
            dialogueCanvas.alpha = Mathf.MoveTowards(dialogueCanvas.alpha, targetAlpha, Time.deltaTime * 3);
            yield return null;
        }
    }

    private IEnumerator UpdateSprites(Sprite sprite1, Sprite sprite2)
    {
        if (sprite1 != lastSprite1)
        {
            yield return StartCoroutine(FadeSprite(characterSprite1, sprite1));
            lastSprite1 = sprite1;
        }
        if (sprite2 != lastSprite2)
        {
            yield return StartCoroutine(FadeSprite(characterSprite2, sprite2));
            lastSprite2 = sprite2;
        }
    }

    private IEnumerator FadeSprite(Image spriteImage, Sprite newSprite)
    {
        if (newSprite == null)
        {
            spriteImage.CrossFadeAlpha(0f, 0.5f, false);
            yield return new WaitForSeconds(0.5f);
            spriteImage.sprite = null;
        }
        else
        {
            if (spriteImage.sprite == null) spriteImage.CrossFadeAlpha(0f, 0f, true);
            spriteImage.sprite = newSprite;
            spriteImage.CrossFadeAlpha(1f, 0.5f, false );
            yield return new WaitForSeconds(0.5f);
        }
    }

    private IEnumerator FadeIndicator(bool fadeIn)
    {
        float targetAlpha = fadeIn ? 1f : 0f;
        Color color = indicatorSprite.color;
        while(!Mathf.Approximately(color.a, targetAlpha))
        {
            color.a = Mathf.MoveTowards(color.a, targetAlpha, Time.deltaTime * 3);
            indicatorSprite.color = color;
            yield return null;
        }
    }

}
