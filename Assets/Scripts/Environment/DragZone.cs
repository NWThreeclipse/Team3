using DG.Tweening;
using UnityEngine;

public class DragZone : MonoBehaviour
{
    [SerializeField] protected GameObject enteredItem;
    [SerializeField] protected bool isHoldingItem;
    [SerializeField] protected bool isHoveringItem;
    [SerializeField] protected GameObject snapPoint;
    [SerializeField] protected BarkManager barkManager;



    public bool IsHoldingItem() => isHoldingItem;
    public bool IsHoveringItem() => isHoveringItem;

    public ItemSO GetItem() => enteredItem != null ? enteredItem.GetComponent<Item>().GetItemData() : null;
    public GameObject GetEnteredItem() => enteredItem != null ? enteredItem: null;


    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Item"))
        {
            var draggable = collision.GetComponent<Draggable>();

            if (draggable != null)
            {
                draggable.OnReleased += HandleItemRelease;
            }
        }

        isHoveringItem = true;
    }

    protected virtual void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.CompareTag("Item"))
        {
            return;
        }
        if (enteredItem != null)
        {
            SpriteRenderer spriteRenderer = collision.gameObject.GetComponentInChildren<SpriteRenderer>();
            spriteRenderer.sortingLayerID = SortingLayer.NameToID("Held");
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Item"))
        {
            return;
        }

        var draggable = collision.GetComponent<Draggable>();

        if (draggable != null)
            isHoveringItem = false;

        if (collision.gameObject == enteredItem)
        {
            if (!draggable.IsResetting)
            {
                draggable.OnReleased -= HandleItemRelease;
                enteredItem = null;
                isHoldingItem = false;
            }
            return;
        }

        if (draggable != null)
            draggable.OnReleased -= HandleItemRelease;
    }



    protected virtual void HandleItemRelease(Draggable draggable)
    {
        if (isHoldingItem && draggable.gameObject != enteredItem)
        {
            draggable.ResetPosition();
            return;
        }

        if (!isHoveringItem)
        {
            return;
        }

        Item item = draggable.gameObject.GetComponent<Item>();

        if (item.GetItemData().Rarity == Rarity.Anomalous)
        {
            barkManager.HidePlayerBark();
            item.EnableSorting();
        }

        //SpriteRenderer spriteRenderer = draggable.gameObject.GetComponentInChildren<SpriteRenderer>();
        //spriteRenderer.sortingLayerID = SortingLayer.NameToID("Held");


        enteredItem = draggable.gameObject;
        enteredItem.transform.DOMove(snapPoint.transform.position, 0.1f);
        isHoldingItem = true;
    }


    public void ForceHandleItem(Draggable draggable)
    {
        HandleItemRelease(draggable);
    }


}
