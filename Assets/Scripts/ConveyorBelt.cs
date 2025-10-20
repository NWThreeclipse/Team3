using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    private bool isMoving;
    [SerializeField] private List<GameObject> collidingItems = new List<GameObject>();

    private void FixedUpdate()
    {
        if(isMoving || collidingItems.Count != 0)
        {
            foreach (GameObject item in collidingItems.ToList())
            {
                if (item == null)
                {
                    collidingItems.Remove(item);
                    continue;
                }

                if (!item.GetComponent<Draggable>().GetHeld())
                {
                    item.transform.position += new Vector3(moveSpeed * Time.deltaTime, 0f, 0f);
                }


            }
        }
    }
    public void ToggleBelt()
    {
        isMoving = !isMoving;
        Debug.Log(isMoving);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Item") && !collidingItems.Contains(collision.gameObject))
        {
            Debug.Log("ITEM ENTERED");
            collidingItems.Add(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Item"))
        {
            collidingItems.Remove(collision.gameObject);
        }
    }
}
