using UnityEngine;

public class ThermalButton : MonoBehaviour
{
    [SerializeField] private ThermalScanner thermalScanner;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private HelpMenuButton helpButton;


    private void Start()
    {
        if (gameManager.GetDay() < 1)
        {
            thermalScanner.gameObject.SetActive(false);
            helpButton.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }
    }
    private void OnMouseDown()
    {
        thermalScanner.EnableMiniGame();
    }
}
