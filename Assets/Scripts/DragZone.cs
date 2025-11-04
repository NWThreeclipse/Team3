using UnityEngine;

public class DragZone : MonoBehaviour
{
    [SerializeField] protected GameObject enteredItem;
    [SerializeField] protected bool isHoldingItem;

    public bool IsHoldingItem() => isHoldingItem;
    public ItemSO GetItem() => enteredItem.GetComponent<Item>().GetItemData();


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
        }
    }

    protected virtual void HandleItemRelease(Draggable draggable)
    {
        enteredItem = draggable.gameObject;
        enteredItem.transform.position = transform.position;
        isHoldingItem = true;
    }
}
