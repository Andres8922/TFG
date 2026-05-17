using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Configuraci�n Actual")]
    public int heroeSeleccionado = 0;
    public int dificultad = 0;

    [Header("--- PROGRESO DE LA PARTIDA --- (Se borra al morir)")]
    public int oroTotal = 0;
    public List<ObjetoTienda> pocionesGlobales = new List<ObjetoTienda>();
    public List<Habilidad> habilidadesGlobales = new List<Habilidad>();

    [Header("--- META-PROGRESO --- (Para siempre)")]
    public int nivelCuenta = 1;
    public int experienciaActual = 0;
    public int experienciaNecesaria = 100;

    [Tooltip("Marca el primer hueco como TRUE (H�roe inicial). El resto se desbloquear�n solos.")]
    public bool[] heroesDesbloqueados = new bool[4] { true, false, false, false };

    [Header("--- ESTAD�STICAS DE LA RUN ---")]
    public int dañoTotalPartida = 0;
    public int manaTotalPartida = 0;
    public int turnosTotalesPartida = 0;
    public int xpTotalPartida = 0;
    public bool victoriaJefe = false;

    private float tiempoPartida = 0f;
    private bool cronometroActivo = false;

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

    void Update()
    {
        if (cronometroActivo) tiempoPartida += Time.deltaTime;
    }

    public void IniciarCronometro() { cronometroActivo = true; }
    public void PararCronometro()   { cronometroActivo = false; }
    public float ObtenerTiempo()    { return tiempoPartida; }

    public void GanarExperiencia(int cantidadXP)
    {
        xpTotalPartida += cantidadXP;
        experienciaActual += cantidadXP;
        Debug.Log("Has ganado " + cantidadXP + " XP. Total: " + experienciaActual + "/" + experienciaNecesaria);

        while (experienciaActual >= experienciaNecesaria)
        {
            SubirNivelCuenta();
        }
    }

    void SubirNivelCuenta()
    {
        experienciaActual -= experienciaNecesaria;
        nivelCuenta++;

        experienciaNecesaria = Mathf.RoundToInt(experienciaNecesaria * 1.5f);

        Debug.Log("¡NIVEL DE CUENTA " + nivelCuenta + " ALCANZADO!");

        if (nivelCuenta == 3 && heroesDesbloqueados.Length > 1 && !heroesDesbloqueados[1])
        {
            heroesDesbloqueados[1] = true;
            Debug.Log("¡NUEVO H�ROE DESBLOQUEADO: Arquero!");
        }
        else if (nivelCuenta == 5 && heroesDesbloqueados.Length > 2 && !heroesDesbloqueados[2])
        {
            heroesDesbloqueados[2] = true;
            Debug.Log("¡NUEVO H�ROE DESBLOQUEADO: Mago!");
        }
    }

    public void ResetearPartida()
    {
        oroTotal = 0;
        pocionesGlobales.Clear();
        habilidadesGlobales.Clear();

        dañoTotalPartida = 0;
        manaTotalPartida = 0;
        turnosTotalesPartida = 0;
        xpTotalPartida = 0;
        victoriaJefe = false;
        tiempoPartida = 0f;
        cronometroActivo = false;

        DatosGlobales.hayPartidaGuardada = false;
        DatosGlobales.semillaMapa = 0;
        DatosGlobales.pisoActualJugador = 0;
        DatosGlobales.nodoActualJugador = 0;
        DatosGlobales.nodosCompletados.Clear();
        DatosGlobales.oroJugador = 100;
        DatosGlobales.habilidadesJugador.Clear();
        DatosGlobales.combatesRealizados = 0;

        Debug.Log("Run terminada. Inventario y mapa reiniciados. Meta-Progreso intacto.");
    }
}
