using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;

public class SystemVolumeController : MonoBehaviour
{
    [DllImport("user32.dll")]
    private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

    private const byte VK_VOLUME_DOWN = 0xAE;
    private const byte VK_VOLUME_UP   = 0xAF;
    private const byte VK_VOLUME_MUTE = 0xAD;
    private const uint KEYEVENTF_EXTENDEDKEY = 0x0001;
    private const uint KEYEVENTF_KEYUP       = 0x0002;

    public Slider sliderVol;
    public bool controlSystemVolume = false;

    void Start()
    {
        if (sliderVol != null)
            sliderVol.onValueChanged.AddListener(OnVolumeChanged);
    }

    void OnVolumeChanged(float value)
    {
        if (controlSystemVolume)
            AdjustSystemVolume(value);
    }

    public void DecreaseSystemVolume()
    {
        keybd_event(VK_VOLUME_DOWN, 0, KEYEVENTF_EXTENDEDKEY, 0);
        keybd_event(VK_VOLUME_DOWN, 0, KEYEVENTF_KEYUP, 0);
    }

    public void IncreaseSystemVolume()
    {
        keybd_event(VK_VOLUME_UP, 0, KEYEVENTF_EXTENDEDKEY, 0);
        keybd_event(VK_VOLUME_UP, 0, KEYEVENTF_KEYUP, 0);
    }

    public void ToggleMute()
    {
        keybd_event(VK_VOLUME_MUTE, 0, KEYEVENTF_EXTENDEDKEY, 0);
        keybd_event(VK_VOLUME_MUTE, 0, KEYEVENTF_KEYUP, 0);
    }

    // Windows no expone API directa para leer/escribir el volumen del sistema,
    // así que aproximamos mediante pulsaciones de tecla
    private void AdjustSystemVolume(float normalizedValue)
    {
        if (normalizedValue < 0.5f)
            DecreaseSystemVolume();
        else
            IncreaseSystemVolume();
    }

    public void OnVolumeDownButtonPressed()
    {
        DecreaseSystemVolume();
    }
}
