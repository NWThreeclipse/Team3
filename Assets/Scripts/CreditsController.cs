using UnityEngine;
using UnityEngine.UI;

public class CreditsController : MonoBehaviour
{
    [SerializeField] private GameObject creditsMenu;
    [SerializeField] private Image teamImage;
    [SerializeField] private Image qaImage;

    private void Start()
    {
        ToggleCanvas(false);
    }


    public void ToggleCanvas(bool toggle)
    {
        creditsMenu.SetActive(toggle);
        if (toggle)
        {
            qaImage.enabled = false;
            teamImage.enabled = true;
        }
        
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
}
