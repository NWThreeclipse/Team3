using System;
using System.Linq;
using UnityEngine;

public class Item : Draggable
{
    [SerializeField] private ItemSO itemData;
    private SpriteRenderer spriteRenderer;

    private bool isShrinking = false;
    private bool isGrowing = false;

    private void Start()
    {
        base.Start();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (itemData != null && itemData.Sprite.Count() != 0)
        {
            SetSprite();

            //Vector2 spriteSize = spriteRenderer.sprite.rect.size / spriteRenderer.sprite.pixelsPerUnit;

            //BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
            //boxCollider.size = spriteSize;
        }
    }

    public ItemSO GetItemData() => itemData;

    public void Shrink()
    {
        offset = new Vector3(0, 0, offset.z);

        if (!isShrinking && !isGrowing)
            isShrinking = true;
    }

    public void Grow()
    {
        if (!isShrinking && !isGrowing)
            isGrowing = true;
    }

    public void SetItemData(ItemSO newItemData)
    {
        itemData = newItemData;
        if (spriteRenderer != null && itemData != null && itemData.Sprite != null)
        {
            SetSprite();
        }
    }

    private void SetSprite()
    {
        if (itemData.Sprite.Count() == 1)
        {
            spriteRenderer.sprite = itemData.Sprite[0];
        }
        else
        {
            spriteRenderer.sprite = itemData.Sprite[UnityEngine.Random.Range(0, itemData.Sprite.Count())];
        }
    }

    void Update()
    {
        base.Update();

        if (isShrinking && transform.localScale.x > .2f)
        {
            transform.localScale -= new Vector3(5f, 5f, 0) * Time.deltaTime;
        }
        else if (isShrinking)
        {
            isShrinking = false;
        }

        if (isGrowing && transform.localScale.x < 1f)
        {
            transform.localScale += new Vector3(5f, 5f, 0) * Time.deltaTime;
        }
        else if (isGrowing)
        {
            isGrowing = false;
        }
    }

    private void OnMouseDown()
    {
        base.OnMouseDown();
        spriteRenderer.sortingLayerID = SortingLayer.NameToID("Held");
        

    }

    private void OnMouseUp()
    {
        base.OnMouseUp();
        spriteRenderer.sortingLayerID = SortingLayer.NameToID("Item");
    }

    
}
