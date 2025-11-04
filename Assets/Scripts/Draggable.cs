using System;
using UnityEngine;
using System.Collections;

public class Draggable : MonoBehaviour
{
    private bool held = false;

    private Vector3 screenPoint;
    protected Vector3 offset;


    public event Action<Draggable> OnReleased;

    protected void Start()
    {
        screenPoint = Camera.main.WorldToScreenPoint(transform.position);

    }

    protected void Update()
    {
        
    }

    protected void OnMouseDown()
    {

        //check if playing game
        offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
        held = true;
        //maybe custom sounds for anomalous items (if itemdata.pickupsound != null) play that, else play pickupsound
        AudioController.PlayPickupSound();
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
            OnReleased?.Invoke(this);
            AudioController.PlayDropSound();
        }
    }
    
    public bool GetHeld() => held;

}
