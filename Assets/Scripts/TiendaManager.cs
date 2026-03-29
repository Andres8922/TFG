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

        // ¡AHORA COMPROBAMOS EL ORO GLOBAL!
        if (DatosGlobales.oroJugador >= objeto.precio)
        {
            DatosGlobales.oroJugador -= objeto.precio;
            ActualizarUIOro();
            AplicarEfecto(objeto);

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
        // (Aquí mantienes el switch de efectos que tenías)
    }

    void ActualizarUIOro()
    {
        if (textoOro != null)
        {
            textoOro.text = "Oro: " + DatosGlobales.oroJugador; // Actualiza con el oro global
        }
    }
}