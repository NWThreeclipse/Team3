using System.Collections.Generic;
using UnityEngine;

public class Bin : MonoBehaviour
{
    [SerializeField] private Sorting sorting;
    [SerializeField] private List<GameObject> collidingItems = new List<GameObject>();
    [SerializeField] private GameManager gameManager;

    public void DeleteItem(GameObject gameObject) => collidingItems.Remove(gameObject);


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Item"))
        {
            GameObject gameObject = collision.gameObject;
            if (!collidingItems.Contains(gameObject))
                collidingItems.Add(gameObject);

            var draggable = gameObject.GetComponent<Draggable>();
            if (draggable != null)
                draggable.OnReleased += HandleItemRelease;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Item"))
        {
            GameObject gameObject = collision.gameObject;
            collidingItems.Remove(gameObject);

            var draggable = gameObject.GetComponent<Draggable>();
            if (draggable != null)
                draggable.OnReleased -= HandleItemRelease;
        }
    }

    private void HandleItemRelease(Draggable draggable)
    {
        GameObject gameObject = draggable.gameObject;
        if (!collidingItems.Contains(gameObject))
        {
            return;
        }
        //check if colliding with multiple bins, if so then snap back to safe area

        Item item = gameObject.GetComponent<Item>();

        if (item.GetItemData().Sorting == sorting)
        {
            gameManager.AddStat(sorting, item.GetItemData().Value);
        }
        else
        {
            gameManager.SubtractStat(sorting, item.GetItemData().Value);
            gameManager.CheckLoss();
        }

        DeleteItem(gameObject);
        Destroy(gameObject);
    }

}
