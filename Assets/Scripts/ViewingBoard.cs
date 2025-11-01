using System.Net;
using Unity.VisualScripting;
using UnityEngine;

public class ViewingBoard : MonoBehaviour
{

    [SerializeField] private GameObject enteredItem;
    [SerializeField] private bool isHoldingItem;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!isHoldingItem)
        {
            if (collision.CompareTag("Item") && !enteredItem.Equals(collision.gameObject))
            {
                enteredItem = collision.gameObject;
                collision.gameObject.transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
                isHoldingItem = true;
            }
        }
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Item"))
        {
            enteredItem = null;
            isHoldingItem = false;
        }

    }

    public bool IsHoldingItem()
    {
        return isHoldingItem;
    }

    public ItemSO GetItem()
    {
        return enteredItem.GetComponent<Item>().getItemData();
    }
}
