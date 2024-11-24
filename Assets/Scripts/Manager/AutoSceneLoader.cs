using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AutoSceneLoader : MonoBehaviour
{
    [Header("Scene Transition Settings")]
    [Tooltip("Waktu delay sebelum berpindah ke scene berikutnya.")]
    public float delay = 15f;
    [Tooltip("Nama scene yang akan dimuat setelah delay.")]
    public string sceneName;

    private void Start()
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("Scene name is not set. Please specify the target scene name in the Inspector.");
            return;
        }


        StartCoroutine(LoadSceneAfterDelay(delay, sceneName));
    }

    private IEnumerator LoadSceneAfterDelay(float delay, string sceneName)
    {
        yield return new WaitForSeconds(delay);

        Debug.Log($"Loading scene: {sceneName} after {delay} seconds.");
        SceneManager.LoadScene(sceneName);
    }
}
