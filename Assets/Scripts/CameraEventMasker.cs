using UnityEngine;

public class CameraEventMasker : MonoBehaviour
{

    private void Start()
    {
        var cam = GetComponent<Camera>();
        int ignoreMouseLayer = LayerMask.NameToLayer("IgnoreMouse");
        if (ignoreMouseLayer >= 0)
        {
            cam.eventMask &= ~(1 << ignoreMouseLayer);
        }
    }
}
