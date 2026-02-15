using UnityEngine;

public class UnidadCombate : MonoBehaviour
{
    [Header("Datos Básicos")]
    public string nombreUnidad;
    public int dañoBase = 10;

    [Header("Vida ❤️")]
    public int vidaMax = 100;
    public int vidaActual;

    [Header("Maná 💧")]
    public int manaMax = 50;
    public int manaActual;
    public int manaPorTurno = 10; // Cuánto recupera al empezar su ronda

    void Awake()
    {
        // Al empezar, rellenamos vida y maná a tope (o a 0 el maná si prefieres)
        vidaActual = vidaMax;
        manaActual = 0; // Normalmente en estos juegos empiezas con poco maná
    }

    // Función para recibir daño
    public bool RecibirDaño(int daño)
    {
        vidaActual -= daño;
        if (vidaActual <= 0)
        {
            vidaActual = 0;
            return true; // Devuelve TRUE si ha muerto
        }
        return false; // Devuelve FALSE si sigue vivo
    }

    // Función para curar vida
    public void Curar(int cantidad)
    {
        vidaActual += cantidad;
        if (vidaActual > vidaMax) vidaActual = vidaMax;
    }

    // Función para gastar maná (retorna true si pudo gastarlo)
    public bool GastarMana(int coste)
    {
        if (manaActual >= coste)
        {
            manaActual -= coste;
            return true; // ¡Éxito!
        }
        return false; // ¡No hay suficiente maná!
    }

    // Función para recargar maná al inicio del turno
    public void RegenerarManaTurno()
    {
        manaActual += manaPorTurno;
        if (manaActual > manaMax) manaActual = manaMax;
    }
}