using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class StatsUIController : MonoBehaviour
{
    [SerializeField] private GameObject statsCanvas;
    [SerializeField] private TMP_Text totalItems;
    [SerializeField] private TMP_Text correctItems;
    [SerializeField] private TMP_Text incorrectItems;
    [SerializeField] private TMP_Text sortSpeed;
    [SerializeField] private TMP_Text dayText;
    [SerializeField] private Image anomalousItemImage;
    [SerializeField] private Sprite[] anomalousItemSprites;
    [SerializeField] private GameObject bubblePrefab;
    [SerializeField] private GameObject gridParent;

    [SerializeField] private Dialogue bark;
    [SerializeField] private float textSpeed = 0.01f;

    [SerializeField] private GameObject barkCanvas;
    [SerializeField] private TMP_Text barkText;

    [SerializeField] private GameObject nextButton;
    [SerializeField] private GameObject nextDayButton;

    [SerializeField] private AudioClip panelOpen, panelClose;
    [SerializeField] private Vector3 showPanelPos;
    [SerializeField] private Vector3 hidePanelPos;

    [SerializeField] private Vector3 dialogueShowPanelPos;
    [SerializeField] private Vector3 dialogueHidePanelPos;
    [SerializeField] private float panelAnimationTime = 1;

    [SerializeField] private float shakeStrength;
    [SerializeField] private MusicController musicController;
    [SerializeField] private Image dialogueSprite;
    [SerializeField] private TMP_Text playerName;

    [SerializeField] private Volume postprocessingVolume;
    private Vignette vignette;
    private Coroutine fadeInCoroutine;
    private Coroutine fadeOutCoroutine;
    private AudioSource source;


    private void Awake()
    {
        musicController.FadeInMusic();

    }
    private void Start()
    {
        dayText.text = StatsController.Instance.GetDays().ToString();
        totalItems.text = StatsController.Instance.GetItems().ToString();
        correctItems.text = StatsController.Instance.GetCorrectItems().ToString();
        incorrectItems.text = StatsController.Instance.GetIncorrectItems().ToString();

        if (StatsController.Instance.GetItems() == 0)
        {
            sortSpeed.text = "0";
        }
        else
        {
            sortSpeed.text = (150 / StatsController.Instance.GetItems()).ToString();
        }
        source = GetComponent<AudioSource>();
        //barkCanvas.SetActive(false);
        //statsCanvas.SetActive(true);
        statsCanvas.transform.DOLocalMove(showPanelPos, panelAnimationTime).SetEase(Ease.OutQuad);
        postprocessingVolume.profile.TryGet<Vignette>(out vignette);
        anomalousItemImage.enabled = false;

    }

    public void HideStats()
    {
        statsCanvas.transform.DOLocalMove(hidePanelPos, panelAnimationTime).SetEase(Ease.InQuad).OnComplete(() => { nextButton.SetActive(false); nextDayButton.SetActive(true); ShowBarks(); });
        //statsCanvas.SetActive(false);
        //nextButton.SetActive(false);
        //nextDayButton.SetActive(true);
        //ShowBarks();
    }

    public void ShowBarks()
    {
        List<string> anomItems = StatsController.Instance.GetAnomalousItemNames();
        ItemSO[] items = Resources.LoadAll<ItemSO>("");
        for (int i = 0; i < anomItems.Count; i++)
        {
            ItemSO item = items.FirstOrDefault(item => item.ItemName == anomItems[i]);
            GameObject prefabInstance = Instantiate(bubblePrefab, gridParent.transform.position, Quaternion.identity);
            prefabInstance.transform.SetParent(gridParent.transform);
            ThoughtBubble thoughtbubble = prefabInstance.GetComponent<ThoughtBubble>();

            int index;
            if (item.ItemName == "Model Spaceship")
            {
                index = 0;
            }
            else if (item.ItemName == "Bloodied Microscope")
            {
                index = 1;
            }
            else if (item.ItemName == "Infected Severed Arm")
            {
                index = 2;
            }
            else
            {
                index = 3;
            }
            thoughtbubble.InitializeThoughtBubble(i, item.Sprite[0], this, index);
        }

        if (anomItems.Count % 2 != 0)
        {
            GameObject prefabInstance = Instantiate(bubblePrefab, gridParent.transform.position, Quaternion.identity);
            prefabInstance.transform.SetParent(gridParent.transform);
            ThoughtBubble thoughtbubble = prefabInstance.GetComponent<ThoughtBubble>();
            thoughtbubble.InitializeEmptyBubble();
        }
    }

    public void StartPlayerReflection(int index)
    {
        gridParent.SetActive(false);
        string chosenDialogue = "";
        for (int i = 0; i < 4; i++)
        {
            if (index == i)
            {
                chosenDialogue = bark.sentences[i];
                anomalousItemImage.sprite = anomalousItemSprites[i];
            }
        }
        Color currentColor = anomalousItemImage.color;
        currentColor.a = 0;
        anomalousItemImage.color = currentColor; 
        
        anomalousItemImage.enabled = true;

        barkText.text = "";
        //StopAllCoroutines();
        barkCanvas.transform.DOLocalMove(dialogueShowPanelPos, panelAnimationTime).OnComplete(() => {StartCoroutine(RenderSentence(chosenDialogue, barkText, bark.name, bark.portrait, bark.talkingClip, barkCanvas)); if (fadeOutCoroutine != null)StopCoroutine(fadeOutCoroutine); fadeInCoroutine = StartCoroutine(FadeInVignette()); });

    }

    public void EndDialogue()
    {
        StopAllCoroutines();
        barkCanvas.transform.DOLocalMove(dialogueHidePanelPos, panelAnimationTime).OnComplete(() => {fadeOutCoroutine = StartCoroutine(FadeOutVignette()); gridParent.SetActive(true); });
    }

    IEnumerator RenderSentence(string sentence, TMP_Text textbox, string speakerName, Sprite speakerIcon, AudioClip audioClip, GameObject canvas)
    {
        
        playerName.enableAutoSizing = true;
        playerName.text = speakerName;
        dialogueSprite.sprite = speakerIcon; 
        //textbox.enableAutoSizing = true;
        textbox.text = sentence;
        textbox.ForceMeshUpdate();
        textbox.maxVisibleCharacters = 0;
        int totalChars = textbox.textInfo.characterCount;

        
        yield return new WaitForSeconds(.5f);

        for (int i = 0; i < totalChars; i++)
        {
            textbox.maxVisibleCharacters++;
            if (i % 4 == 0)
            {
                //source.volume = UnityEngine.Random.Range(0.5f, 1.0f);
                source.pitch = Random.Range(0.8f, 1.2f);
                source.PlayOneShot(audioClip);
            }

            yield return new WaitForSeconds(textSpeed);
        }

    }

    private IEnumerator FadeInVignette()
    {
        float targetIntensity = 0.6f;
        float fadeDuration = 1f;
        float startIntensity = vignette.intensity.value;
        float timeElapsed = 0f;
        float startAlpha = anomalousItemImage.color.a;


        while (timeElapsed < fadeDuration)
        {
            timeElapsed += Time.deltaTime;
            vignette.intensity.value = Mathf.Lerp(startIntensity, targetIntensity, timeElapsed / fadeDuration);
            
            Color currentColor = anomalousItemImage.color;
            currentColor.a = Mathf.Lerp(startAlpha, 1f, timeElapsed / fadeDuration);
            anomalousItemImage.color = currentColor;
            
            yield return null;
        }

        vignette.intensity.value = targetIntensity;

        Color finalColor = anomalousItemImage.color;
        finalColor.a = 1f;
        anomalousItemImage.color = finalColor;

    }

    private IEnumerator FadeOutVignette()
    {
        float targetIntensity = vignette.intensity.value;
        float fadeDuration = 1f;
        float timeElapsed = 0f;
        float startAlpha = anomalousItemImage.color.a;


        timeElapsed = 0f;
        while (timeElapsed < fadeDuration)
        {
            timeElapsed += Time.deltaTime;
            vignette.intensity.value = Mathf.Lerp(targetIntensity, 0f, timeElapsed / fadeDuration);

            Color currentColor = anomalousItemImage.color;
            currentColor.a = Mathf.Lerp(startAlpha, 0f, timeElapsed / fadeDuration);
            anomalousItemImage.color = currentColor;

            yield return null;
        }

        vignette.intensity.value = 0f;
        Color finalColor = anomalousItemImage.color;
        finalColor.a = 0f;
        anomalousItemImage.color = finalColor;
        anomalousItemImage.enabled = false;

    }

}
