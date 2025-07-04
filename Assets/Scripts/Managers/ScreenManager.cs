using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    void Start()
    {
        Screen.SetResolution(2436, 1125, false);
        Camera.main.orthographicSize = 8f;
    }
}
