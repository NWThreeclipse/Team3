using UnityEngine;

public class DragZone : MonoBehaviour
{
    [SerializeField] protected GameObject enteredItem;
    [SerializeField] protected bool isHoldingItem;

    public bool IsHoldingItem() => isHoldingItem;
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

        enteredItem = collision.gameObject;
        isHoldingItem = true;
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Item"))
        {
            var draggable = collision.GetComponent<Draggable>();
            if (draggable != null)
            {
                draggable.OnReleased -= HandleItemRelease;
            }

            enteredItem = null;
            isHoldingItem = false;
        }
    }

    protected virtual void HandleItemRelease(Draggable draggable)
    {
        enteredItem = draggable.gameObject;
        enteredItem.transform.position = transform.position;
        isHoldingItem = true;
    }
}
