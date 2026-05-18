using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

[System.Serializable]
public class HuecoTienda
{
    public Button botonComprar;
    public TextMeshProUGUI textoNombre;
    public TextMeshProUGUI textoPrecio;
    public Image icono;
    [HideInInspector] public ObjetoTienda objetoAsignado;
}

public class TiendaManager : MonoBehaviour
{
    [Header("Catálogo de la Tienda")]
    public List<ObjetoTienda> catalogoCompleto;

    [Header("Interfaz de la Tienda")]
    public HuecoTienda[] huecosTienda;
    public TextMeshProUGUI textoOro;

    [Header("Visualización del Héroe")]
    public Transform posicionHeroe;
    public GameObject[] listaHeroesPrefab;

    [Header("Panel de Intercambio")]
    public GameObject panelIntercambio;
    public SlotIntercambio[] slotsIntercambio = new SlotIntercambio[5];

    [Header("Límites de Inventario")]
    public int maxTiposPociones = 5;

    private int pendingHueco = -1;

    void Start()
    {
        if (panelIntercambio != null) panelIntercambio.SetActive(false);
        ActualizarUIOro();
        RellenarEscaparate();
        InvocarHeroeEnTienda();
    }

    void InvocarHeroeEnTienda()
    {
        if (GameManager.Instance != null && listaHeroesPrefab != null && listaHeroesPrefab.Length > 0 && posicionHeroe != null)
        {
            int indiceElegido = GameManager.Instance.heroeSeleccionado;
            if (indiceElegido >= 0 && indiceElegido < listaHeroesPrefab.Length)
                Instantiate(listaHeroesPrefab[indiceElegido], posicionHeroe.position, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("Falta asignar los Prefabs de los héroes o la Posición en el TiendaManager.");
        }
    }

    void RellenarEscaparate()
    {
        int semilla = DatosGlobales.semillaMapa
            + DatosGlobales.pisoActualJugador * 1000
            + DatosGlobales.nodoActualJugador * 100;
        System.Random rng = new System.Random(semilla);

        List<ObjetoTienda> objetosDisponibles = new List<ObjetoTienda>(catalogoCompleto);

        for (int i = 0; i < huecosTienda.Length; i++)
        {
            if (objetosDisponibles.Count == 0) break;

            int indiceRandom = rng.Next(0, objetosDisponibles.Count);
            ObjetoTienda objetoElegido = objetosDisponibles[indiceRandom];
            objetosDisponibles.RemoveAt(indiceRandom);

            huecosTienda[i].objetoAsignado = objetoElegido;
            huecosTienda[i].textoNombre.text = objetoElegido.nombreObjeto;
            huecosTienda[i].textoPrecio.text = objetoElegido.precio.ToString();
            huecosTienda[i].icono.sprite = objetoElegido.iconoObjeto;

            int indiceHueco = i;
            huecosTienda[i].botonComprar.onClick.RemoveAllListeners();
            huecosTienda[i].botonComprar.onClick.AddListener(() => IntentarComprar(indiceHueco));
        }
    }

    public void IntentarComprar(int indiceHueco)
    {
        if (pendingHueco >= 0) return;

        ObjetoTienda objeto = huecosTienda[indiceHueco].objetoAsignado;

        if (GameManager.Instance == null)
        {
            Debug.LogWarning("¡Falta el GameManager! Dale al Play desde la escena del Menú Principal.");
            return;
        }

        if (GameManager.Instance.oroTotal < objeto.precio)
        {
            Debug.Log("¡No tienes suficiente oro para comprar " + objeto.nombreObjeto + "!");
            return;
        }

        bool esMejoraPermanente = objeto.tipoEfecto == TipoEfectoTienda.MejoraFuerza ||
                                   objeto.tipoEfecto == TipoEfectoTienda.MejoraDefensa;

        bool esNuevoTipo = !esMejoraPermanente && !GameManager.Instance.pocionesGlobales.Contains(objeto);
        bool mochilasLlena = GameManager.Instance.pocionesGlobales.Count >= maxTiposPociones;

        if (esNuevoTipo && mochilasLlena)
        {
            pendingHueco = indiceHueco;
            AbrirPanelIntercambio();
            return;
        }

        EjecutarCompra(indiceHueco);
    }

    void ReactivarHuecosNoVendidos()
    {
        foreach (var h in huecosTienda)
            if (h.botonComprar != null && h.textoNombre != null && h.textoNombre.text != "AGOTADO")
                h.botonComprar.interactable = true;
    }

    void AbrirPanelIntercambio()
    {
        if (panelIntercambio == null) return;

        foreach (var h in huecosTienda)
            if (h.botonComprar != null) h.botonComprar.interactable = false;

        for (int i = 0; i < slotsIntercambio.Length; i++)
        {
            if (slotsIntercambio[i].boton == null) continue;

            if (i < GameManager.Instance.pocionesGlobales.Count)
            {
                ObjetoTienda pocionActual = GameManager.Instance.pocionesGlobales[i];
                slotsIntercambio[i].boton.gameObject.SetActive(true);
                if (slotsIntercambio[i].icono != null) slotsIntercambio[i].icono.sprite = pocionActual.iconoObjeto;

                int idx = i;
                slotsIntercambio[i].boton.onClick.RemoveAllListeners();
                slotsIntercambio[i].boton.onClick.AddListener(() => ConfirmarIntercambio(idx));
            }
            else
            {
                slotsIntercambio[i].boton.gameObject.SetActive(false);
            }
        }

        panelIntercambio.SetActive(true);
    }

    public void ConfirmarIntercambio(int indiceEnLista)
    {
        if (pendingHueco < 0) return;

        ObjetoTienda pocionAEliminar = GameManager.Instance.pocionesGlobales[indiceEnLista];
        pocionAEliminar.cantidadEnInventario = 0;
        GameManager.Instance.pocionesGlobales.RemoveAt(indiceEnLista);

        if (panelIntercambio != null) panelIntercambio.SetActive(false);

        EjecutarCompra(pendingHueco);
        pendingHueco = -1;
        ReactivarHuecosNoVendidos();
    }

    public void CancelarIntercambio()
    {
        pendingHueco = -1;
        if (panelIntercambio != null) panelIntercambio.SetActive(false);
        ReactivarHuecosNoVendidos();
    }

    void EjecutarCompra(int indiceHueco)
    {
        ObjetoTienda objeto = huecosTienda[indiceHueco].objetoAsignado;

        GameManager.Instance.oroTotal -= objeto.precio;
        ActualizarUIOro();
        AplicarEfecto(objeto);

        huecosTienda[indiceHueco].botonComprar.interactable = false;
        huecosTienda[indiceHueco].textoNombre.text = "AGOTADO";
        huecosTienda[indiceHueco].icono.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
    }

    void AplicarEfecto(ObjetoTienda objeto)
    {
        Debug.Log("Has comprado: " + objeto.nombreObjeto);

        if (GameManager.Instance == null) return;

        if (objeto.tipoEfecto == TipoEfectoTienda.MejoraFuerza)
        {
            GameManager.Instance.bonusDaño += objeto.valorEfecto;
        }
        else if (objeto.tipoEfecto == TipoEfectoTienda.MejoraDefensa)
        {
            GameManager.Instance.bonusVida += objeto.valorEfecto;
        }
        else
        {
            if (GameManager.Instance.pocionesGlobales.Contains(objeto))
                objeto.cantidadEnInventario++;
            else
            {
                objeto.cantidadEnInventario = 1;
                GameManager.Instance.pocionesGlobales.Add(objeto);
            }
        }
    }

    void ActualizarUIOro()
    {
        if (textoOro != null)
        {
            int oroActual = (GameManager.Instance != null) ? GameManager.Instance.oroTotal : 0;
            textoOro.text = "" + oroActual;
        }
    }
}
