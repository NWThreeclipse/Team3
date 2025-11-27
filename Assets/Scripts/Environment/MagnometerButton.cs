using UnityEngine;

public class MagnometerButton : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Magnometer magnometer;
    [SerializeField] private HelpMenuButton helpButton;


    private void Start()
    {
        if (gameManager.GetDay() < 2)
        {
            magnometer.gameObject.SetActive(false);
            helpButton.gameObject.SetActive(false);
            gameObject.SetActive(false);
            return;
        }
    }

    private void OnMouseDown()
    {
        magnometer.EnableMiniGame();
    }
}
