using DG.Tweening;
using TMPro;
using UnityEngine;

public class BioScanner : DragZone
{
    [SerializeField] private bool isPlaying = false;
    //[SerializeField] private ViewingBoard viewingBoard;
    //[SerializeField] private TMP_Text textfield;
    [SerializeField] private float shakeStrength = 0.05f;
    [SerializeField] private GameObject scannerLid;
    private Vector3 originalPosition;



    protected override void OnTriggerExit2D(Collider2D collision)
    {
        base.OnTriggerExit2D(collision);
        DisableMiniGame();
    }
    void Start()
    {
        //textfield.text = "";
        originalPosition = scannerLid.transform.position;
    }

    private void Update()
    {
        if(isPlaying && !IsHoldingItem())
        {
            DisableMiniGame();
        }
    }
    protected override void HandleItemRelease(Draggable draggable)
    {
        if (!DOTween.IsTweening(scannerLid))
        {
            scannerLid.transform.DOShakePosition(shakeStrength, 0.1f).OnComplete(() => scannerLid.transform.DOMove(originalPosition, 0.1f));
        }
        base.HandleItemRelease(draggable);
        

    }

    public void EnableMiniGame()
    {
        if (IsHoldingItem())
        {
            isPlaying = true;
            //textfield.text = GetItem().Organic.ToString() + "%";
        }
    }

    public void DisableMiniGame()

    {
        isPlaying = false;
        //textfield.text = "";
    }
}
