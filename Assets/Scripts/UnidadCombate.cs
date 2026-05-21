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

    // Devuelve true si la unidad muere
    public bool RecibirDaño(int daño)
    {
        vidaActual -= daño;
        if (vidaActual <= 0)
        {
            vidaActual = 0;
            return true;
        }
        return false;
    }

    public void Curar(int cantidad)
    {
        vidaActual += cantidad;
        if (vidaActual > vidaMaxima) vidaActual = vidaMaxima;
    }

    public bool GastarMana(int coste)
    {
        if (manaActual >= coste)
        {
            manaActual -= coste;
            return true;
        }
        return false;
    }

    public void RegenerarManaTurno()
    {
        manaActual += manaPorTurno;
        if (manaActual > manaMaximo) manaActual = manaMaximo;
    }
}
