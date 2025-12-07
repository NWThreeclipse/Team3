using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;

public class BarkManager : MonoBehaviour
{
    [SerializeField] private GameObject skoogeBarkCanvas;
    [SerializeField] private TMP_Text skoogeText;
    [SerializeField] private float textSpeed = 0.01f;
    [SerializeField] private Dialogue skoogeDialogue;

    [SerializeField] private GameObject playerBarkCanvas;
    [SerializeField] private TMP_Text playerText;
    [SerializeField] private Dialogue playerDialogue;

    [SerializeField] private GameObject leftSupervisorCanvas;
    [SerializeField] private GameObject rightSupervisorCanvas;
    [SerializeField] private TMP_Text leftSupervisorText;
    [SerializeField] private TMP_Text rightSupervisorText;
    [SerializeField] private Dialogue supervisorDialogue;
    [SerializeField] private float shakeStrength = 0.05f;



    private bool isSupervisorBarking = false;



    private AudioSource source;

    private void Start()
    {
        source = GetComponent<AudioSource>();
        skoogeBarkCanvas.SetActive(false);
        playerBarkCanvas.SetActive(false);
        leftSupervisorCanvas.SetActive(false);
        rightSupervisorCanvas.SetActive(false);
    }

    public void StartSkoogeBark()
    {
        string chosenText = skoogeDialogue.sentences[Random.Range(0, skoogeDialogue.sentences.Length)];
        skoogeText.text = "";
        //StopAllCoroutines();
        StartCoroutine(RenderSentence(chosenText, skoogeText, skoogeDialogue.talkingClip, true, skoogeBarkCanvas));
        skoogeBarkCanvas.SetActive(true);
    }

    public void StartPlayerBark()
    {
        string chosenText = playerDialogue.sentences[StatsController.Instance.GetDays() - 2];
        playerText.text = "";
        //StopAllCoroutines();
        StartCoroutine(RenderSentence(chosenText, playerText, playerDialogue.talkingClip, false, playerBarkCanvas));
        playerBarkCanvas.SetActive(true);
    }

    public void StartSupervisorBark(bool leftSide, bool sortOutcome)
    {
        if (isSupervisorBarking)
        {
            return;
        }
        isSupervisorBarking = true;
        leftSupervisorCanvas.SetActive(leftSide);
        rightSupervisorCanvas.SetActive(!leftSide);

        leftSupervisorText.text = "";
        rightSupervisorText.text = "";
        //StopAllCoroutines();

        string chosenText = sortOutcome ? supervisorDialogue.sentences[Random.Range(0, 4)] : supervisorDialogue.sentences[Random.Range(5, 11)];

        StartCoroutine(RenderSentence(chosenText, leftSide ? leftSupervisorText : rightSupervisorText, supervisorDialogue.talkingClip, true, leftSide ? leftSupervisorCanvas : rightSupervisorCanvas));
    }

    public void HidePlayerBark()
    {
        playerBarkCanvas.SetActive(false);
    }



    IEnumerator RenderSentence(string sentence, TMP_Text textbox, AudioClip audioClip, bool autoFade, GameObject canvas)
    {

        if (!DOTween.IsTweening(canvas.gameObject))
        {
            //Vector3 originalPosition = canvas.gameObject.transform.position;
            canvas.gameObject.transform.DOShakePosition(shakeStrength, 0.1f);//.OnComplete(() => transform.DOMove(originalPosition, 0.1f));
        }
        textbox.text = "";
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
        isSupervisorBarking = false;
    }
}
