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

    // Esta funci�n la llamar� el CombatManager cuando ganes
    public void GanarExperiencia(int cantidadXP)
    {
        experienciaActual += cantidadXP;
        Debug.Log("Has ganado " + cantidadXP + " XP. Total: " + experienciaActual + "/" + experienciaNecesaria);

        // Usamos un 'while' por si ganas much�sima XP de golpe y subes 2 niveles a la vez
        while (experienciaActual >= experienciaNecesaria)
        {
            SubirNivelCuenta();
        }
    }

    void SubirNivelCuenta()
    {
        experienciaActual -= experienciaNecesaria; // Restamos la XP usada, pero conservamos el sobrante
        nivelCuenta++;

        // Hacemos que cada nivel cueste un 50% m�s que el anterior para que sea un reto
        experienciaNecesaria = Mathf.RoundToInt(experienciaNecesaria * 1.5f);

        Debug.Log("�NIVEL DE CUENTA " + nivelCuenta + " ALCANZADO!");

        // L�GICA DE DESBLOQUEO DE H�ROES
        // Ej: Al nivel 3 se desbloquea el H�roe 2 (�ndice 1)
        if (nivelCuenta == 3 && heroesDesbloqueados.Length > 1 && !heroesDesbloqueados[1])
        {
            heroesDesbloqueados[1] = true;
            Debug.Log("�NUEVO H�ROE DESBLOQUEADO: Arquero!");
        }
        // Ej: Al nivel 5 se desbloquea el H�roe 3 (�ndice 2)
        else if (nivelCuenta == 5 && heroesDesbloqueados.Length > 2 && !heroesDesbloqueados[2])
        {
            heroesDesbloqueados[2] = true;
            Debug.Log("�NUEVO H�ROE DESBLOQUEADO: Mago!");
        }
    }

    // F�jate que esta funci�n YA NO borra la experiencia ni los h�roes
    public void ResetearPartida()
    {
        // Resetear progreso de la run (inventario y oro)
        oroTotal = 0;
        pocionesGlobales.Clear();
        habilidadesGlobales.Clear();

        // Resetear el mapa para que se genere uno nuevo la pr�xima run
        DatosGlobales.hayPartidaGuardada = false;
        DatosGlobales.semillaMapa = 0;
        DatosGlobales.pisoActualJugador = 0;
        DatosGlobales.nodoActualJugador = 0;
        DatosGlobales.nodosCompletados.Clear();
        DatosGlobales.oroJugador = 100;
        DatosGlobales.habilidadesJugador.Clear();

        Debug.Log("Run terminada. Inventario y mapa reiniciados. Meta-Progreso intacto.");
    }
}