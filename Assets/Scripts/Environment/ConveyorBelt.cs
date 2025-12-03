using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private bool isMoving = true;
    [SerializeField] private List<GameObject> collidingItems = new List<GameObject>();

    [SerializeField] private float scrollSpeed;

    private SpriteRenderer spriteRenderer;
    private Material movingMaterial;
    private float scrollOffset;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        movingMaterial = spriteRenderer.material;
    }

    private void Update()
    {
        if(isMoving)
        {
            scrollOffset += scrollSpeed * Time.deltaTime;
            scrollOffset = Mathf.Repeat(scrollOffset, 1f);
        }
        movingMaterial.SetFloat("_ScrollOffset", scrollOffset);
    }
    private void FixedUpdate()
    {

        if (isMoving && collidingItems.Count != 0)
        {

            foreach (GameObject item in collidingItems.ToList())
            {
                if (item == null)
                {
                    collidingItems.Remove(item);
                    continue;
                }

                Item draggableItem = item.GetComponent<Item>();
                Trash trashItem = item.GetComponent<Trash>();
                SpriteRenderer itemSP = item.GetComponentInChildren<SpriteRenderer>();


                if (trashItem != null)
                {
                    item.transform.position += new Vector3(moveSpeed * Time.deltaTime, 0f, 0f);
                }

                else if (draggableItem != null && !draggableItem.GetHeld())
                {
                    item.transform.position += new Vector3(moveSpeed * Time.deltaTime, 0f, 0f);
                    itemSP.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;

                }
                else if (draggableItem != null && draggableItem.GetHeld())
                {
                    itemSP.maskInteraction = SpriteMaskInteraction.None;

                }
            }
        }
    }
    public void ToggleBelt()
    {
        isMoving = !isMoving;

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Item") && !collidingItems.Contains(collision.gameObject))
        {
            collidingItems.Add(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Item"))
        {
            collidingItems.Remove(collision.gameObject);
            SpriteRenderer itemSP = collision.gameObject.GetComponentInChildren<SpriteRenderer>();
            itemSP.maskInteraction = SpriteMaskInteraction.None;


        }
    }

    public void SetSpeed(string speed)
    {
        if (float.TryParse(speed, out float result))
        {
            moveSpeed = result;
        }
        else
        {
            Debug.LogWarning("Invalid speed value entered.");
        }
    }


    public void AddItem(GameObject item)
    {
        collidingItems.Add(item);
    }
}
