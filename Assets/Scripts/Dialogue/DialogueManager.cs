using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using XNode;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private GameObject dialogueMenu;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text sentenceText;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button[] optionButtons;
    [SerializeField] private TMP_Text[] optionButtonsText;

    [SerializeField] private Image portrait;
    [SerializeField] private Vector3 showPanelPos;
    [SerializeField] private Vector3 hidePanelPos;
    [SerializeField] private float panelAnimationTime = 1;
    [SerializeField] private float textSpeed = 0.01f;
    [SerializeField] private Skooge skooge;
    [SerializeField] private GameObject supervisor;
    [SerializeField] private Transform[] showHidePositions;

    Node curNode;
    Queue<string> sentences = new Queue<string>();
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioSource footstepSource;
    AudioClip talkingClip;

    private Coroutine typingCoroutine;
    private bool isTyping;

    public event Action<DialogueManager> OnDialogueEnd;

    private void ShowSupervisor()
    {
        supervisor.SetActive(true);
        footstepSource.Play();
        supervisor.transform.DOMove(showHidePositions[0].position, 4f).OnComplete(() => footstepSource.Stop());
    }

    private void HideSupervisor()
    {
        supervisor.SetActive(true);
        footstepSource.Play();
        supervisor.transform.DOMove(showHidePositions[1].position, 5f).OnComplete(() => footstepSource.Stop());
    }


    public void StartDialogue(Node rootNode)
    {
        curNode = rootNode;

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
            isTyping = false;
        }

        if (curNode.name == "SupervisorEnter")
            ShowSupervisor();
        else if (curNode.name == "SupervisorExit")
            HideSupervisor();
        else if (curNode.name == "SkoogeEnter")
            skooge.PlayOpenVent();
        else if (curNode.name == "SkoogeExit")
            skooge.PlayCloseVent();

        if (curNode.GetType() == typeof(OptionDialogueNode))
        {
            OptionDialogueNode options = curNode as OptionDialogueNode;
            Dialogue dialogue = options.speaker;

            nameText.enableAutoSizing = true;
            nameText.text = dialogue.name;
            portrait.sprite = dialogue.portrait;
            AlignIcons(dialogue.name);
            talkingClip = dialogue.talkingClip;
            sentenceText.text = "";

            nextButton.gameObject.SetActive(false);

            for (int i = 0; i < optionButtons.Length; i++)
                optionButtons[i].gameObject.SetActive(false);

            for (int i = 0; i < options.responses.sentences.Length; i++)
            {
                optionButtons[i].gameObject.SetActive(true);
                optionButtonsText[i].text = options.responses.sentences[i];
            }

            sentences.Clear();
            foreach (var s in dialogue.sentences)
                sentences.Enqueue(s);

            dialogueMenu.transform
                .DOLocalMove(showPanelPos, panelAnimationTime)
                .OnComplete(DisplaySentence);
        }
        else if (curNode.GetType() == typeof(SimpleDialogueNode))
        {
            SimpleDialogueNode simple = curNode as SimpleDialogueNode;
            Dialogue dialogue = simple.sentence;

            nameText.text = dialogue.name;
            portrait.sprite = dialogue.portrait;
            AlignIcons(dialogue.name);
            talkingClip = dialogue.talkingClip;
            sentenceText.text = "";

            nextButton.gameObject.SetActive(true);

            for (int i = 0; i < optionButtons.Length; i++)
                optionButtons[i].gameObject.SetActive(false);

            sentences.Clear();
            foreach (var s in dialogue.sentences)
                sentences.Enqueue(s);

            dialogueMenu.transform
                .DOLocalMove(showPanelPos, panelAnimationTime)
                .OnComplete(DisplaySentence);
        }
        else
        {
            DialogueControlNode control = curNode as DialogueControlNode;

            if (control.dialogueControl == DialogueControlNode.option.endDialogue)
                EndDialogue();
        }
    }

    public void DisplayNextOption(string option)
    {
        if (isTyping)
        {
            sentenceText.maxVisibleCharacters = sentenceText.textInfo.characterCount;
            isTyping = false;
            return;
        }

        OptionDialogueNode optionNode = curNode as OptionDialogueNode;
        NodePort port = null;

        if (option == "A") port = optionNode.GetOutputPort("optionA");
        else if (option == "B") port = optionNode.GetOutputPort("optionB");
        else if (option == "C") port = optionNode.GetOutputPort("optionC");
        else if (option == "D") port = optionNode.GetOutputPort("optionD");

        if (port != null && port.Connection != null)
        {
            curNode = port.Connection.node;
            StartDialogue(curNode);
        }
    }

    public void DisplayNextSimple()
    {
        if (isTyping)
        {
            sentenceText.maxVisibleCharacters = sentenceText.textInfo.characterCount;
            isTyping = false;
            return;
        }

        SimpleDialogueNode simpleNode = curNode as SimpleDialogueNode;
        NodePort port = simpleNode.GetOutputPort("nextNode").Connection;

        if (port != null)
            curNode = port.node;

        StartDialogue(curNode);
    }

    public void DisplaySentence()
    {
        if (sentences.Count == 0)
            return;

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(RenderSentence(sentences.Dequeue()));
    }

    IEnumerator RenderSentence(string sentence)
    {
        isTyping = true;

        sentenceText.text = sentence;
        sentenceText.ForceMeshUpdate();
        sentenceText.maxVisibleCharacters = 0;

        int totalChars = sentenceText.textInfo.characterCount;

        for (int i = 0; i < totalChars; i++)
        {
            sentenceText.maxVisibleCharacters++;

            if (i % 4 == 0 && talkingClip != null)
            {
                source.pitch = UnityEngine.Random.Range(0.8f, 1.2f);
                source.PlayOneShot(talkingClip);
            }

            yield return new WaitForSeconds(textSpeed);
        }

        isTyping = false;
    }

    public void EndDialogue()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        dialogueMenu.transform.DOLocalMove(hidePanelPos, panelAnimationTime);
        OnDialogueEnd?.Invoke(this);
    }

    private void AlignIcons(string name)
    {
        Vector3 portraitPosition;
        Vector3 portraitScale;

        if (name == "Supervisor")
        {
            portraitPosition = new Vector3(0, -74.7f, 0);
            portraitScale = new Vector3(2.7f, 2.7f, 2.7f);
        }
        else if (name == "Xanthon" || name == "Unit 481202")
        {
            portraitPosition = new Vector3(0, -64, 0);
            portraitScale = new Vector3(2.5f, 2.5f, 2.5f);
        }
        else
        {
            portraitPosition = new Vector3(616, -174.9f, 0);
            portraitScale = new Vector3(20f, 20f, 20f);
        }

        portrait.transform.localPosition = portraitPosition;
        portrait.transform.localScale = portraitScale;
    }
}
