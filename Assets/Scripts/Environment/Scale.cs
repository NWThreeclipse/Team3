using TMPro;
using UnityEngine;

public class Scale : DragZone
{
    [SerializeField] private TMP_Text textfield;
    [SerializeField] private AudioSource minigameSFX;


    protected override void OnTriggerExit2D(Collider2D collision)
    {
        base.OnTriggerExit2D(collision);
        textfield.text = "";
    }
    protected override void HandleItemRelease(Draggable draggable)
    {
        base.HandleItemRelease(draggable);
        Item item = enteredItem.GetComponent<Item>();
        textfield.text = item.GetItemData().Weight.ToString();
        minigameSFX.Play();
    }

    
}
