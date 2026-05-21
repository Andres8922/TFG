using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Configuración Actual")]
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

    [Tooltip("Marca el primer hueco como TRUE (Héroe inicial). El resto se desbloquearán solos.")]
    public bool[] heroesDesbloqueados = new bool[4] { true, true, true, false };

    [Header("--- MEJORAS DE LA RUN --- (Se borran al acabar)")]
    public int bonusDaño = 0;
    public int bonusVida = 0;

    [Header("--- ESTADÍSTICAS DE LA RUN ---")]
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
            CargarMetaProgreso();
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

    void CargarMetaProgreso()
    {
        nivelCuenta         = PlayerPrefs.GetInt("nivelCuenta", 1);
        experienciaActual   = PlayerPrefs.GetInt("experienciaActual", 0);
        experienciaNecesaria = PlayerPrefs.GetInt("experienciaNecesaria", 100);

        for (int i = 0; i < heroesDesbloqueados.Length; i++)
            heroesDesbloqueados[i] = PlayerPrefs.GetInt("heroe_" + i, i < 3 ? 1 : 0) == 1;
    }

    void GuardarMetaProgreso()
    {
        PlayerPrefs.SetInt("nivelCuenta", nivelCuenta);
        PlayerPrefs.SetInt("experienciaActual", experienciaActual);
        PlayerPrefs.SetInt("experienciaNecesaria", experienciaNecesaria);

        for (int i = 0; i < heroesDesbloqueados.Length; i++)
            PlayerPrefs.SetInt("heroe_" + i, heroesDesbloqueados[i] ? 1 : 0);

        PlayerPrefs.Save();
    }

    public void GanarExperiencia(int cantidadXP)
    {
        xpTotalPartida += cantidadXP;
        experienciaActual += cantidadXP;

        while (experienciaActual >= experienciaNecesaria)
        {
            SubirNivelCuenta();
        }

        GuardarMetaProgreso();
    }

    void SubirNivelCuenta()
    {
        experienciaActual -= experienciaNecesaria;
        nivelCuenta++;
        experienciaNecesaria = Mathf.RoundToInt(experienciaNecesaria * 1.5f);

        // El nivel 2 desbloquea al Enano
        if (nivelCuenta == 2 && heroesDesbloqueados.Length > 3 && !heroesDesbloqueados[3])
            heroesDesbloqueados[3] = true;
    }

    public void ResetearPartida()
    {
        oroTotal = 0;
        foreach (ObjetoTienda pocion in pocionesGlobales)
            if (pocion != null) pocion.cantidadEnInventario = 0;
        pocionesGlobales.Clear();
        habilidadesGlobales.Clear();

        bonusDaño = 0;
        bonusVida = 0;

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

    }

    [ContextMenu("Borrar datos guardados (Meta-Progreso)")]
    void BorrarDatosGuardados()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        nivelCuenta = 1;
        experienciaActual = 0;
        experienciaNecesaria = 100;
        for (int i = 0; i < heroesDesbloqueados.Length; i++)
            heroesDesbloqueados[i] = i < 3;

    }
}
