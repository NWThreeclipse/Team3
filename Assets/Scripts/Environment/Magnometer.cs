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

    [SerializeField] DialController magnometerDial;
    [SerializeField] GameObject itemSprite;
    [SerializeField] private int itemMagnetism;
    [SerializeField] private int dialMagnetism;
    private Image spriteImage;
    [SerializeField] private TMP_Text magnetismText;
    private Tween currentTween;
    [SerializeField] private float[] vibrateStrengths;
    [SerializeField] private float vibrato;

    [SerializeField] private AudioSource minigameSFX;



    private void Start()
    {
        minigameCanvas.SetActive(false);
        magnometerDial.InitializeDial(1, 7, true);
        spriteImage = itemSprite.GetComponent<Image>();
        magnetismText.text = "";

        Vector3 initialPosition = itemSprite.transform.position;
        initialPosition.y = spriteHeights[0];
        itemSprite.transform.position = initialPosition;

    }

    private void Update()
    {
        if (isPlaying)
        {
            ItemEffects();
            dialMagnetism = (int)magnometerDial.GetCurrentValue();
            magnetismText.text = dialMagnetism.ToString();

        }
    }
    private void ItemEffects()
    {
        if (itemMagnetism == 5)
        {
            MoveItemSprite(2);
        }
        if (itemMagnetism == 4)
        {
            if (dialMagnetism == 1)
            {
                //strong vib & float
                MoveItemSprite(1);

            }

            if (dialMagnetism >= 2)
            {
                //pull
                MoveItemSprite(2);

            }
        }
        if (itemMagnetism == 3)
        {
            if (dialMagnetism == 1)
            {
                //med vib
                MoveItemSprite(0);
            }
            if (dialMagnetism == 2)
            {
                //strong vib and float
                MoveItemSprite(1);
            }
            if (dialMagnetism >= 3)
            {
                MoveItemSprite(2);
            }
        }

        if (itemMagnetism == 2)
        {
            if (dialMagnetism == 1)
            {
                //low vib
                MoveItemSprite(0);
            }
            if (dialMagnetism == 2)
            {
                //med vib
                MoveItemSprite(0);
            }
            if (dialMagnetism == 3)
            {
                //strong vib and float
                MoveItemSprite(1);
            }
            if (dialMagnetism >= 4)
            {
                MoveItemSprite(2);
            }
        }

        if (itemMagnetism == 1)
        {
            if (dialMagnetism == 1)
            {
                //no vib
                MoveItemSprite(0);
            }
            if (dialMagnetism == 2)
            {
                //low vib
                MoveItemSprite(0);
            }
            if (dialMagnetism == 3)
            {
                //med vib
                MoveItemSprite(0);
            }
            if (dialMagnetism == 4)
            {
                //strong vib and float
                MoveItemSprite(1);
            }
            if (dialMagnetism >= 5)
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
            magnometerDial.SetEnabled(true);
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

    }

    public void ResetMiniGame()
    {
        dialMagnetism = 0;
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
