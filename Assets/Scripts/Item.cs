using System;
using UnityEngine;

public class Item : Draggable
{
    [SerializeField]private ItemSO itemData;
    private SpriteRenderer spriteRenderer;

    private bool isShrinking = false;
    private bool isGrowing = false;


    private void Start()
    {
        base.Start();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (itemData != null && itemData.Sprite != null)
        {
            spriteRenderer.sprite = itemData.Sprite;
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
            spriteRenderer.sprite = itemData.Sprite;
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


}
