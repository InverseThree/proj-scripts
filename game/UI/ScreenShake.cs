using UnityEngine;
using Cinemachine;

public class ScreenShake : MonoBehaviour
{
    public CinemachineImpulseSource impulse;

    public void Shake()
    {
        impulse.GenerateImpulse();
    }
}
