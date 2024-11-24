using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering.Universal;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public class HintGroup
{
    public GameObject mainHint;
    public List<GameObject> subHints = new List<GameObject>();
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private EventTrigger eventTrigger;
    [SerializeField] private GameObject emrysCharacter;
    [SerializeField] private GameObject player;
    [SerializeField] private Collider2D finishLine;
    [SerializeField] private List<GameObject> panelsToCheck; 
    [SerializeField] private List<HintGroup> hintGroupsModeOpenWorld;
    [SerializeField] private AudioSource audioSource;

    [Header("Cooldown Emrys Hint Settings")]
    [SerializeField] private Image cooldownFillImage;
    [SerializeField] private float cooldownDuration = 5f;
    private bool isCooldown = false;
    [Header("GameObject Settings")]
    [SerializeField] private List<GameObject> objectsToDisableDuringDialogue; 


    private ItemsManager itemsManager;
    public bool isOpenWorldMode = true;
    public bool IsDialogueActive => dialogueManager != null && dialogueManager.GetisActiveDialogue();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
    }

    private void Start()
    {
        itemsManager = ItemsManager.Instance;
        InitializeScene();
        if (dialogueManager != null)
        {
            dialogueManager.OnDialogueStateChanged += HandleDialogueStateChanged;
        }
    }

    private void Update()
    {
        HandlePanelStatus();
        HandleDialogueObjects(); 
    }

    public bool ArePanelsActive()
    {
        foreach (var panel in panelsToCheck)
        {
            if (panel != null && panel.activeSelf)
            {
                return true;
            }
        }
        return false;
    }

    private void HandlePanelStatus()
    {
        if (ArePanelsActive())
        {
            itemsManager?.EnableBackPack();
            isOpenWorldMode = false;
            DeactivateAllMainHints();
        }
        else
        {
            itemsManager?.DisableBackpack();
            isOpenWorldMode = true;
            DeactivateAllSubHints();
        }
    }

    public void LevelEnding()
    {
        finishLine.isTrigger = true;
        string currentSceneName = SceneManager.GetActiveScene().name;

        if (eventTrigger == null)
        {
            Debug.LogWarning("EventTrigger is not assigned!");
            return;
        }

        switch (currentSceneName)
        {
            case "Stage1":
                eventTrigger.TriggerEvent("Level_1_event_ending");
                break;
            case "Stage2":
                eventTrigger.TriggerEvent("Level_2_event_ending");
                break;
            case "Stage3":
                eventTrigger.TriggerEvent("Level_3_event_ending");
                break;
            default:
                Debug.LogWarning($"No event configured for scene: {currentSceneName}");
                break;
        }
    }

    private void InitializeScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;

        if (eventTrigger == null)
        {
            Debug.LogWarning("EventTrigger is not assigned!");
            return;
        }

        switch (currentSceneName)
        {
            case "Stage1":
                eventTrigger.TriggerEvent("Level_1_event_0");
                break;
            case "Stage2":
                eventTrigger.TriggerEvent("Level_2_event_0");
                break;
            case "Stage3":
                eventTrigger.TriggerEvent("Level_3_event_0");
                break;
            default:
                Debug.LogWarning($"No event configured for scene: {currentSceneName}");
                break;
        }
    }

    private void OnDestroy()
    {
        if (dialogueManager != null)
        {
            dialogueManager.OnDialogueStateChanged -= HandleDialogueStateChanged;
        }
    }
    private void HandleDialogueObjects()
    {
        bool shouldDisableObjects = IsDialogueActive;

        foreach (var obj in objectsToDisableDuringDialogue)
        {
            if (obj != null)
            {
                obj.SetActive(!shouldDisableObjects);
            }
        }
    }

    private void HandleDialogueStateChanged(bool isActive)
    {
        emrysCharacter.SetActive(dialogueManager.GetEmrysSpeak());
    }

    private void ResetCooldownUI()
    {
        if (cooldownFillImage != null)
        {
            cooldownFillImage.fillAmount = 1f;
        }
        isCooldown = false;
    }

    private IEnumerator CooldownCoroutine()
    {
        isCooldown = true;

        float elapsedTime = 0f;

        while (elapsedTime < cooldownDuration)
        {
            elapsedTime += Time.deltaTime;
            if (cooldownFillImage != null)
            {
                cooldownFillImage.fillAmount = elapsedTime / cooldownDuration; 
            }

            yield return null;
        }

        ResetCooldownUI();
    }

    public void AskEmrys()
    {
        if (isCooldown) return;
        StartCoroutine(CooldownCoroutine());

        if (RageManager.Instance != null)
        {
            RageManager.Instance.AddRage(1);
        }

        if (isOpenWorldMode)
        {
            StartCoroutine(ActivateMainHintsWithValidation(hintGroupsModeOpenWorld));
        }
        else
        {
            StartCoroutine(ActivateSubHints(hintGroupsModeOpenWorld));
        }
    }

    private IEnumerator ActivateMainHintsWithValidation(List<HintGroup> hintGroups)
    {
        if (hintGroups == null || hintGroups.Count == 0)
        {
            Debug.LogWarning("No hint groups available for OpenWorld mode.");
            yield break;
        }

        List<Coroutine> activeCoroutines = new List<Coroutine>();

        foreach (var group in hintGroups)
        {
            bool hasSubHints = group.subHints != null && group.subHints.Count > 0;

            if (!hasSubHints)
            {
                Debug.Log($"Skipping mainHint {group.mainHint?.name} because it has no subHints.");
                continue;
            }

            if (group.mainHint != null)
            {
                group.mainHint.SetActive(true);
                Light2D mainLight = group.mainHint.GetComponent<Light2D>();

                if (mainLight != null)
                {
                    Coroutine pulseCoroutine = StartCoroutine(PulseLight(mainLight));
                    activeCoroutines.Add(pulseCoroutine);
                }
            }
        }

        yield return new WaitForSeconds(3f);

        foreach (var coroutine in activeCoroutines)
        {
            StopCoroutine(coroutine);
        }

        foreach (var group in hintGroups)
        {
            if (group.mainHint != null)
            {
                Light2D mainLight = group.mainHint.GetComponent<Light2D>();
                if (mainLight != null)
                {
                    yield return StartCoroutine(FadeOutLight(mainLight));
                }

                group.mainHint.SetActive(false);
            }
        }
    }

    private IEnumerator ActivateSubHints(List<HintGroup> hintGroups)
    {
        if (hintGroups == null || hintGroups.Count == 0)
        {
            Debug.LogWarning("No hint groups available for CloseUp mode.");
            yield break;
        }

        List<Coroutine> activeCoroutines = new List<Coroutine>();

        foreach (var group in hintGroups)
        {
            foreach (var subHint in group.subHints)
            {
                if (subHint != null)
                {
                    subHint.SetActive(true);
                    Light2D subLight = subHint.GetComponent<Light2D>();

                    if (subLight != null)
                    {
                        Coroutine pulseCoroutine = StartCoroutine(PulseLight(subLight));
                        activeCoroutines.Add(pulseCoroutine);
                    }
                }
            }
        }

        yield return new WaitForSeconds(3f);

        foreach (var coroutine in activeCoroutines)
        {
            StopCoroutine(coroutine);
        }

        foreach (var group in hintGroups)
        {
            foreach (var subHint in group.subHints)
            {
                if (subHint != null)
                {
                    Light2D subLight = subHint.GetComponent<Light2D>();
                    if (subLight != null)
                    {
                        yield return StartCoroutine(FadeOutLight(subLight));
                    }

                    subHint.SetActive(false);
                }
            }
        }
    }

    private void DeactivateAllMainHints()
    {
        foreach (var group in hintGroupsModeOpenWorld)
        {
            if (group.mainHint != null && group.mainHint.activeSelf)
            {
                Debug.Log($"Deactivating mainHint {group.mainHint.name} because mode changed to CloseUp.");
                group.mainHint.SetActive(false);
            }
        }
    }

    private void DeactivateAllSubHints()
    {
        foreach (var group in hintGroupsModeOpenWorld)
        {
            foreach (var subHint in group.subHints)
            {
                if (subHint != null && subHint.activeSelf)
                {
                    Debug.Log($"Deactivating subHint {subHint.name} because mode changed to OpenWorld.");
                    subHint.SetActive(false);
                }
            }
        }
    }

    private IEnumerator PulseLight(Light2D light)
    {
        float minIntensity = 1f;
        float maxIntensity = 2f;
        float pulseSpeed = 2f;

        while (true)
        {
            for (float t = 0; t < 1f; t += Time.deltaTime * pulseSpeed)
            {
                light.intensity = Mathf.Lerp(minIntensity, maxIntensity, t);
                yield return null;
            }

            for (float t = 0; t < 1f; t += Time.deltaTime * pulseSpeed)
            {
                light.intensity = Mathf.Lerp(maxIntensity, minIntensity, t);
                yield return null;
            }
        }
    }

    private IEnumerator FadeOutLight(Light2D light)
    {
        float duration = 1f;
        float initialIntensity = light.intensity;
        float targetIntensity = 0f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            light.intensity = Mathf.Lerp(initialIntensity, targetIntensity, elapsedTime / duration);
            yield return null;
        }

        light.intensity = targetIntensity;
    }

    public void removeHintArea(Transform itemTransform)
    {
        foreach (Transform child in itemTransform)
        {
            GameObject childObject = child.gameObject;

            foreach (var hintGroup in hintGroupsModeOpenWorld)
            {
                if (hintGroup.mainHint == childObject)
                {
                    hintGroup.mainHint.SetActive(false);
                    Debug.Log($"Removed main hint {childObject.name} from OpenWorld.");
                    return;
                }

                if (hintGroup.subHints.Contains(childObject))
                {
                    hintGroup.subHints.Remove(childObject);
                    childObject.SetActive(false);
                    Debug.Log($"Removed sub hint {childObject.name} from OpenWorld.");
                    return;
                }
            }
        }
    }
    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
            Debug.Log($"Playing SFX: {clip.name} from GameManager");
        }
        else
        {
            Debug.LogWarning("No AudioClip provided to GameManager.");
        }
    }
    public void PlayItemSFXFromBehavior(ItemBehavior itemBehavior)
    {
        if (itemBehavior != null && itemBehavior.itemData != null && itemBehavior.itemData.sfx != null)
        {
            audioSource.PlayOneShot(itemBehavior.itemData.sfx);
            Debug.Log($"Playing SFX: {itemBehavior.itemData.sfx.name} for {itemBehavior.itemData.name} from GameManager");
        }
        else
        {
            Debug.LogWarning("ItemBehavior or SFX is null, cannot play SFX.");
        }
    }

}
