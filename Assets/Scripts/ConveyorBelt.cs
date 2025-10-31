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

        //moving belt
        if (isMoving && collidingItems.Count != 0)
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
        }
    }
}
