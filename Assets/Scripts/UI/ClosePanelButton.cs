using UnityEngine;

public class ClosePanelButton : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    private void OnMouseDown()
    {
        if (GameManager.Instance.IsDialogueActive)
        {
            Debug.Log("Button click disabled during dialogue.");
            return;
        }

        if (panel != null && ItemsManager.Instance)
        {
            Debug.Log("Closing panel.");
            panel.SetActive(false);
            ItemsManager.Instance.CheckClueCompletion();
        }
    }
}
