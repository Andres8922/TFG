using UnityEngine;

public class UnidadCombate : MonoBehaviour
{
    [Header("Datos Básicos")]
    public string nombreUnidad;
    public int dañoBase = 10;

    [Header("Vida ❤️")]
    public int vidaMaxima = 100;
    public int vidaActual;

    [Header("Maná 💧")]
    public int manaMaximo = 50;
    public int manaActual;
    public int manaPorTurno = 10;

    void Awake()
    {
        vidaActual = vidaMaxima;
        manaActual = 0;
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
        if (vidaActual > vidaMaxima) vidaActual = vidaMaxima;
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
        if (manaActual > manaMaximo) manaActual = manaMaximo;
    }
}