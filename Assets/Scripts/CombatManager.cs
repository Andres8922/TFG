using UnityEngine;
using UnityEngine.UI;
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

    [Header("UI - Barras de Estado (Arrastra los RELLENOS)")]
    public Image imagenVidaHeroe;
    public Image imagenManaHeroe;
    public Image imagenVidaEnemigo;

    [Header("UI - Textos de Números (Arrastra los TEXTOS)")]
    public TMP_Text textoNumerosVidaHeroe;
    public TMP_Text textoNumerosManaHeroe;
    public TMP_Text textoNumerosVidaEnemigo;

    [Header("UI - Textos Generales")]
    public TMP_Text textoTurnos;

    private int numeroTurno = 0;

    [HideInInspector] public UnidadCombate unidadHeroe;
    [HideInInspector] public UnidadCombate unidadEnemigo;

    private Animator animatorHeroe;
    private Animator animatorEnemigo; 

    void Awake() { Instance = this; }

    void Start()
    {
        estado = EstadoCombate.START;
        StartCoroutine(ConfigurarCombate());
    }

    IEnumerator ConfigurarCombate()
    {
        int indice = 0;
        if (GameManager.Instance != null) indice = GameManager.Instance.heroeSeleccionado;

        GameObject heroeGO = Instantiate(listaHeroes[indice], puntoHeroe.position, Quaternion.identity);
        unidadHeroe = heroeGO.GetComponent<UnidadCombate>();
        animatorHeroe = heroeGO.GetComponent<Animator>();

        GameObject enemigoGO = Instantiate(enemigoPrefab, puntoEnemigo.position, Quaternion.identity);
        unidadEnemigo = enemigoGO.GetComponent<UnidadCombate>();
        animatorEnemigo = enemigoGO.GetComponent<Animator>(); 

        ActualizarUI();

        yield return new WaitForSeconds(1f);
        EmpezarTurnoJugador();
    }

    void ActualizarUI()
    {
        // --- UI DEL HÉROE ---
        if (unidadHeroe != null)
        {
            if (imagenVidaHeroe != null)
            {
                float porcentajeVida = (float)unidadHeroe.vidaActual / unidadHeroe.vidaMaxima;
                imagenVidaHeroe.fillAmount = porcentajeVida;
            }
            if (textoNumerosVidaHeroe != null)
            {
                textoNumerosVidaHeroe.text = unidadHeroe.vidaActual + " / " + unidadHeroe.vidaMaxima;
            }

            if (imagenManaHeroe != null)
            {
                float porcentajeMana = (float)unidadHeroe.manaActual / unidadHeroe.manaMaximo; 
                imagenManaHeroe.fillAmount = porcentajeMana;
            }
            if (textoNumerosManaHeroe != null)
            {
                textoNumerosManaHeroe.text = unidadHeroe.manaActual + " / " + unidadHeroe.manaMaximo;
            }
        }

        // --- UI DEL ENEMIGO ---
        if (unidadEnemigo != null)
        {
            if (imagenVidaEnemigo != null)
            {
                float porcentajeEnemigo = (float)unidadEnemigo.vidaActual / unidadEnemigo.vidaMaxima;
                imagenVidaEnemigo.fillAmount = porcentajeEnemigo;
            }
            if (textoNumerosVidaEnemigo != null)
            {
                textoNumerosVidaEnemigo.text = unidadEnemigo.vidaActual + " / " + unidadEnemigo.vidaMaxima;
            }
        }

        // --- TURNOS ---
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

        yield return new WaitForSeconds(0.5f); 

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

        if (animatorEnemigo != null)
        {
            animatorEnemigo.SetTrigger("AtacarEnemigo"); 
        }

        yield return new WaitForSeconds(0.5f); 

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