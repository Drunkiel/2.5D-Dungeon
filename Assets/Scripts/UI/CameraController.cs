using Cinemachine;
using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;

    private int currentCamera;
    public CinemachineVirtualCamera[] virtualCameras;

    private void Awake()
    {
        instance = this;
    }

    public void SetCamera(int index)
    {
        if (index >= virtualCameras.Length)
            return;

        for (int i = 0; i < virtualCameras.Length; i++)
        {
            if (i != index)
                virtualCameras[i].Priority = 1;
            else
            {
                currentCamera = index;
                virtualCameras[index].Priority = 99;
            }
        }
    }

    public void ResetZoom()
    {
        virtualCameras[currentCamera].m_Lens.FieldOfView = 80;
    }

    public IEnumerator ZoomTo(int number, float timeToEnd)
    {
        float startValue = virtualCameras[0].m_Lens.FieldOfView;
        float time = 0f;

        while (time < timeToEnd)
        {
            time += Time.deltaTime;
            virtualCameras[currentCamera].m_Lens.FieldOfView = Mathf.Lerp(startValue, number, time / timeToEnd);
            yield return null;
        }
    }
}
