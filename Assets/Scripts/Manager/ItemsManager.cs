using System.Collections.Generic;
using UnityEngine;

public class ItemsManager : MonoBehaviour, IItemsManager
{
    public static ItemsManager Instance { get; private set; } 
    private readonly List<Items> clueItems = new List<Items>();
    public List<Items> keyItems = new List<Items>();
    private int collectedClues = 0;
    public int RequiredClues { get; private set; } = 3;
    public GameManager gameManager;
    public GameObject backpack;

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
    }
    private void Start() 
    {
        gameManager = GetComponent<GameManager>();
        
    }
    public void RegisterCluePickup(Items item)
    {
        if (item == null || clueItems.Contains(item)) return;

        clueItems.Add(item);
        collectedClues++;
        Debug.Log($"Clue collected: {item.name}");
    }
    public void RegisterKeyItems(Items item, GameObject itemObject)
    {
        if (item == null || keyItems.Contains(item)) return;

        keyItems.Add(item);
        Debug.Log($"Key collected: {item.name}");
        AddToBackpack(itemObject);
    }

    public void CheckClueCompletion()
    {
        if (collectedClues >= RequiredClues)
        {
            Debug.Log("Target clue count reached! Triggering event...");
            TriggerStageCompletionEvent();
        }
    }

    private void TriggerStageCompletionEvent()
    {
        Debug.Log("Stage completed! Performing stage completion actions.");
        //tambahan
        gameManager.LevelEnding();
    }
    public void DisableBackpack(){
        backpack.SetActive(false);
    }
    public void EnableBackPack(){
        backpack.SetActive(true);
    }

    public void AddToBackpack(GameObject itemObject)
    {
        if (backpack == null) return;

        GameObject itemCopy = Instantiate(itemObject);
        itemCopy.SetActive(true);

        ItemBehavior originalBehavior = itemObject.GetComponent<ItemBehavior>();
        if (originalBehavior != null)
        {
            Destroy(itemCopy.GetComponent<ItemBehavior>());
        }

        itemCopy.transform.SetParent(backpack.transform);
        itemCopy.transform.localScale = Vector3.one;
        itemCopy.transform.localPosition = Vector3.zero;

        SpriteRenderer spriteRenderer = itemCopy.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = 104;
        }

        DraggableItem draggable = itemCopy.AddComponent<DraggableItem>();
        if (draggable != null && originalBehavior != null)
        {
            if (originalBehavior.itemData != null)
            {
                draggable.SetID(originalBehavior.itemData.id);
            }
        }
        HorizontalLayoutForSprites layout = backpack.GetComponent<HorizontalLayoutForSprites>();
        if (layout != null)
        {
            layout.UpdateLayout();

            foreach (Transform child in backpack.transform)
            {
                DraggableItem childDraggable = child.GetComponent<DraggableItem>();
                if (childDraggable != null)
                {
                    childDraggable.SetInitialPosition(child.localPosition);
                }
            }
        }
        else
        {
            Debug.LogWarning("HorizontalLayoutForSprites component not found on backpack.");
        }

        Debug.Log($"Item {itemObject.name} added to backpack.");
    }
}


public interface IItemsManager
{
    void RegisterCluePickup(Items item);
}
