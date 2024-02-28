using UnityEngine;

public class FrameRate : MonoBehaviour
{
    void Start()
    {
        QualitySettings.vSyncCount = 0;

        Application.targetFrameRate = 60;
    }
}
