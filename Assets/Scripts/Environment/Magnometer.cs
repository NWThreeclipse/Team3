using DG.Tweening;
using DG.Tweening.Core.Easing;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Magnometer : DragZone
{
    [SerializeField] private bool isPlaying = false;
    [SerializeField] private GameObject minigameCanvas;
    [SerializeField] private float[] spriteHeights;

    [SerializeField] GameObject spriteObject;
    [SerializeField] private int itemMagnetism;
    [SerializeField] private int sliderMagnetism;
    private Image itemSprite;
    private Tween currentTween;
    private Tween currentShake;

    [SerializeField] private float[] vibrateStrengths;
    [SerializeField] private float vibrato;

    [SerializeField] private AudioSource minigameSFX;
    [SerializeField] private Slider slider;

    [SerializeField] private GameObject helpCanvas;

    [SerializeField] private AudioClip panelOpen, panelClose;
    [SerializeField] private Vector3 showPanelPos;
    [SerializeField] private Vector3 hidePanelPos;
    [SerializeField] private float panelAnimationTime = 1;

    private Vector3 tempPos;

    protected override void OnTriggerExit2D(Collider2D collision)
    {
        base.OnTriggerExit2D(collision);
        DisableMiniGame();
    }

    private void Start()
    {
        //minigameCanvas.SetActive(false);
        slider.value = 0;
        itemSprite = spriteObject.GetComponentInChildren<Image>();

        Vector3 initialPosition = spriteObject.transform.localPosition;
        initialPosition.y = spriteHeights[0];
        spriteObject.transform.localPosition = initialPosition;
        helpCanvas.SetActive(false);


    }

    private void Update()
    {
        if (isPlaying)
        {
            ItemEffects();
            sliderMagnetism = (int)slider.value;
        }
    }
    private void ItemEffects()
    {
        if (itemMagnetism == 5 )
        {
            if (sliderMagnetism == 0)
            {
                MovespriteObject(0);
            }
            if (sliderMagnetism != 0)
            {
                MovespriteObject(2);
            }
        }
        if (itemMagnetism == 4)
        {
            if (sliderMagnetism == 0)
            {
                MovespriteObject(0);
            }
            if (sliderMagnetism == 1)
            {
                //strong vib & float

                MovespriteObject(1);
                currentShake = itemSprite.gameObject.transform.DOShakePosition(.5f, vibrateStrengths[2]).OnComplete(() => itemSprite.gameObject.transform.position = tempPos);

            }

            if (sliderMagnetism >= 2)
            {
                //pull
                MovespriteObject(2);

            }
        }
        if (itemMagnetism == 3)
        {
            if (sliderMagnetism == 0)
            {
                MovespriteObject(0);
            }
            if (sliderMagnetism == 1)
            {
                //med vib

                MovespriteObject(0);
                currentShake = itemSprite.gameObject.transform.DOShakePosition(.5f, vibrateStrengths[1]).OnComplete(() => itemSprite.gameObject.transform.position = tempPos);

            }
            if (sliderMagnetism == 2)
            {
                //strong vib and float

                MovespriteObject(1);
                currentShake = itemSprite.gameObject.transform.DOShakePosition(.5f, vibrateStrengths[2]).OnComplete(() => itemSprite.gameObject.transform.position = tempPos);

            }
            if (sliderMagnetism >= 3)
            {
                MovespriteObject(2);
            }
        }

        if (itemMagnetism == 2)
        {
            if (sliderMagnetism == 0)
            {
                MovespriteObject(0);
            }
            if (sliderMagnetism == 1)
            {
                //low vib

                MovespriteObject(0);
                currentShake = itemSprite.gameObject.transform.DOShakePosition(.5f, vibrateStrengths[0]).OnComplete(() => itemSprite.gameObject.transform.position = tempPos);

            }
            if (sliderMagnetism == 2)
            {
                //med vib

                MovespriteObject(0);
                currentShake = itemSprite.gameObject.transform.DOShakePosition(.5f, vibrateStrengths[1]).OnComplete(() => itemSprite.gameObject.transform.position = tempPos);

            }
            if (sliderMagnetism == 3)
            {
                //strong vib and float

                MovespriteObject(1);
                currentShake = itemSprite.gameObject.transform.DOShakePosition(.5f, vibrateStrengths[2]).OnComplete(() => itemSprite.gameObject.transform.position = tempPos);

            }
            if (sliderMagnetism >= 4)
            {
                MovespriteObject(2);
            }
        }

        if (itemMagnetism == 1)
        {
            if (sliderMagnetism == 0)
            {
                MovespriteObject(0);
            }
            if (sliderMagnetism == 1)
            {
                //no vib
                MovespriteObject(0);
            }
            if (sliderMagnetism == 2)
            {
                //low vib
                MovespriteObject(0);
                currentShake = itemSprite.gameObject.transform.DOShakePosition(.5f, vibrateStrengths[0]).OnComplete(() => itemSprite.gameObject.transform.position = tempPos);

            }
            if (sliderMagnetism == 3)
            {
                //med vib
                MovespriteObject(0);
                currentShake = itemSprite.gameObject.transform.DOShakePosition(.5f, vibrateStrengths[1]).OnComplete(() => itemSprite.gameObject.transform.position = tempPos);

            }
            if (sliderMagnetism == 4)
            {
                //strong vib and float
                MovespriteObject(1);
                currentShake = itemSprite.gameObject.transform.DOShakePosition(.5f, vibrateStrengths[2]).OnComplete(() => itemSprite.gameObject.transform.position = tempPos);

            }
            if (sliderMagnetism >= 5)
            {
                MovespriteObject(2);
            }
        }

        if (itemMagnetism == 0)
        {
            MovespriteObject(0);
        }


    }


    public void EnableMiniGame()
    {
        if (IsHoldingItem())
        {
            minigameCanvas.transform.DOLocalMove(showPanelPos, panelAnimationTime);
            isPlaying = true;
            SetGoalValues();
            ResetMiniGame();

            SpriteRenderer heldItem = GetEnteredItem().GetComponentInChildren<SpriteRenderer>();
            itemSprite.sprite = heldItem.sprite;
            minigameSFX.Play();
        }
    }
    public void SetGoalValues()
    {
        Item item = GetEnteredItem().GetComponent<Item>();
        itemMagnetism = item.GetItemData().Magnetism;
    }

    public void DisableMiniGame()
    {
        minigameCanvas.transform.DOLocalMove(hidePanelPos, panelAnimationTime);
        isPlaying = false;
        ResetMiniGame();
        minigameSFX.Stop();
        helpCanvas.SetActive(false);
    }

    public void ToggleHelpCanvas()
    {
        helpCanvas.SetActive(!helpCanvas.activeSelf);
    }

    public void ResetMiniGame()
    {
        sliderMagnetism = 0;
        slider.value = 0;
        MovespriteObject(0);
    }


    private void MovespriteObject(int targetIndex)
    {
        if (currentTween != null)
        {
            currentTween.Kill();
        }
        Vector3 newPosition = spriteObject.transform.localPosition;
        newPosition.y = spriteHeights[targetIndex];
        currentTween = spriteObject.transform.DOLocalMove(newPosition, 0.5f).SetEase(Ease.OutQuad);
        tempPos = itemSprite.gameObject.transform.position;
    }
}
