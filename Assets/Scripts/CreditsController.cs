using DG.Tweening;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class CreditsController : MonoBehaviour
{
    [SerializeField] private GameObject creditsMenu;
    [SerializeField] private Image teamImage;
    [SerializeField] private Image qaImage;

    [SerializeField] private AudioClip panelOpen, panelClose;
    [SerializeField] private Vector3 showPanelPos;
    [SerializeField] private Vector3 hidePanelPos;
    [SerializeField] private float panelAnimationTime = 1;

    [SerializeField] private MusicController musicController;

    private void Start()
    {
        ToggleCanvas(false);
    }


    public void ToggleCanvas(bool toggle)
    {
        if (toggle)
        {
            qaImage.enabled = false;
            teamImage.enabled = true;
        }
        creditsMenu.transform.DOLocalMove(toggle ? showPanelPos : hidePanelPos, panelAnimationTime).SetEase(toggle ? Ease.OutQuad : Ease.InQuad);
    }
    
    public void ToggleTeam(bool toggle)
    {
        teamImage.enabled = toggle;
        qaImage.enabled = !toggle;

    }

    public void ToggleQA(bool toggle)
    {
        qaImage.enabled = toggle;
        teamImage.enabled = !toggle;

    }

    private void Awake()
    {
        musicController.FadeInMusic();
    }
}
