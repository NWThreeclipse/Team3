using DG.Tweening;
using TMPro;
using UnityEngine;

public class ViewingBoard : DragZone
{

    [SerializeField] private TMP_Text textfield;
    [SerializeField] private AudioSource minigameSFX;

    
    private Weight currentWeight = Weight.VeryLight;
    private Weight targetWeight;
    private bool isCounting = false;
    private float countSpeed = .1f;
    private float nextUpdateTime = 0f;

    private void Start()
    {
        textfield.text = "";
    }
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
    }

    protected override void OnTriggerExit2D(Collider2D collision)
    {
        base.OnTriggerExit2D(collision);

        if (isHoldingItem && collision.gameObject == enteredItem)
        {
            textfield.text = "";
            isHoldingItem = false;
            enteredItem = null;
            isCounting = false;
        }
    }


    protected override void HandleItemRelease(Draggable draggable)
    {
        if (isHoldingItem && draggable.gameObject != enteredItem)
        {
            draggable.ResetPosition();
            return;
        }

        if (!isHoveringItem)
        {
            return;
        }

        Item item = draggable.gameObject.GetComponent<Item>();

        if (item.GetItemData().Rarity == Rarity.Anomalous)
        {
            barkManager.HidePlayerBark();
            item.EnableSorting();
        }

        

        enteredItem = draggable.gameObject;
        enteredItem.transform.DOMove(snapPoint.transform.position, 0.1f);
        isHoldingItem = true;
        StartWeightTransition(item.GetItemData().Weight);
    }

    private void StartWeightTransition(Weight weight)
    {
        
        if (!isCounting)
        {
            targetWeight = weight;
            isCounting = true;
            currentWeight = Weight.VeryLight;
            textfield.text = currentWeight.ToString();
            nextUpdateTime = Time.time + countSpeed;
            minigameSFX.Play();
        }
    }

    private void Update()
    {
        if (isCounting)
        {
            if (Time.time >= nextUpdateTime)
            {
                int currentIndex = (int)currentWeight;
                int targetIndex = (int)targetWeight;

                if (currentIndex < targetIndex)
                {
                    currentWeight = (Weight)(currentIndex + 1);
                    textfield.text = currentWeight.ToString();
                    nextUpdateTime = Time.time + countSpeed;
                    minigameSFX.Play();
                }
                else
                {
                    isCounting = false;
                }
            }
        }
    }
}
