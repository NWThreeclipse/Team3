using UnityEngine;

public class CreditsController : MonoBehaviour
{
    [SerializeField] private GameObject creditsMenu;

    private void Awake()
    {
        ToggleCanvas(false);
    }

    public void ToggleCanvas(bool toggle)
    {
        creditsMenu.SetActive(toggle);
    }
}
