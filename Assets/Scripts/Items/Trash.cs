using UnityEngine;

public class Trash : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;


    public void SetSprite(Sprite s)
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        Vector2 spriteSize = spriteRenderer.sprite.rect.size / spriteRenderer.sprite.pixelsPerUnit;

        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        boxCollider.size = spriteSize;

        spriteRenderer.sprite = s;
    }
}
