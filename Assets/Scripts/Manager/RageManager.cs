using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RageManager : MonoBehaviour
{
    public static RageManager Instance { get; private set; }

    [SerializeField] private int currentRage = 0;
    private int rage;

    private EventTrigger eventTrigger;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        FindEventTrigger();
    }

    public void AddRage(int amount)
    {
        rage += amount;
        currentRage += amount;
        Debug.Log($"Rage increased by {amount}. Current rage: {rage}");
    }

    public int GetRage() => rage;

    public void ResetRage()
    {
        rage = 0;
        currentRage = 0;
        Debug.Log("Rage has been reset.");
    }

    public void StartGame()
    {
        ResetRage();
        SceneManager.LoadScene("Prolog"); 
    }

    private void Update()
    {
        CheckEndingStage(); 
    }

    private void FindEventTrigger()
    {
        eventTrigger = FindObjectOfType<EventTrigger>();

        if (eventTrigger == null)
        {
            Debug.LogWarning("No EventTrigger found in the current scene.");
        }
        else
        {
            Debug.Log($"EventTrigger found: {eventTrigger.name}");
        }
    }

    private void CheckEndingStage()
    {
        if (SceneManager.GetActiveScene().name == "StageEnding")
        {
            if (eventTrigger == null)
            {
                Debug.LogWarning("EventTrigger is not set. Attempting to find it.");
                FindEventTrigger();
            }

            if (eventTrigger != null)
            {
                if (rage > 0)
                {
                    eventTrigger.TriggerEvent("Level_bad_ending");
                }
                else
                {
                    eventTrigger.TriggerEvent("Level_good_ending");
                }
            }
            else
            {
                Debug.LogError("EventTrigger is still null. Cannot trigger events.");
            }
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindEventTrigger(); 
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded; 
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
