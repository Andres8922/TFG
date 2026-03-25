using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public enum TipoNodo { Inicio, CombateFácil, CombateDifícil, Evento, Tienda, Jefe }

public class NodoMapa : MonoBehaviour, IPointerClickHandler
{
    public int piso;
    public int indiceHorizontal;
    public TipoNodo tipoDeNodo;

    public List<NodoMapa> conexionesSalientes = new List<NodoMapa>();

    // Aquí está la función completa, sin puntos suspensivos que rompan nada
    public void AnadirConexion(NodoMapa nodoDestino)
    {
        if (!conexionesSalientes.Contains(nodoDestino))
        {
            conexionesSalientes.Add(nodoDestino);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Hiciste clic en un nodo de tipo: " + tipoDeNodo);

        // Descomenta esta línea quitando las dos barras (//) cuando hayas podido poner el SceneLoader en la escena
        SceneLoader.Instance.CargarNivel(tipoDeNodo);
    }
}