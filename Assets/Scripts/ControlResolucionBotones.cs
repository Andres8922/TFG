using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ControlResolucionBotones : MonoBehaviour
{
    private List<Vector2Int> resoluciones = new List<Vector2Int>()
    {
        new Vector2Int(640,  480),   // VGA
        new Vector2Int(800,  600),   // SVGA
        new Vector2Int(1024, 768),   // XGA
        new Vector2Int(1280, 720),   // HD
        new Vector2Int(1366, 768),   // HD común en laptops
        new Vector2Int(1600, 900),   // HD+
        new Vector2Int(1920, 1080),  // Full HD
        new Vector2Int(2560, 1440),  // 2K
        new Vector2Int(3840, 2160)   // 4K
    };

    private int indiceActual = 4; // 1366x768 por defecto

    public Text textoResolucionActual;

    void Start()
    {
        AplicarResolucionActual();
        ActualizarTextoResolucion();
    }

    public void AumentarResolucion()
    {
        if (indiceActual < resoluciones.Count - 1)
        {
            indiceActual++;
            AplicarResolucionActual();
            ActualizarTextoResolucion();
        }
    }

    public void DisminuirResolucion()
    {
        if (indiceActual > 0)
        {
            indiceActual--;
            AplicarResolucionActual();
            ActualizarTextoResolucion();
        }
    }

    private void AplicarResolucionActual()
    {
        Vector2Int res = resoluciones[indiceActual];
        Screen.SetResolution(res.x, res.y, Screen.fullScreen);
    }

    private void ActualizarTextoResolucion()
    {
        if (textoResolucionActual != null)
        {
            Vector2Int res = resoluciones[indiceActual];
            textoResolucionActual.text = $"{res.x} x {res.y}";
        }
    }

    public string ObtenerResolucionActual()
    {
        Vector2Int res = resoluciones[indiceActual];
        return $"{res.x} x {res.y}";
    }

    public void ResetearResolucion()
    {
        indiceActual = 4;
        AplicarResolucionActual();
        ActualizarTextoResolucion();
    }
}
