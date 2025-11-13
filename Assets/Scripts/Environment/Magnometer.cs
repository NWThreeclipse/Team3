using DG.Tweening.Core.Easing;
using UnityEngine;

public class Magnometer : DragZone
{

    [SerializeField] private GameManager gameManager;

    private void Start()
    {
        if (gameManager.GetDay() < 2)
        {
            gameObject.SetActive(false);
        }
    }
}
