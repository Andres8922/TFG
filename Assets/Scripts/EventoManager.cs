using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.EventSystems; // Vital para detectar el ratón

[System.Serializable]
public class HuecoHabilidad
{
    public Button botonElegir;
    public TextMeshProUGUI textoMana;
    public Image icono;
    [HideInInspector] public Habilidad habilidadAsignada;
}

public class EventoManager : MonoBehaviour
{
    [Header("Catálogo de Habilidades")]
    public List<Habilidad> catalogoCompleto;

    [Header("Interfaz de las Cartas")]
    public HuecoHabilidad[] huecosHabilidad;
    public TextMeshProUGUI textoInstrucciones;

    [Header("Interfaz del Tooltip (Recuadro)")]
    public GameObject panelTooltip;
    public TextMeshProUGUI textoTooltipNombre;
    public TextMeshProUGUI textoTooltipInfo;

    private bool habilidadElegida = false;

    void Start()
    {
        // Nos aseguramos de que el tooltip empiece apagado
        if (panelTooltip != null) panelTooltip.SetActive(false);

        RellenarCartas();
    }

    void RellenarCartas()
    {
        List<Habilidad> disponibles = new List<Habilidad>(catalogoCompleto);

        for (int i = 0; i < huecosHabilidad.Length; i++)
        {
            if (disponibles.Count == 0) break;

            int rand = Random.Range(0, disponibles.Count);
            Habilidad habElegida = disponibles[rand];
            disponibles.RemoveAt(rand);

            huecosHabilidad[i].habilidadAsignada = habElegida;
            if (huecosHabilidad[i].textoMana != null) huecosHabilidad[i].textoMana.text = habElegida.costeMana.ToString();
            if (huecosHabilidad[i].icono != null) huecosHabilidad[i].icono.sprite = habElegida.iconoHabilidad;

            int indice = i;
            huecosHabilidad[i].botonElegir.onClick.RemoveAllListeners();
            huecosHabilidad[i].botonElegir.onClick.AddListener(() => SeleccionarHabilidad(indice));

            // ¡LA MAGIA DEL RATÓN! Le inyectamos por código el detector de Hover al botón
            BotonHoverCarta detectorHover = huecosHabilidad[i].botonElegir.gameObject.AddComponent<BotonHoverCarta>();
            detectorHover.indiceHueco = indice;
            detectorHover.manager = this; // Le decimos quién es el jefe
        }
    }

    public void MostrarTooltip(int indice)
    {
        if (habilidadElegida || panelTooltip == null) return;

        Habilidad hab = huecosHabilidad[indice].habilidadAsignada;

        textoTooltipNombre.text = hab.nombreHabilidad;
        textoTooltipInfo.text = hab.descripcion;

        panelTooltip.SetActive(true);
    }

    public void OcultarTooltip()
    {
        if (panelTooltip != null) panelTooltip.SetActive(false);
    }

    public void SeleccionarHabilidad(int indice)
    {
        if (habilidadElegida) return;

        Habilidad hab = huecosHabilidad[indice].habilidadAsignada;
        Debug.Log("¡Has aprendido la habilidad: " + hab.nombreHabilidad + "!");

        // --- CONEXIÓN CON EL CEREBRO GLOBAL ---
        if (GameManager.Instance != null)
        {
            GameManager.Instance.habilidadesGlobales.Add(hab);
            Debug.Log("Habilidad guardada en el GameManager.");
        }
        else
        {
            Debug.LogWarning("¡Falta el GameManager! Dale al Play desde el Menú Principal para guardar la habilidad.");
        }

        habilidadElegida = true;
        OcultarTooltip();

        if (textoInstrucciones != null)
        {
            textoInstrucciones.text = "¡Habilidad aprendida! Vuelve al mapa.";
        }

        for (int i = 0; i < huecosHabilidad.Length; i++)
        {
            huecosHabilidad[i].botonElegir.interactable = false;

            if (i != indice)
            {
                huecosHabilidad[i].icono.color = new Color(0.3f, 0.3f, 0.3f, 0.5f);
            }
            else
            {
                // Un pequeño toque visual para la elegida
                if (huecosHabilidad[i].textoMana != null)
                    huecosHabilidad[i].textoMana.text = "✓";
            }
        }
    }
}

// ---------------------------------------------------------
// ESTE ES EL PEQUEÑO ESPIA QUE DETECTA TU RATÓN
// ---------------------------------------------------------
public class BotonHoverCarta : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public int indiceHueco;
    public EventoManager manager;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (manager != null) manager.MostrarTooltip(indiceHueco);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (manager != null) manager.OcultarTooltip();
    }
}