using UnityEngine;

public class DragZone : MonoBehaviour
{
    [SerializeField] protected GameObject enteredItem;
    [SerializeField] protected bool isHoldingItem;
    [SerializeField] protected bool isHoveringItem;


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

        //enteredItem = collision.gameObject;
        //isHoldingItem = true;
        isHoveringItem = true;
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
            isHoveringItem = false;
        }
    }

    protected virtual void HandleItemRelease(Draggable draggable)
    {
        if (isHoveringItem && isHoldingItem)
        {
            draggable.ResetPosition();
        }
        enteredItem = draggable.gameObject;
        enteredItem.transform.position = transform.position;
        isHoldingItem = true;
    }
}
