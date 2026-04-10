using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuPrincipalManager : MonoBehaviour
{
    [Header("UI Meta-Progreso")]
    public Slider barraXP;
    public TMP_Text textoNivel;
    public TMP_Text textoNumerosXP;

    void Start()
    {
        ActualizarUIExperiencia();
    }

    public void ActualizarUIExperiencia()
    {
        // Comprobamos que nuestro cerebro global existe
        if (GameManager.Instance != null)
        {
            // 1. Actualizamos los textos
            if (textoNivel != null)
                textoNivel.text = "Nivel " + GameManager.Instance.nivelCuenta;

            if (textoNumerosXP != null)
                textoNumerosXP.text = GameManager.Instance.experienciaActual + " / " + GameManager.Instance.experienciaNecesaria;

            // 2. Actualizamos el relleno de la barra
            if (barraXP != null)
            {
                // Le decimos a la barra cuál es el tope (ej: 100)
                barraXP.maxValue = GameManager.Instance.experienciaNecesaria;
                // Le decimos cuánto relleno poner (ej: 80)
                barraXP.value = GameManager.Instance.experienciaActual;
            }
        }
        else
        {
            Debug.LogWarning("No hay GameManager en la escena. Asegúrate de que existe.");
        }
    }

    // --- (Aquí puedes poner tus funciones para los botones de Empezar Partida, Salir, etc.) ---
}