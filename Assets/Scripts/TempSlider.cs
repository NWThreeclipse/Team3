using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TempSlider : MonoBehaviour
{
    public TMP_Text text;
    public Slider slider;

    private void Update()
    {
        text.text = ((int)slider.value).ToString();

    }
}
