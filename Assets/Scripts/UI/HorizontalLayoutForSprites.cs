using UnityEngine;

public class HorizontalLayoutForSprites : MonoBehaviour
{
    [SerializeField] private float spacing = 1f; 
    [SerializeField] private bool autoAlign = true;

    public void UpdateLayout()
    {
        // Hitung total lebar semua sprite
        float totalWidth = 0f;
        foreach (Transform child in transform)
        {
            SpriteRenderer sprite = child.GetComponent<SpriteRenderer>();
            if (sprite != null)
            {
                totalWidth += sprite.bounds.size.x;
                totalWidth += spacing;
            }
        }

        if (transform.childCount > 0)
        {
            totalWidth -= spacing; 
        }

        float startX = -totalWidth / 2f;
        float currentX = startX;

        foreach (Transform child in transform)
        {
            SpriteRenderer sprite = child.GetComponent<SpriteRenderer>();
            if (sprite != null)
            {
                child.localPosition = new Vector3(currentX + sprite.bounds.size.x / 2f, 0, 0);
                currentX += sprite.bounds.size.x + spacing;
            }
        }
    }

    private void OnValidate()
    {
        if (autoAlign)
        {
            UpdateLayout();
        }
    }
}
