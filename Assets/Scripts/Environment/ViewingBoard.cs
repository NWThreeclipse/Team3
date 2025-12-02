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
        textfield.text = "";
    }

    protected override void HandleItemRelease(Draggable draggable)
    {
        base.HandleItemRelease(draggable);
        Item item = draggable.gameObject.GetComponent<Item>();
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
