using UnityEngine;
using System.Collections.Generic; // Esto es vital para usar Listas

public enum TipoNodo { Inicio, CombateFacil, CombateDificil, Tienda, Evento, Jefe, Vacio }

public class NodoMapa : MonoBehaviour
{
    public int pisoIndex;
    public int nodoIndex;
    public TipoNodo tipoDeNodo;

    // Aquí es donde el Generador anota si hay línea o no
    public List<NodoMapa> nodosConectados = new List<NodoMapa>();

    void OnMouseDown()
    {
        if (GeneradorMapa.Instance != null)
        {
            GeneradorMapa.Instance.IntentarMoverJugador(this);
        }
    }
}