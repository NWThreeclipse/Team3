using UnityEngine;

public class ThermalButton : MonoBehaviour
{
    [SerializeField] private ThermalScanner thermalScanner;
    [SerializeField] private GameManager gameManager;

    private void Start()
    {
        if (gameManager.GetDay() < 1)
        {
            thermalScanner.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }
    }
    private void OnMouseDown()
    {
        thermalScanner.EnableMiniGame();
    }
}
