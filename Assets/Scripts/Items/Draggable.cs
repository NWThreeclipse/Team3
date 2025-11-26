using System;
using UnityEngine;
using System.Collections;
using DG.Tweening;
using System.Linq;

public class Draggable : MonoBehaviour
{
    private bool held = false;

    private Vector3 screenPoint;
    protected Vector3 offset;
    private Vector3 pickupPosition;
    protected bool isInteractible = true;
    public bool IsResetting { get; private set; } = false;


    public bool GetInteractible() => isInteractible;

    public event Action<Draggable> OnReleased;

    protected void Start()
    {
        screenPoint = Camera.main.WorldToScreenPoint(transform.position);

    }

    protected void OnMouseDown()
    {
       
        if (!isInteractible)
        {
            return;
        }
        //check if playing game
        offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
        held = true;
        //maybe custom sounds for anomalous items (if itemdata.pickupsound != null) play that, else play pickupsound
        AudioController.PlayPickupSound();
        pickupPosition = transform.position;
    }

    protected void OnMouseDrag()
    {
        if (!isInteractible)
        {
            return;
        }
        Vector3 currentScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
        Vector3 currentPosition = Camera.main.ScreenToWorldPoint(currentScreenPoint) + offset;
        transform.position = currentPosition;
    }

    protected void OnMouseUp()
    {
        if (!held)
        {
            return;
        }
        held = false;
        AudioController.PlayDropSound();
        CollisionDetection();

    }

    public bool GetHeld() => held;


    public void ResetPosition()
    {
        isInteractible = false;
        IsResetting = true;

        transform.DOLocalMove(pickupPosition, 0.5f).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                isInteractible = true;
                IsResetting = false;
                DetectZoneAfterReset();
            });
    }

    private void DetectZoneAfterReset()
    {
        Collider2D[] hits = Physics2D.OverlapPointAll(transform.position);

        foreach (Collider2D hit in hits)
        {
            if (hit.TryGetComponent(out DragZone zone))
            {
                zone.ForceHandleItem(this);
                return;
            }
        }
    }



    private void CollisionDetection()
    {
        Collider2D[] hits = Physics2D.OverlapPointAll(transform.position);
        int binCount = 0;
        bool inValidZone = false;
        bool onConveyor = false;


        foreach (Collider2D hit in hits)
        {
            if (hit.TryGetComponent(out ConveyorBelt belt))
            {
                onConveyor = true;
                break;
            }

            if (hit.TryGetComponent(out Bin bin))
            {
                inValidZone = true;
                binCount++;
            }
            else if (hit.TryGetComponent(out DragZone zone))
            {
                inValidZone = true;
            }
        }

        // Multi-bin overlap
        if (binCount > 1)
        {
            Debug.Log("Multiple bins detected");
            ResetPosition();
            return;
        }

        // No valid zone or conveyor
        if (!inValidZone && !onConveyor && binCount == 0)
        {
            Debug.Log("Dropped outside drag zone");
            ResetPosition();
            return;
        }


        OnReleased?.Invoke(this);
    }
}
