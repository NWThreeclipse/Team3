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
    [SerializeField] private Vector3[] spriteHeights;

    [SerializeField] GameObject spriteObject;
    [SerializeField] private int itemMagnetism;
    [SerializeField] private int sliderMagnetism;
    private Image itemSprite;
    private Tween currentTween;
    //private Tween currentShake;

    [SerializeField] private float[] vibrateStrengths;
    [SerializeField] private float vibrato;

    [SerializeField] private AudioSource minigameSFX;
    [SerializeField] private Slider slider;

    [SerializeField] private GameObject helpCanvas;

    [SerializeField] private AudioClip panelOpen, panelClose;
    [SerializeField] private Vector3 showPanelPos;
    [SerializeField] private Vector3 hidePanelPos;
    [SerializeField] private float panelAnimationTime = 1;

    [SerializeField] private Vector3 showPanelPos2;
    [SerializeField] private Vector3 hidePanelPos2;
    private bool helpCanvasEnabled = false;
    [SerializeField] private ThermalScanner thermalScanner;

    [SerializeField] private float vibSpeed;
    [SerializeField] private float vibAmplitude;

    [SerializeField] private float lowSpeed = 25f;
    [SerializeField] private float lowAmp = 2f;
    [SerializeField] private float medSpeed = 30f;
    [SerializeField] private float medAmp = 4f;
    [SerializeField] private float highSpeed = 50f;
    [SerializeField] private float highAmp = 3f;
    public bool GetPlaying() => isPlaying;


    protected override void OnTriggerExit2D(Collider2D collision)
    {
        DisableMiniGame();
        base.OnTriggerExit2D(collision);
        SetVibrateStrength(0);
    }

    private void Start()
    {
        //minigameCanvas.SetActive(false);
        slider.value = 0;
        itemSprite = spriteObject.GetComponentInChildren<Image>();

        Vector3 initialPosition = spriteObject.transform.localPosition;
        initialPosition = spriteHeights[0];
        spriteObject.transform.localPosition = initialPosition;
        //helpCanvas.SetActive(false);


    }

    private void Update()
    {
        if (isPlaying)
        {
            ItemEffects();
            sliderMagnetism = (int)slider.value;

            ApplyVibrate();
        }
    }

    private void ApplyVibrate()
    {
        float xOffset = Mathf.Sin(Time.time * vibSpeed) * vibAmplitude;
        float yOffset = Mathf.Cos(Time.time * vibSpeed * 1.3f) * vibAmplitude;

        itemSprite.transform.localPosition = new Vector3(xOffset, yOffset, 0);
    }

    private void SetVibrateStrength(int strength)
    {
        switch (strength)
        {
            // low
            case 1:
                vibSpeed = lowSpeed;
                vibAmplitude = lowAmp;
                break;
            // med
            case 2:
                vibSpeed = medSpeed;
                vibAmplitude = medAmp;
                break;
            // high
            case 3:
                vibSpeed = highSpeed;
                vibAmplitude = highAmp;
                break;
            case 0:
                vibSpeed = 0f;
                vibAmplitude = 0f;
                break;
        }
    }

    private void ItemEffects()
    {
        if (itemMagnetism == 5)
        {
            if (sliderMagnetism == 0)
            {
                MovespriteObject(0);
                SetVibrateStrength(0);
            }
            if (sliderMagnetism != 0)
            {
                MovespriteObject(2);
                //SetVibrateStrength(3);
            }
        }
        if (itemMagnetism == 4)
        {
            if (sliderMagnetism == 0)
            {
                MovespriteObject(0);
                SetVibrateStrength(0);
            }
            if (sliderMagnetism == 1)
            {
                //strong vib & float
                MovespriteObject(1);
                SetVibrateStrength(3);

            }

            if (sliderMagnetism >= 2)
            {
                //pull
                MovespriteObject(2);
                SetVibrateStrength(0);

            }
        }
        if (itemMagnetism == 3)
        {
            if (sliderMagnetism == 0)
            {
                MovespriteObject(0);
                SetVibrateStrength(0);
            }
            if (sliderMagnetism == 1)
            {
                // med vib
                MovespriteObject(0);
                SetVibrateStrength(2);
            }
            if (sliderMagnetism == 2)
            {
                // strong vib
                MovespriteObject(1);
                SetVibrateStrength(3);
            }
            if (sliderMagnetism >= 3)
            {
                // pull
                MovespriteObject(2);
                SetVibrateStrength(0);

            }
        }

        if (itemMagnetism == 2)
        {
            if (sliderMagnetism == 0)
            {
                MovespriteObject(0);
                SetVibrateStrength(0);
            }
            if (sliderMagnetism == 1)
            {
                // low vib
                MovespriteObject(0);
                SetVibrateStrength(1);
            }
            if (sliderMagnetism == 2)
            {
                // med vib
                MovespriteObject(0);
                SetVibrateStrength(2);
            }
            if (sliderMagnetism == 3)
            {
                //strong vib and float

                MovespriteObject(1);
                SetVibrateStrength(3);
            }
            if (sliderMagnetism >= 4)
            {
                // pull
                MovespriteObject(2);
                SetVibrateStrength(0);
            }
        }

        if (itemMagnetism == 1)
        {
            if (sliderMagnetism == 0)
            {
                MovespriteObject(0);
                SetVibrateStrength(0);
            }
            if (sliderMagnetism == 1)
            {
                //no vib
                MovespriteObject(0);
                SetVibrateStrength(0);
            }
            if (sliderMagnetism == 2)
            {
                //low vib
                MovespriteObject(0);
                SetVibrateStrength(1);
            }
            if (sliderMagnetism == 3)
            {
                //med vib
                MovespriteObject(0);
                SetVibrateStrength(2);
            }
            if (sliderMagnetism == 4)
            {
                //strong vib and float
                MovespriteObject(1);
                SetVibrateStrength(3);
            }
            if (sliderMagnetism >= 5)
            {
                // pull
                MovespriteObject(2);
                SetVibrateStrength(0);

            }
        }

        if (itemMagnetism == 0)
        {
            MovespriteObject(0);
            SetVibrateStrength(0);
        }


    }


    public void EnableMiniGame()
    {
        if (thermalScanner.GetPlaying())
        {
            return;
        }
        if (IsHoldingItem())
        {
            minigameCanvas.transform.DOLocalMove(showPanelPos, panelAnimationTime).SetEase(Ease.OutQuad);
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
        minigameCanvas.transform.DOLocalMove(hidePanelPos, panelAnimationTime).SetEase(Ease.InQuad);
        isPlaying = false;
        ResetMiniGame();
        minigameSFX.Stop();
        if (helpCanvasEnabled)
        {
            ToggleHelpCanvas();
        }
        //helpCanvas.SetActive(false);
    }

    public void ToggleHelpCanvas()
    {
        helpCanvasEnabled = !helpCanvasEnabled;
        helpCanvas.transform.DOLocalMove(helpCanvasEnabled ? showPanelPos2 : hidePanelPos2, panelAnimationTime).SetEase(helpCanvasEnabled ? Ease.OutQuad : Ease.InQuad);
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
        Vector3 newPosition = spriteHeights[targetIndex];
        currentTween = spriteObject.transform.DOLocalMove(newPosition, 0.5f);//.SetEase(Ease.OutQuad);
    }
}
