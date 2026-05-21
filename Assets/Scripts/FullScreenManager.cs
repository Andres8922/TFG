using UnityEngine;
using UnityEngine.UI;

public class FullScreenManager : MonoBehaviour
{
    public void CambiarPantallaCompletaToggle(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }
}
