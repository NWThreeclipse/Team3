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
        if (!isInteractible || GameManager.isMinigameOpen())
        {
            return;
        }

        //check if playing game
        offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
        held = true;
        AudioController.PlayPickupSound();
        pickupPosition = transform.position;
    }

    protected void OnMouseDrag()
    {
        if (!isInteractible || !held)
        {
            return;
        }
        Vector3 currentScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
        Vector3 currentPosition = Camera.main.ScreenToWorldPoint(currentScreenPoint) + offset;
        transform.position = currentPosition;
    }

    protected void OnMouseUp()
    {
        if (!held || IsResetting)
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
        Collider2D itemCollider = GetComponent<Collider2D>();
        if (itemCollider == null) return;
        Collider2D[] hits = Physics2D.OverlapBoxAll(itemCollider.bounds.center, itemCollider.bounds.size, 0f);

        bool inValidZone = false;
        bool onConveyor = false;
        Bin closestBin = null;
        float closestDistance = float.MaxValue;



        foreach (Collider2D hit in hits)
        {

            if (hit.TryGetComponent(out Bin bin))
            {
                inValidZone = true;

                // find closest bin
                float distance = Vector3.Distance(transform.position, bin.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestBin = bin;
                }
            }
            else if (hit.TryGetComponent(out DragZone zone))
            {
                inValidZone = true;
            }


            if (hit.TryGetComponent(out ConveyorBelt belt))
            {
                onConveyor = true;
            }

        }

        // if item dropped in bin, send to closest one
        if (closestBin != null)
        {
            Debug.Log("Bin drop: " + closestBin.name);
            closestBin.HandleItemRelease(this);
            return;
        }


        // No valid zone or conveyor
        if (!inValidZone && !onConveyor)
        {
            Debug.Log("Dropped outside drag zone");
            ResetPosition();
            return;
        }


        OnReleased?.Invoke(this);
    }
}
