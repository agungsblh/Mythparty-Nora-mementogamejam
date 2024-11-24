using UnityEngine;

public class DraggableItem : MonoBehaviour
{
    public string id;
    private Vector3 initialPosition; 
    private bool isDragging = false; 

    private void Start()
    {
        initialPosition = transform.localPosition;
    }

    private void OnMouseDown()
    {
        isDragging = true;
    }

    private void OnMouseDrag()
    {
        if (!isDragging) return;

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(mousePosition.x, mousePosition.y, transform.position.z);
    }

    private void OnMouseUp()
    {
        isDragging = false;
        ReturnToInitialPosition();
    }

private void OnTriggerEnter2D(Collider2D collision)
{
    ItemBehavior targetItem = collision.GetComponent<ItemBehavior>();
    if (targetItem != null && targetItem.itemData.needKey)
    {
        Debug.Log($"Triggered with {targetItem.itemData.name} requiring key.");

        if (targetItem.itemData.id == id)
        {
            Debug.Log($"Key {id} used on {targetItem.itemData.name}.");

            if (targetItem.itemData.canChange)
            {
                SpriteRenderer targetSpriteRenderer = targetItem.GetComponent<SpriteRenderer>();
                if (targetSpriteRenderer != null )
                {
                    if(targetItem.itemData.spriteChange != null){
                        targetSpriteRenderer.sprite = targetItem.itemData.spriteChange;
                        GameManager.Instance.PlayItemSFXFromBehavior(targetItem);
                    Debug.Log($"Sprite of {targetItem.itemData.name} changed to {targetItem.itemData.spriteChange.name}.");
                    }else{
                        targetSpriteRenderer.sprite = null;
                         GameManager.Instance.PlayItemSFXFromBehavior(targetItem);
                    }
                    
                }
            }else{

            }

            foreach (Transform child in targetItem.transform)
            {
                child.gameObject.SetActive(true);
                Debug.Log($"Child {child.name} of {targetItem.itemData.name} activated.");
            }

            Destroy(gameObject);
        }
        else
        {
            Debug.Log("Key ID does not match the required ID for this item.");
        }
    }
}


    public void SetInitialPosition(Vector3 newPosition)
    {
        initialPosition = newPosition;
    }

    private void ReturnToInitialPosition()
    {
        transform.localPosition = initialPosition;
    }

    public void SetID(string itemID)
    {
        id = itemID;
        Debug.Log($"DraggableItem ID set to: {id}");
    }
}
