using System;
using UnityEngine;

public class ItemBehavior : MonoBehaviour
{
    public Items itemData;
    public EventTrigger eventTrigger;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer is missing on the object.");
        }
        if (ItemsManager.Instance != null)
        {
            Initialize(itemData, ItemsManager.Instance);
        }
        else
        {
            Debug.LogError("ItemsManager instance is not available in the scene.");
        }
    }

    public void Initialize(Items itemTemplate, IItemsManager itemsManager)
    {
        itemData = Instantiate(itemTemplate);
    }

    private void OnMouseDown()
    {
        Debug.Log("OnMouseDown detected");
        if (DialogueManager.Instance != null && DialogueManager.Instance.GetisActiveDialogue())
        {
            Debug.Log("Dialogue is active. Interaction disabled.");
            return;
        }

        if (itemData == null || ItemsManager.Instance == null) return;

        if (itemData.spriteChange != null && itemData.interactable && !itemData.needKey && !itemData.taken)
        {
            Debug.Log($"Item {itemData.name} is interactable and has a new sprite.");
            itemData.taken = true;
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = itemData.spriteChange;
                Debug.Log($"Sprite changed to {itemData.spriteChange.name}.");
            }
            else
            {
                Debug.LogWarning("SpriteRenderer is not assigned.");
            }
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(true);
                Debug.Log($"Child {child.name} activated.");
            }
            PlayItemSFX();
            removeHint();
        }

        if (itemData.clue && itemData.interactable && !itemData.taken && !itemData.keyItem)
        {
            Debug.Log("Clue is interactable and not taken");
            itemData.taken = true;
            ItemsManager.Instance.RegisterCluePickup(itemData);
            PlayItemSFX();
            gameObject.SetActive(false);

            if (eventTrigger != null)
            {
                eventTrigger.TriggerEvent(itemData.id);
            }
            
            removeHint();
        }
        else if (!itemData.clue && itemData.interactable && itemData.keyItem)
        {
            PlayItemSFX();
            ItemsManager.Instance.RegisterKeyItems(itemData, gameObject);
            removeHint();
            gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("Item is not interactable");
        }
    }
    private void removeHint(){
                if (GameManager.Instance != null)
            {
                GameManager.Instance.removeHintArea(transform);
            }
    }
    private void PlayItemSFX()
    {
        if (itemData != null && itemData.sfx != null)
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.PlayItemSFXFromBehavior(this);
            }
            else
            {
                Debug.LogWarning("GameManager instance is missing.");
            }
        }
        else
        {
            Debug.LogWarning($"No SFX assigned for {itemData?.name ?? "unknown item"}.");
        }
    }
}
