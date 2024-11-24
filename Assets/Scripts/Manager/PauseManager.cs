using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu; 
    [SerializeField] private List<GameObject> objectsToHide; 
    private bool isPaused = false;
    private PlayerInput controls;

    private void Awake()
    {
        controls = new PlayerInput();
        controls.Player.Pause.performed += ctx => TogglePause();
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (pauseMenu != null)
        {
            pauseMenu.SetActive(isPaused);
        }

        Time.timeScale = isPaused ? 0f : 1f;

        HandleObjectsVisibility();

        Debug.Log(isPaused ? "Game Paused" : "Game Resumed");
    }

    private void HandleObjectsVisibility()
    {
        if (objectsToHide == null || objectsToHide.Count == 0) return;

        foreach (var obj in objectsToHide)
        {
            if (obj != null)
            {
                obj.SetActive(!isPaused);
            }
        }
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;
    }
}
