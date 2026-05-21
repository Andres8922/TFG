using UnityEngine;
using System.Collections.Generic;

public enum TipoNodo { Inicio, CombateFacil, CombateDificil, Tienda, Evento, Jefe, Vacio }

public class NodoMapa : MonoBehaviour
{
    public int pisoIndex;
    public int nodoIndex;
    public TipoNodo tipoDeNodo;
    public List<NodoMapa> nodosConectados = new List<NodoMapa>();

    void OnMouseDown()
    {
        if (GeneradorMapa.Instance != null)
            GeneradorMapa.Instance.IntentarMoverJugador(this);
    }
}
