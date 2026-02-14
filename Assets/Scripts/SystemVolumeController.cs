using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;

public class SystemVolumeController : MonoBehaviour
{
    // Importar funciones de Windows API
    [DllImport("user32.dll")]
    private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

    // Códigos de teclas
    private const byte VK_VOLUME_DOWN = 0xAE;
    private const byte VK_VOLUME_UP = 0xAF;
    private const byte VK_VOLUME_MUTE = 0xAD;
    private const uint KEYEVENTF_EXTENDEDKEY = 0x0001;
    private const uint KEYEVENTF_KEYUP = 0x0002;

    // Referencia al slider
    public Slider sliderVol;

    // Variable para controlar si queremos ajustar volumen del sistema
    public bool controlSystemVolume = false;

    void Start()
    {
        if (sliderVol != null)
        {
            // Configurar listener para el slider
            sliderVol.onValueChanged.AddListener(OnVolumeChanged);
        }
    }

    void OnVolumeChanged(float value)
    {
        if (controlSystemVolume)
        {
            AdjustSystemVolume(value);
        }
    }

    // Método para bajar el volumen del sistema
    public void DecreaseSystemVolume()
    {
        // Simular pulsación de tecla VOLUMEN ABAJO
        keybd_event(VK_VOLUME_DOWN, 0, KEYEVENTF_EXTENDEDKEY, 0);
        keybd_event(VK_VOLUME_DOWN, 0, KEYEVENTF_KEYUP, 0);

        // Actualizar slider si existe
        if (sliderVol != null)
        {
            // Obtener volumen actual del sistema para actualizar slider
            // (esto requeriría más código para leer el volumen actual)
        }
    }

    // Método para subir el volumen del sistema
    public void IncreaseSystemVolume()
    {
        keybd_event(VK_VOLUME_UP, 0, KEYEVENTF_EXTENDEDKEY, 0);
        keybd_event(VK_VOLUME_UP, 0, KEYEVENTF_KEYUP, 0);
    }

    // Método para mute/desmute
    public void ToggleMute()
    {
        keybd_event(VK_VOLUME_MUTE, 0, KEYEVENTF_EXTENDEDKEY, 0);
        keybd_event(VK_VOLUME_MUTE, 0, KEYEVENTF_KEYUP, 0);
    }

    // Método para ajustar volumen basado en slider
    private void AdjustSystemVolume(float normalizedValue)
    {
        // Convertir valor normalizado (0-1) a número de pulsaciones
        // Esto es aproximado ya que Windows no expone API directa para esto
        int targetVolume = Mathf.RoundToInt(normalizedValue * 50);

        // Obtener volumen actual (necesitarías leerlo primero)
        // Por simplicidad, aquí solo ajustamos con pulsaciones
        if (normalizedValue < 0.5f)
        {
            DecreaseSystemVolume();
        }
        else
        {
            IncreaseSystemVolume();
        }
    }

    // Método para asignar desde un botón UI
    public void OnVolumeDownButtonPressed()
    {
        DecreaseSystemVolume();
    }
}