using UnityEngine;
using TMPro;
using System.Collections;

public enum EstadoCombate { START, TURNO_JUGADOR, TURNO_ENEMIGO, VICTORIA, DERROTA }

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance;
    public EstadoCombate estado;

    [Header("Configuración")]
    public GameObject[] listaHeroes;
    public Transform puntoHeroe;
    public GameObject enemigoPrefab;
    public Transform puntoEnemigo;

    [Header("UI (Arrastra los textos aquí)")]
    public TMP_Text textoVidaHeroe;   // ✅ BIEN (Usa mayúsculas TMP_Text)
    public TMP_Text textoManaHeroe;   // ✅ BIEN
    public TMP_Text textoVidaEnemigo; // ✅ BIEN
    public TMP_Text textoTurnos;

    private int numeroTurno = 0;

    [HideInInspector] public UnidadCombate unidadHeroe;
    [HideInInspector] public UnidadCombate unidadEnemigo;

    void Awake() { Instance = this; }

    void Start()
    {
        estado = EstadoCombate.START;
        StartCoroutine(ConfigurarCombate());
    }

    IEnumerator ConfigurarCombate()
    {
        // 1. Crear Héroe
        int indice = 0;
        if (GameManager.Instance != null) indice = GameManager.Instance.heroeSeleccionado;

        GameObject heroeGO = Instantiate(listaHeroes[indice], puntoHeroe.position, Quaternion.identity);
        unidadHeroe = heroeGO.GetComponent<UnidadCombate>();

        // 2. Crear Enemigo
        GameObject enemigoGO = Instantiate(enemigoPrefab, puntoEnemigo.position, Quaternion.identity);
        unidadEnemigo = enemigoGO.GetComponent<UnidadCombate>();

        // ACTUALIZAR LA UI AL EMPEZAR
        ActualizarUI();

        yield return new WaitForSeconds(1f);
        EmpezarTurnoJugador();
    }

    // FUNCIÓN NUEVA PARA PINTAR LOS TEXTOS
    void ActualizarUI()
    {
        // 1. Comprobamos si el Héroe y sus textos existen antes de escribir
        if (unidadHeroe != null)
        {
            if (textoVidaHeroe != null) textoVidaHeroe.text = "HP: " + unidadHeroe.vidaActual;
            if (textoManaHeroe != null) textoManaHeroe.text = "MP: " + unidadHeroe.manaActual;
        }

        // 2. Comprobamos si el Enemigo y su texto existen
        if (unidadEnemigo != null)
        {
            if (textoVidaEnemigo != null) textoVidaEnemigo.text = "HP: " + unidadEnemigo.vidaActual;
        }

        // ACTUALIZAR CONTADOR DE TURNOS
        if (textoTurnos != null)
        {
            textoTurnos.text = "TURNO: " + numeroTurno;
        }
    }

    void EmpezarTurnoJugador()
    {
        numeroTurno++; // <--- SUMAMOS 1 AL TURNO
        estado = EstadoCombate.TURNO_JUGADOR;

        unidadHeroe.RegenerarManaTurno();
        ActualizarUI(); // Esto actualizará el texto en pantalla

        Debug.Log("¡Tu turno! Turno número: " + numeroTurno);
    }

    public void BotonAtacar()
    {
        if (estado != EstadoCombate.TURNO_JUGADOR) return;
        StartCoroutine(AtacarEnemigo());
    }

    public void BotonHabilidad()
    {
        // 1. ¿Es mi turno?
        if (estado != EstadoCombate.TURNO_JUGADOR) return;

        // 2. ¿Tengo maná suficiente? (Digamos que cuesta 10)
        int coste = 10;

        if (unidadHeroe.manaActual >= coste)
        {
            // ¡SÍ PUEDO!
            unidadHeroe.GastarMana(coste); // Restamos el maná
            StartCoroutine(UsarHabilidadEspecial()); // Lanzamos el ataque
        }
        else
        {
            Debug.Log("¡No tienes suficiente maná! Necesitas " + coste);
            // Aquí luego podríamos poner un sonido de error
        }
    }

    IEnumerator UsarHabilidadEspecial()
    {
        estado = EstadoCombate.START; // Bloqueamos botones

        // Actualizamos la UI inmediatamente para ver que bajó el maná
        ActualizarUI();

        Debug.Log("¡Lanzando Habilidad Especial! ⚡");
        yield return new WaitForSeconds(1f); // Pequeña pausa dramática

        // Hacemos EL DOBLE de daño que un ataque normal
        int dañoEspecial = unidadHeroe.dañoBase * 2;

        bool enemigoMuerto = unidadEnemigo.RecibirDaño(dañoEspecial);
        ActualizarUI(); // Actualizamos la vida del enemigo

        yield return new WaitForSeconds(1f);

        if (enemigoMuerto)
        {
            estado = EstadoCombate.VICTORIA;
            FinalizarCombate(true);
        }
        else
        {
            estado = EstadoCombate.TURNO_ENEMIGO;
            StartCoroutine(TurnoEnemigo());
        }
    }

    IEnumerator AtacarEnemigo()
    {
        estado = EstadoCombate.START; // Bloqueamos para que no pulses 2 veces

        // Daño
        bool enemigoMuerto = unidadEnemigo.RecibirDaño(unidadHeroe.dañoBase);
        ActualizarUI(); // <--- ¡Actualizamos la vida del enemigo en pantalla!

        yield return new WaitForSeconds(1f);

        if (enemigoMuerto)
        {
            estado = EstadoCombate.VICTORIA;
            FinalizarCombate(true);
        }
        else
        {
            estado = EstadoCombate.TURNO_ENEMIGO;
            StartCoroutine(TurnoEnemigo());
        }
    }

    IEnumerator TurnoEnemigo()
    {
        yield return new WaitForSeconds(1f);

        bool heroeMuerto = unidadHeroe.RecibirDaño(unidadEnemigo.dañoBase);
        ActualizarUI(); // <--- ¡Actualizamos nuestra vida en pantalla!

        if (heroeMuerto)
        {
            estado = EstadoCombate.DERROTA;
            FinalizarCombate(false);
        }
        else
        {
            EmpezarTurnoJugador();
        }
    }

    void FinalizarCombate(bool victoria)
    {
        if (victoria) Debug.Log("¡GANASTE!"); // Aquí luego pondremos volver al mapa
        else Debug.Log("¡PERDISTE!");
    }
}