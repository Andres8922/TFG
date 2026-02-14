using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ControlResolucionBotones : MonoBehaviour
{
    // Lista de resoluciones predefinidas
    private List<Vector2Int> resoluciones = new List<Vector2Int>()
    {
        new Vector2Int(640, 480),    // VGA
        new Vector2Int(800, 600),    // SVGA
        new Vector2Int(1024, 768),   // XGA
        new Vector2Int(1280, 720),   // HD
        new Vector2Int(1366, 768),   // HD común en laptops
        new Vector2Int(1600, 900),   // HD+
        new Vector2Int(1920, 1080),  // Full HD
        new Vector2Int(2560, 1440),  // 2K
        new Vector2Int(3840, 2160)   // 4K
    };

    // Índice de resolución actual
    private int indiceActual = 4; // Empieza en 1366x768

    // Referencia para mostrar resolución actual (opcional)
    public Text textoResolucionActual;

    void Start()
    {
        // Establecer resolución inicial
        AplicarResolucionActual();

        // Actualizar texto si existe
        ActualizarTextoResolucion();
    }

    // Método para AUMENTAR resolución (botón +)
    public void AumentarResolucion()
    {
        if (indiceActual < resoluciones.Count - 1)
        {
            indiceActual++;
            AplicarResolucionActual();
            ActualizarTextoResolucion();
            Debug.Log("Resolución aumentada");
        }
        else
        {
            Debug.Log("Ya estás en la resolución máxima");
        }
    }

    // Método para DISMINUIR resolución (botón -)
    public void DisminuirResolucion()
    {
        if (indiceActual > 0)
        {
            indiceActual--;
            AplicarResolucionActual();
            ActualizarTextoResolucion();
            Debug.Log("Resolución disminuida");
        }
        else
        {
            Debug.Log("Ya estás en la resolución mínima");
        }
    }

    // Aplicar la resolución actual
    private void AplicarResolucionActual()
    {
        Vector2Int resolucion = resoluciones[indiceActual];
        Screen.SetResolution(resolucion.x, resolucion.y, Screen.fullScreen);
    }

    // Actualizar texto que muestra la resolución
    private void ActualizarTextoResolucion()
    {
        if (textoResolucionActual != null)
        {
            Vector2Int resolucion = resoluciones[indiceActual];
            textoResolucionActual.text = $"{resolucion.x} × {resolucion.y}";
        }
    }

    // Método para obtener resolución actual como string
    public string ObtenerResolucionActual()
    {
        Vector2Int resolucion = resoluciones[indiceActual];
        return $"{resolucion.x} × {resolucion.y}";
    }

    // Método para restablecer a resolución por defecto
    public void ResetearResolucion()
    {
        indiceActual = 4; // 1366x768
        AplicarResolucionActual();
        ActualizarTextoResolucion();
        Debug.Log("Resolución restablecida");
    }
}