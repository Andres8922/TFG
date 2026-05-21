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
        if (GameManager.Instance != null)
        {
            if (textoNivel != null)
                textoNivel.text = "Nivel " + GameManager.Instance.nivelCuenta;

            if (textoNumerosXP != null)
                textoNumerosXP.text = GameManager.Instance.experienciaActual + " / " + GameManager.Instance.experienciaNecesaria;

            if (barraXP != null)
            {
                barraXP.maxValue = GameManager.Instance.experienciaNecesaria;
                barraXP.value = GameManager.Instance.experienciaActual;
            }
        }
        else
        {
            Debug.LogWarning("No hay GameManager en la escena.");
        }
    }
}
