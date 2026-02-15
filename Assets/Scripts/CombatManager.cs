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
    public TMP_Text textoVidaHeroe;
    public TMP_Text textoManaHeroe;
    public TMP_Text textoVidaEnemigo;
    public TMP_Text textoTurnos;

    private int numeroTurno = 0;

    [HideInInspector] public UnidadCombate unidadHeroe;
    [HideInInspector] public UnidadCombate unidadEnemigo;

    private Animator animatorHeroe;
    private Animator animatorEnemigo; // ✅ Referencia para el monstruo

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
        animatorHeroe = heroeGO.GetComponent<Animator>();

        // 2. Crear Enemigo
        GameObject enemigoGO = Instantiate(enemigoPrefab, puntoEnemigo.position, Quaternion.identity);
        unidadEnemigo = enemigoGO.GetComponent<UnidadCombate>();
        animatorEnemigo = enemigoGO.GetComponent<Animator>(); // ✅ Capturamos el animator del monstruo

        ActualizarUI();

        yield return new WaitForSeconds(1f);
        EmpezarTurnoJugador();
    }

    void ActualizarUI()
    {
        if (unidadHeroe != null)
        {
            if (textoVidaHeroe != null) textoVidaHeroe.text = "HP: " + unidadHeroe.vidaActual;
            if (textoManaHeroe != null) textoManaHeroe.text = "MP: " + unidadHeroe.manaActual;
        }

        if (unidadEnemigo != null)
        {
            if (textoVidaEnemigo != null) textoVidaEnemigo.text = "HP: " + unidadEnemigo.vidaActual;
        }

        if (textoTurnos != null)
        {
            textoTurnos.text = "TURNO: " + numeroTurno;
        }
    }

    void EmpezarTurnoJugador()
    {
        numeroTurno++;
        estado = EstadoCombate.TURNO_JUGADOR;
        unidadHeroe.RegenerarManaTurno();
        ActualizarUI();
        Debug.Log("¡Tu turno! Turno número: " + numeroTurno);
    }

    public void BotonAtacar()
    {
        if (estado != EstadoCombate.TURNO_JUGADOR) return;
        StartCoroutine(AtacarEnemigo());
    }

    public void BotonHabilidad()
    {
        if (estado != EstadoCombate.TURNO_JUGADOR) return;

        int coste = 10;
        if (unidadHeroe.manaActual >= coste)
        {
            unidadHeroe.GastarMana(coste);
            StartCoroutine(UsarHabilidadEspecial());
        }
        else
        {
            Debug.Log("¡No tienes suficiente maná!");
        }
    }

    IEnumerator UsarHabilidadEspecial()
    {
        estado = EstadoCombate.START;
        ActualizarUI();

        if (animatorHeroe != null) animatorHeroe.SetTrigger("Atacar");

        yield return new WaitForSeconds(1f);

        int dañoEspecial = unidadHeroe.dañoBase * 2;
        bool enemigoMuerto = unidadEnemigo.RecibirDaño(dañoEspecial);
        ActualizarUI();

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
        estado = EstadoCombate.START;

        if (animatorHeroe != null)
        {
            animatorHeroe.SetTrigger("Atacar");
        }

        yield return new WaitForSeconds(0.5f); // Pausa breve para que se vea el golpe

        bool enemigoMuerto = unidadEnemigo.RecibirDaño(unidadHeroe.dañoBase);
        ActualizarUI();

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

        // ✅ ¡EL MONSTRUO ATACA!
        if (animatorEnemigo != null)
        {
            animatorEnemigo.SetTrigger("AtacarEnemigo"); // Asegúrate de que el trigger en el Animator se llame así
        }

        yield return new WaitForSeconds(0.5f); // Esperamos a que el monstruo lance el golpe

        bool heroeMuerto = unidadHeroe.RecibirDaño(unidadEnemigo.dañoBase);
        ActualizarUI();

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
        if (victoria) Debug.Log("¡GANASTE!");
        else Debug.Log("¡PERDISTE!");
    }
}