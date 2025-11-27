using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class RingUIController : MonoBehaviour
{
    [SerializeField] private Image ring;
    [SerializeField] private Skooge skooge;

    private void Start()
    {
        ring.fillAmount = 0;
        ring.gameObject.SetActive(false);
       
    }

    private void FixedUpdate()
    {
        if (skooge.GetIsItemStaying())
        {
            if (skooge.GetCurrentQuest().IsComplete())
            {
                return;
            }
            ring.gameObject.SetActive(true);
            float[] data = skooge.GetHoldTimeInfo();
            if (data[0] > data[1])
            {
                data[0] = data[1];
            }
            ring.fillAmount = data[0] / data[1];
            ring.gameObject.transform.position = Input.mousePosition;

        }
        else
        {
            ring.gameObject.SetActive(false);
        }
    }


}
