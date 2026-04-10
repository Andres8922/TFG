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

    // Esta función la llamará el CombatManager cuando ganes
    public void GanarExperiencia(int cantidadXP)
    {
        experienciaActual += cantidadXP;
        Debug.Log("Has ganado " + cantidadXP + " XP. Total: " + experienciaActual + "/" + experienciaNecesaria);

        // Usamos un 'while' por si ganas muchísima XP de golpe y subes 2 niveles a la vez
        while (experienciaActual >= experienciaNecesaria)
        {
            SubirNivelCuenta();
        }
    }

    void SubirNivelCuenta()
    {
        experienciaActual -= experienciaNecesaria; // Restamos la XP usada, pero conservamos el sobrante
        nivelCuenta++;

        // Hacemos que cada nivel cueste un 50% más que el anterior para que sea un reto
        experienciaNecesaria = Mathf.RoundToInt(experienciaNecesaria * 1.5f);

        Debug.Log("¡NIVEL DE CUENTA " + nivelCuenta + " ALCANZADO!");

        // LÓGICA DE DESBLOQUEO DE HÉROES
        // Ej: Al nivel 3 se desbloquea el Héroe 2 (índice 1)
        if (nivelCuenta == 3 && heroesDesbloqueados.Length > 1 && !heroesDesbloqueados[1])
        {
            heroesDesbloqueados[1] = true;
            Debug.Log("¡NUEVO HÉROE DESBLOQUEADO: Arquero!");
        }
        // Ej: Al nivel 5 se desbloquea el Héroe 3 (índice 2)
        else if (nivelCuenta == 5 && heroesDesbloqueados.Length > 2 && !heroesDesbloqueados[2])
        {
            heroesDesbloqueados[2] = true;
            Debug.Log("¡NUEVO HÉROE DESBLOQUEADO: Mago!");
        }
    }

    // Fíjate que esta función YA NO borra la experiencia ni los héroes
    public void ResetearPartida()
    {
        oroTotal = 0;
        pocionesGlobales.Clear();
        habilidadesGlobales.Clear();
        Debug.Log("Run terminada. Inventario reiniciado. Meta-Progreso intacto.");
    }
}