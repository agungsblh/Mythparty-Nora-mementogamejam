using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SceneEnd : MonoBehaviour
{
    public GameObject playerChar;
    public SceneManagers sceneManagers;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == playerChar)
        {
            StartCoroutine(ToNextLevel());
        }
    }
    public IEnumerator ToNextLevel()
    {
        yield return new WaitForSeconds(0.1f);
        sceneManagers.LoadNextScene();
    }
}
