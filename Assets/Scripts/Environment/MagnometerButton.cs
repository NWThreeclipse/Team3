using DG.Tweening;
using UnityEngine;

public class MagnometerButton : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Magnometer magnometer;
    [SerializeField] private float shakeStrength = 0.05f;
    private Vector3 originalPosition;




    private void Start()
    {
        if (gameManager.GetDay() < 2)
        {
            magnometer.gameObject.SetActive(false);
            gameObject.SetActive(false);
            return;
        }
        originalPosition = transform.position;


    }

    private void OnMouseDown()
    {
        magnometer.EnableMiniGame();
        if (!DOTween.IsTweening(gameObject))
        {
            transform.DOShakePosition(shakeStrength, 0.1f).OnComplete(() => transform.DOMove(originalPosition, 0.1f));
        }
        AudioController.PlayButton();
    }
}
