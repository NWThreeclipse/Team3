using UnityEngine;

public class Trash : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private SpriteRenderer shadowRenderer;

    public void SetSprite(Sprite s)
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        shadowRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        //Debug.Log(shadowRenderer.gameObject.name);
        Vector2 spriteSize = spriteRenderer.sprite.rect.size / spriteRenderer.sprite.pixelsPerUnit;

        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        boxCollider.size = spriteSize;

        spriteRenderer.sprite = s;
        shadowRenderer.sprite = s;
    }
}
