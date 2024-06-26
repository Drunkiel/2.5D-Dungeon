using Cinemachine;
using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;

    public CinemachineVirtualCamera[] virtualCameras;

    private void Awake()
    {
        instance = this;
    }

    public void ResetZoom()
    {
        virtualCameras[0].m_Lens.FieldOfView = 80;
    }

    public IEnumerator ZoomTo(int number, float timeToEnd)
    {
        float startValue = virtualCameras[0].m_Lens.FieldOfView;
        float time = 0f;

        while (time < timeToEnd)
        {
            time += Time.deltaTime;
            virtualCameras[0].m_Lens.FieldOfView = Mathf.Lerp(startValue, number, time / timeToEnd);
            yield return null;
        }
    }
}
