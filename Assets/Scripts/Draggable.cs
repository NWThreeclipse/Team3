using UnityEngine;

public class Draggable : MonoBehaviour
{
    private bool held = false;
    public float finalScale = 1f;

    private Vector3 screenPoint;
    protected Vector3 offset;

    private bool appearing = true;

    protected void Start()
    {
        //transform.localScale = new Vector3(0.001f, 0.001f, 1);
        appearing = true;
        screenPoint = Camera.main.WorldToScreenPoint(transform.position);

    }

    protected void Update()
    {
        Debug.Log(held);
        if(appearing && transform.localScale.x < finalScale)
        {
            transform.localScale += new Vector3(Time.deltaTime * 3.5f, Time.deltaTime * 3.5f, 0);
        } 
        else if(appearing)
        {
            transform.localScale = new Vector3(finalScale, finalScale, 1);
            appearing = false;
        }
    }

    protected void OnMouseDown()
    {
        //check if playing game
        offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
        held = true;
    }

    protected void OnMouseDrag()
    {
        Vector3 currentScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
        Vector3 currentPosition = Camera.main.ScreenToWorldPoint(currentScreenPoint) + offset;
        transform.position = currentPosition;
    }

    protected void OnMouseUp()
    {
        if(held)
        {
            held = false;
        }
    }

    public bool GetHeld()
    {
        return held;
    }
}
