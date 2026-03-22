using UnityEngine;
using UnityEngine.UI; // IMPRESCINDIBLE

public class FullScreenManager : MonoBehaviour
{
    // Esta es la función que conectaremos al Toggle
    public void CambiarPantallaCompletaToggle(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
        Debug.Log("Pantalla Completa establecida a: " + isFullScreen);
        // Aquí podrías añadir una función para reproducir un efecto de sonido
    }
}