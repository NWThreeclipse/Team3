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

    [SerializeField] GameObject itemSprite;
    [SerializeField] private int itemMagnetism;
    [SerializeField] private int sliderMagnetism;
    private Image spriteImage;
    private Tween currentTween;
    private Tween currentShake;

    [SerializeField] private float[] vibrateStrengths;
    [SerializeField] private float vibrato;

    [SerializeField] private AudioSource minigameSFX;
    [SerializeField] private Slider slider;

    [SerializeField] private GameObject helpCanvas;



    protected override void OnTriggerExit2D(Collider2D collision)
    {
        base.OnTriggerExit2D(collision);
        DisableMiniGame();
    }

    private void Start()
    {
        minigameCanvas.SetActive(false);
        slider.value = 0;
        spriteImage = itemSprite.GetComponent<Image>();

        Vector3 initialPosition = itemSprite.transform.position;
        initialPosition.y = spriteHeights[0];
        itemSprite.transform.position = initialPosition;
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
        if (itemMagnetism == 5 && sliderMagnetism != 0)
        {
            MoveItemSprite(2);
        }
        if (itemMagnetism == 4)
        {
            if (sliderMagnetism == 1)
            {
                //strong vib & float
                //currentShake = itemSprite.transform.DOShakePosition(.5f, vibrateStrengths[2]);
                MoveItemSprite(1);

            }

            if (sliderMagnetism >= 2)
            {
                //pull
                MoveItemSprite(2);

            }
        }
        if (itemMagnetism == 3)
        {
            if (sliderMagnetism == 1)
            {
                //med vib
                //currentShake = itemSprite.transform.DOShakePosition(.5f, vibrateStrengths[1]);
                MoveItemSprite(0);
            }
            if (sliderMagnetism == 2)
            {
                //strong vib and float
                //currentShake = itemSprite.transform.DOShakePosition(.5f, vibrateStrengths[2]);
                MoveItemSprite(1);
            }
            if (sliderMagnetism >= 3)
            {
                MoveItemSprite(2);
            }
        }

        if (itemMagnetism == 2)
        {
            if (sliderMagnetism == 1)
            {
                //low vib
                //currentShake = itemSprite.transform.DOShakePosition(.5f, vibrateStrengths[0]);
                MoveItemSprite(0);
            }
            if (sliderMagnetism == 2)
            {
                //med vib
                //currentShake = itemSprite.transform.DOShakePosition(.5f, vibrateStrengths[1]);
                MoveItemSprite(0);
            }
            if (sliderMagnetism == 3)
            {
                //strong vib and float
                //currentShake = itemSprite.transform.DOShakePosition(.5f, vibrateStrengths[2]);
                MoveItemSprite(1);
            }
            if (sliderMagnetism >= 4)
            {
                MoveItemSprite(2);
            }
        }

        if (itemMagnetism == 1)
        {
            if (sliderMagnetism == 1)
            {
                //no vib
                MoveItemSprite(0);
            }
            if (sliderMagnetism == 2)
            {
                //low vib
                //currentShake = itemSprite.transform.DOShakePosition(.5f, vibrateStrengths[0]);
                MoveItemSprite(0);
            }
            if (sliderMagnetism == 3)
            {
                //med vib
                //currentShake = itemSprite.transform.DOShakePosition(.5f, vibrateStrengths[1]);
                MoveItemSprite(0);
            }
            if (sliderMagnetism == 4)
            {
                //strong vib and float
                //currentShake = itemSprite.transform.DOShakePosition(.5f, vibrateStrengths[2]);

                MoveItemSprite(1);
            }
            if (sliderMagnetism >= 5)
            {
                MoveItemSprite(2);
            }
        }

        if (itemMagnetism == 0)
        {
            MoveItemSprite(0);
        }


    }


    public void EnableMiniGame()
    {
        if (IsHoldingItem())
        {
            minigameCanvas.SetActive(true);
            isPlaying = true;
            SetGoalValues();
            ResetMiniGame();

            SpriteRenderer heldItem = GetEnteredItem().GetComponentInChildren<SpriteRenderer>();
            spriteImage.sprite = heldItem.sprite;
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
        minigameCanvas.SetActive(false);
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
        MoveItemSprite(0);
    }


    private void MoveItemSprite(int targetIndex)
    {
        if (currentTween != null)
        {
            currentTween.Kill();
        }
        Vector3 newPosition = itemSprite.transform.position;
        newPosition.y = spriteHeights[targetIndex];
        currentTween = itemSprite.transform.DOMove(newPosition, 0.5f).SetEase(Ease.OutQuad);
    }
}
