using UnityEngine;

public class DialController : MonoBehaviour
{
    [SerializeField] Transform handle;

    [SerializeField] private float minValue;
    [SerializeField] private float maxValue;
    [SerializeField] private bool isEnabled;

    private float currentValue;

    private bool isInteger;

    private Vector3 mousePos;

    public void InitializeDial(float minValue, float maxValue, bool isInteger)
    {
        this.minValue = minValue;
        this.maxValue = maxValue;
        this.isInteger = isInteger;

        currentValue = Mathf.Lerp(minValue, maxValue, 0.5f);
        currentValue = this.minValue;
        UpdateDialHandle();
    }

    public void OnDialDrag()
    {
        if (!isEnabled)
        {
            return;
        }
        mousePos = Input.mousePosition;
        Vector2 dir = mousePos - handle.position;

        float angle = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;
        angle = (angle <= 0) ? (360 + angle) : angle;

        if (angle <= 225 || angle >= 315)
        {
            Quaternion r = Quaternion.AngleAxis(angle + 135f, Vector3.forward);
            handle.rotation = r;

            angle = ((angle >= 315) ? (angle - 360) : angle) + 45;

            float normalizedAngle = Mathf.InverseLerp(0, 360, angle);

            currentValue = Mathf.Lerp(minValue, maxValue, normalizedAngle);

            if (isInteger)
            {
                currentValue = Mathf.Round(currentValue);
            }

            UpdateDialHandle();
        }
    }


    public void SetEnabled(bool toggle)
    {
        isEnabled = toggle;
    }
    public float GetCurrentValue()
    {
        return currentValue;
    }

    private void UpdateDialHandle()
    {
        float normalizedValue = Mathf.InverseLerp(minValue, maxValue, currentValue);
        float angle = Mathf.Lerp(0, 360, normalizedValue);

        Quaternion targetRotation = Quaternion.AngleAxis(angle - 45f, Vector3.forward);
        handle.rotation = targetRotation;
    }


}
