using UnityEngine;

public class InteractionIndicator : MonoBehaviour
{
    public GameObject indicatorPrefab;
    public GameObject panelToShow;
    public Vector3 indicatorOffset = new Vector3(0, 1, 0); 
    private GameObject indicatorInstance;
    private bool isIndicatorActive = false;
    private Transform characterTransform;

    private void Start(){
        characterTransform = GameObject.FindWithTag("Player")?.transform;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) 
        {
            ShowIndicator();
            isIndicatorActive = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            HideIndicator();
            isIndicatorActive = false;
        }
    }

    private void ShowIndicator()
    {
        if (indicatorInstance == null)
        {
            indicatorInstance = Instantiate(indicatorPrefab, characterTransform);
            indicatorInstance.transform.localPosition = indicatorOffset;
            Debug.Log("Indicator instantiated at position: " + indicatorInstance.transform.position);
        }
    }

    private void HideIndicator()
    {
        if (indicatorInstance != null)
        {
            Debug.Log("Destroying indicator.");
            Destroy(indicatorInstance);
        }
    }

    public void ActivatePanel()
    {
        if (panelToShow != null)
        {
            panelToShow.SetActive(true);
        }
    }

    public bool IsIndicatorActive()
    {
        return isIndicatorActive;
    }
}
