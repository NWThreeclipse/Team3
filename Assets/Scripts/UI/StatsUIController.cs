using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class StatsUIController : MonoBehaviour
{
    [SerializeField] private GameObject statsCanvas;
    [SerializeField] private TMP_Text totalItems;
    [SerializeField] private TMP_Text correctItems;
    [SerializeField] private TMP_Text incorrectItems;
    [SerializeField] private TMP_Text sortSpeed;
    //[SerializeField] private TMP_Text anomalousItems;
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
    [SerializeField] private float panelAnimationTime = 1;

    [SerializeField] private float shakeStrength;
    [SerializeField] private MusicController musicController;

    private AudioSource source;


    private void Awake()
    {
        musicController.FadeInMusic();

    }
    private void Start()
    {
        totalItems.text = StatsController.Instance.GetItems().ToString();
        correctItems.text = StatsController.Instance.GetCorrectItems().ToString();
        incorrectItems.text = StatsController.Instance.GetIncorrectItems().ToString();
        if (StatsController.Instance.GetItems() == 0)
        {
            sortSpeed.text = "0";
        } else
        {
            sortSpeed.text = (150 / StatsController.Instance.GetItems()).ToString();
        }
        //anomalousItems.text = StatsController.Instance.GetAnomalousItems().ToString();
        source = GetComponent<AudioSource>();
        barkCanvas.SetActive(false);
        //statsCanvas.SetActive(true);
        statsCanvas.transform.DOLocalMove(showPanelPos, panelAnimationTime).SetEase(Ease.OutQuad);

    }

    public void HideStats()
    {
        statsCanvas.transform.DOLocalMove(hidePanelPos, panelAnimationTime).SetEase(Ease.InQuad).OnComplete(() => { nextButton.SetActive(false); nextDayButton.SetActive(true); ShowBarks();});
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
        string chosenDialogue;

        if (index == 0)
        {
            chosenDialogue = bark.sentences[0];
        }
        else if (index == 1)
        {
            chosenDialogue = bark.sentences[1];

        }
        else if (index == 2)
        {
            chosenDialogue = bark.sentences[2];
        }
        else
        {
            chosenDialogue = bark.sentences[3];
        }
        barkText.text = "";
        //StopAllCoroutines();
        StartCoroutine(RenderSentence(chosenDialogue, barkText, bark.talkingClip, true, barkCanvas));
        barkCanvas.SetActive(true);

    }
    IEnumerator RenderSentence(string sentence, TMP_Text textbox, AudioClip audioClip, bool autoFade, GameObject canvas)
    {
        gridParent.SetActive(false);
        textbox.text = "";

        if (!DOTween.IsTweening(canvas.gameObject))
        {
            Vector3 originalPosition = canvas.gameObject.transform.position;
            canvas.GetComponent<RectTransform>().DOShakeAnchorPos(0.1f, shakeStrength).OnComplete(() => canvas.transform.position = originalPosition);
        }
        yield return new WaitForSeconds(.5f);

        char[] letters = sentence.ToCharArray();
        for (int i = 0; i < letters.Length; i++)
        {
            textbox.text += letters[i];
            if (i % 4 == 0)
            {
                source.volume = UnityEngine.Random.Range(0.5f, 1.0f);
                source.pitch = UnityEngine.Random.Range(0.8f, 1.2f);
                source.PlayOneShot(audioClip);
            }

            yield return new WaitForSeconds(textSpeed);
        }

        if (autoFade)
        {
            yield return new WaitForSeconds(2f);
            canvas.SetActive(false);

        }
        gridParent.SetActive(true);
    }
}
