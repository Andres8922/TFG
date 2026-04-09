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

    void Start()
    {
        ActualizarUIOro();
        RellenarEscaparate();
    }

    void RellenarEscaparate()
    {
        List<ObjetoTienda> objetosDisponibles = new List<ObjetoTienda>(catalogoCompleto);

        for (int i = 0; i < huecosTienda.Length; i++)
        {
            if (objetosDisponibles.Count == 0) break;

            int indiceRandom = Random.Range(0, objetosDisponibles.Count);
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
        ObjetoTienda objeto = huecosTienda[indiceHueco].objetoAsignado;

        // 1. Evitamos errores si pruebas la escena sin pasar por el Menú Principal
        if (GameManager.Instance == null)
        {
            Debug.LogWarning("¡Falta el GameManager! Dale al Play desde la escena del Menú Principal.");
            return;
        }

        // 2. Comprobamos si tienes suficiente oro en tu "Cerebro Global"
        if (GameManager.Instance.oroTotal >= objeto.precio)
        {
            // Cobramos
            GameManager.Instance.oroTotal -= objeto.precio;
            ActualizarUIOro();

            // Entregamos el objeto en la mochila global
            AplicarEfecto(objeto);

            // Efecto visual de "Agotado" en la tienda
            huecosTienda[indiceHueco].botonComprar.interactable = false;
            huecosTienda[indiceHueco].textoNombre.text = "AGOTADO";
            huecosTienda[indiceHueco].icono.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        }
        else
        {
            Debug.Log("¡No tienes suficiente oro para comprar " + objeto.nombreObjeto + "!");
        }
    }

    void AplicarEfecto(ObjetoTienda objeto)
    {
        Debug.Log("Has comprado: " + objeto.nombreObjeto);

        if (GameManager.Instance != null)
        {
            // Si ya tenías esta poción en la mochila, solo te sumamos 1 frasco más
            if (GameManager.Instance.pocionesGlobales.Contains(objeto))
            {
                objeto.cantidadEnInventario++;
            }
            // Si es un objeto nuevo, lo guardamos en la lista y le ponemos cantidad 1
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
            // Mostramos el oro real, o 0 si estás probando la escena suelta
            int oroActual = (GameManager.Instance != null) ? GameManager.Instance.oroTotal : 0;
            textoOro.text = "Oro: " + oroActual;
        }
    }
}