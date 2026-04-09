using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Datos de Configuración")]
    public int heroeSeleccionado;
    public int dificultad;

    [Header("Memoria de Partida (Inventario Global)")]
    public int oroTotal = 0;

    public List<ObjetoTienda> pocionesGlobales = new List<ObjetoTienda>();

    // Ahora esta lista guarda TODAS las habilidades (tanto pasivas como ataques activos)
    public List<Habilidad> habilidadesGlobales = new List<Habilidad>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ResetearPartida()
    {
        oroTotal = 0;
        pocionesGlobales.Clear();
        habilidadesGlobales.Clear();
        Debug.Log("Partida reseteada. Todo el inventario global vuelve a cero.");
    }
}