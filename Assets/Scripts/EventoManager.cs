using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.EventSystems;

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

    [Header("Panel de Intercambio")]
    public GameObject panelIntercambio;
    public SlotIntercambio[] slotsIntercambio = new SlotIntercambio[5];

    [Header("Límites de Inventario")]
    public int maxActivasAdquiribles = 3;
    public int maxPasivas = 5;

    [Header("Fondo por Héroe")]
    public Image fondoPantalla;
    public Sprite[] fondosPorHeroe;

    private bool habilidadElegida = false;
    private Habilidad habPendiente = null;
    private int indicePendiente = -1;

    void Start()
    {
        if (panelTooltip != null) panelTooltip.SetActive(false);
        if (panelIntercambio != null) panelIntercambio.SetActive(false);

        AplicarFondoHeroe();
        RellenarCartas();
    }

    void AplicarFondoHeroe()
    {
        if (fondoPantalla == null || fondosPorHeroe == null) return;
        int indice = (GameManager.Instance != null) ? GameManager.Instance.heroeSeleccionado : 0;
        if (indice >= 0 && indice < fondosPorHeroe.Length && fondosPorHeroe[indice] != null)
            fondoPantalla.sprite = fondosPorHeroe[indice];
    }

    void RellenarCartas()
    {
        int heroeActual = (GameManager.Instance != null) ? GameManager.Instance.heroeSeleccionado : -1;
        List<Habilidad> disponibles = new List<Habilidad>();
        foreach (Habilidad h in catalogoCompleto)
        {
            if (h.esPasiva || h.heroeRequerido == -1 || h.heroeRequerido == heroeActual)
                disponibles.Add(h);
        }

        System.Random rng = new System.Random();
        for (int i = 0; i < huecosHabilidad.Length; i++)
        {
            if (disponibles.Count == 0) break;

            int rand = rng.Next(0, disponibles.Count);
            Habilidad habElegida = disponibles[rand];
            disponibles.RemoveAt(rand);

            huecosHabilidad[i].habilidadAsignada = habElegida;
            if (huecosHabilidad[i].textoMana != null) huecosHabilidad[i].textoMana.text = habElegida.costeMana.ToString();
            if (huecosHabilidad[i].icono != null) huecosHabilidad[i].icono.sprite = habElegida.iconoHabilidad;

            int indice = i;
            huecosHabilidad[i].botonElegir.onClick.RemoveAllListeners();
            huecosHabilidad[i].botonElegir.onClick.AddListener(() => SeleccionarHabilidad(indice));

            BotonHoverCarta detectorHover = huecosHabilidad[i].botonElegir.gameObject.AddComponent<BotonHoverCarta>();
            detectorHover.indiceHueco = indice;
            detectorHover.manager = this;
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

        if (GameManager.Instance == null) return;

        int countActivas = 0, countPasivas = 0;
        foreach (var h in GameManager.Instance.habilidadesGlobales)
        {
            if (h.esPasiva) countPasivas++;
            else countActivas++;
        }

        bool lleno = hab.esPasiva ? countPasivas >= maxPasivas : countActivas >= maxActivasAdquiribles;

        if (lleno)
        {
            habPendiente = hab;
            indicePendiente = indice;
            AbrirPanelIntercambio(hab.esPasiva);
        }
        else
        {
            ConfirmarSeleccion(hab, indice);
        }
    }

    void AbrirPanelIntercambio(bool esPasiva)
    {
        if (panelIntercambio == null) return;

        foreach (var hueco in huecosHabilidad)
            hueco.botonElegir.interactable = false;

        OcultarTooltip();

        List<Habilidad> listaActual = new List<Habilidad>();
        foreach (var h in GameManager.Instance.habilidadesGlobales)
            if (h.esPasiva == esPasiva) listaActual.Add(h);

        int offset = esPasiva ? 0 : 1;

        for (int i = 0; i < slotsIntercambio.Length; i++)
        {
            if (slotsIntercambio[i].boton == null) continue;

            if (!esPasiva && (i == 0 || i == slotsIntercambio.Length - 1))
            {
                slotsIntercambio[i].boton.gameObject.SetActive(false);
                continue;
            }

            int listaIndex = i - offset;

            if (listaIndex >= 0 && listaIndex < listaActual.Count)
            {
                slotsIntercambio[i].boton.gameObject.SetActive(true);
                if (slotsIntercambio[i].icono != null) slotsIntercambio[i].icono.sprite = listaActual[listaIndex].iconoHabilidad;

                Habilidad habAEliminar = listaActual[listaIndex];
                slotsIntercambio[i].boton.onClick.RemoveAllListeners();
                slotsIntercambio[i].boton.onClick.AddListener(() => ConfirmarIntercambio(habAEliminar));
            }
            else
            {
                slotsIntercambio[i].boton.gameObject.SetActive(false);
            }
        }

        panelIntercambio.SetActive(true);
    }

    public void ConfirmarIntercambio(Habilidad habAEliminar)
    {
        if (habPendiente == null) return;

        GameManager.Instance.habilidadesGlobales.Remove(habAEliminar);
        panelIntercambio.SetActive(false);

        ConfirmarSeleccion(habPendiente, indicePendiente);

        habPendiente = null;
        indicePendiente = -1;
    }

    public void CancelarIntercambio()
    {
        habPendiente = null;
        indicePendiente = -1;
        if (panelIntercambio != null) panelIntercambio.SetActive(false);

        if (!habilidadElegida)
        {
            foreach (var hueco in huecosHabilidad)
                hueco.botonElegir.interactable = true;
        }
    }

    void ConfirmarSeleccion(Habilidad hab, int indice)
    {
        GameManager.Instance.habilidadesGlobales.Add(hab);

        habilidadElegida = true;
        OcultarTooltip();

        if (textoInstrucciones != null)
            textoInstrucciones.text = "¡Habilidad aprendida! Vuelve al mapa.";

        for (int i = 0; i < huecosHabilidad.Length; i++)
        {
            huecosHabilidad[i].botonElegir.interactable = false;

            if (i != indice)
                huecosHabilidad[i].icono.color = new Color(0.3f, 0.3f, 0.3f, 0.5f);
            else
            {
                if (huecosHabilidad[i].textoMana != null)
                    huecosHabilidad[i].textoMana.text = "✓";
            }
        }
    }
}

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
