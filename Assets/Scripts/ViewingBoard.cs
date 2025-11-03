using NUnit.Framework.Interfaces;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;

public class ViewingBoard : MonoBehaviour
{

    [SerializeField] private GameObject enteredItem;
    [SerializeField] private bool isHoldingItem;

    public bool IsHoldingItem() => isHoldingItem;
    public ItemSO GetItem() => enteredItem.GetComponent<Item>().GetItemData();


    private void OnTriggerEnter2D(Collider2D collision)
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

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Item"))
        {
            var draggable = collision.GetComponent<Draggable>();
            if (draggable != null)
                draggable.OnReleased -= HandleItemRelease;
        }
    }

    private void HandleItemRelease(Draggable draggable)
    {
        enteredItem = draggable.gameObject;
        enteredItem.transform.position = transform.position;
        isHoldingItem = true;
    }

}
