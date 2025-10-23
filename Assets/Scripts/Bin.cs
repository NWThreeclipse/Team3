using System.Collections.Generic;
using UnityEngine;

public class Bin : MonoBehaviour
{
    [SerializeField] private Sorting sorting;
    [SerializeField] private List<GameObject> collidingItems = new List<GameObject>();
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Item") && !collidingItems.Contains(collision.gameObject))
        {
            collidingItems.Add(collision.gameObject);
            collision.gameObject.GetComponent<Item>().Shrink();
        }
    }

    public void DeleteItem(GameObject item)
    {
        collidingItems.Remove(item);
    }

    public Sorting getSortingType()
    {
        return sorting;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Item"))
        {
            collidingItems.Remove(collision.gameObject);

            collision.gameObject.GetComponent<Item>().Grow();
        }
    }



    public List<GameObject> GetItems()
    {
        return collidingItems;
    }


}
