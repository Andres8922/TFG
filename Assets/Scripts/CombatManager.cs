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

    [Header("UI - Estandarte de Turnos")]
    public TMP_Text textoNumeroTurnoEstandarte;

    [Header("UI - Pantalla Fin de Combate (Arrastra los objetos)")]
    [Tooltip("El objeto padre 'PantallaFinCombate' que lo contiene todo.")]
    public GameObject panelFinCombate; 
    
    [Tooltip("El objeto de imagen completo para VICTORIA (Cartel_Victoria).")]
    public GameObject objetoCartelVictoria; 
    
    [Tooltip("El objeto de imagen completo para DERROTA (Cartel_Derrota).")]
    public GameObject objetoCartelDerrota; 

    public TMP_Text textoResumenTurnos; 
    public TMP_Text textoResumenOro;

    private int numeroTurno = 0;

    [HideInInspector] public UnidadCombate unidadHeroe;
    [HideInInspector] public UnidadCombate unidadEnemigo;

    private Animator animatorHeroe;
    private Animator animatorEnemigo; 

    void Awake() { Instance = this; }

    void Start()
    {
        estado = EstadoCombate.START;
        if (panelFinCombate != null) panelFinCombate.SetActive(false);
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
        if (unidadHeroe != null)
        {
            if (imagenVidaHeroe != null) imagenVidaHeroe.fillAmount = (float)unidadHeroe.vidaActual / unidadHeroe.vidaMaxima;
            if (textoNumerosVidaHeroe != null) textoNumerosVidaHeroe.text = unidadHeroe.vidaActual + " / " + unidadHeroe.vidaMaxima;
            if (imagenManaHeroe != null) imagenManaHeroe.fillAmount = (float)unidadHeroe.manaActual / unidadHeroe.manaMaximo;
            if (textoNumerosManaHeroe != null) textoNumerosManaHeroe.text = unidadHeroe.manaActual + " / " + unidadHeroe.manaMaximo;
        }

        if (unidadEnemigo != null)
        {
            if (imagenVidaEnemigo != null) imagenVidaEnemigo.fillAmount = (float)unidadEnemigo.vidaActual / unidadEnemigo.vidaMaxima;
            if (textoNumerosVidaEnemigo != null) textoNumerosVidaEnemigo.text = unidadEnemigo.vidaActual + " / " + unidadEnemigo.vidaMaxima;
        }

        if (textoNumeroTurnoEstandarte != null) textoNumeroTurnoEstandarte.text = numeroTurno.ToString("00");
    }

    void EmpezarTurnoJugador()
    {
        numeroTurno++;
        estado = EstadoCombate.TURNO_JUGADOR;
        unidadHeroe.RegenerarManaTurno();
        ActualizarUI();
    }

    public void BotonAtacar() { if (estado == EstadoCombate.TURNO_JUGADOR) StartCoroutine(AtacarEnemigo()); }

    // --- ¡AQUÍ ESTÁ LA FUNCIÓN RESTAURADA! ---
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

    // --- ¡AQUÍ ESTÁ LA CORRUTINA RESTAURADA! ---
    IEnumerator UsarHabilidadEspecial()
    {
        estado = EstadoCombate.START; // Bloquea botones
        ActualizarUI();

        if (animatorHeroe != null) animatorHeroe.SetTrigger("Atacar"); // Si tienes animación de habilidad, cámbiala aquí

        yield return new WaitForSeconds(1f);

        int dañoEspecial = unidadHeroe.dañoBase * 2; // Hace el doble de daño
        bool enemigoMuerto = unidadEnemigo.RecibirDaño(dañoEspecial);
        ActualizarUI();

        yield return new WaitForSeconds(1f);

        if (enemigoMuerto) FinalizarCombate(true);
        else
        {
            estado = EstadoCombate.TURNO_ENEMIGO;
            StartCoroutine(TurnoEnemigo());
        }
    }

    IEnumerator AtacarEnemigo()
    {
        estado = EstadoCombate.START;
        if (animatorHeroe != null) animatorHeroe.SetTrigger("Atacar");
        yield return new WaitForSeconds(0.5f); 
        bool enemigoMuerto = unidadEnemigo.RecibirDaño(unidadHeroe.dañoBase);
        ActualizarUI();
        yield return new WaitForSeconds(1f);

        if (enemigoMuerto) FinalizarCombate(true);
        else { estado = EstadoCombate.TURNO_ENEMIGO; StartCoroutine(TurnoEnemigo()); }
    }

    IEnumerator TurnoEnemigo()
    {
        yield return new WaitForSeconds(1f);
        if (animatorEnemigo != null) animatorEnemigo.SetTrigger("AtacarEnemigo"); 
        yield return new WaitForSeconds(0.5f); 
        bool heroeMuerto = unidadHeroe.RecibirDaño(unidadEnemigo.dañoBase);
        ActualizarUI();

        if (heroeMuerto) FinalizarCombate(false);
        else EmpezarTurnoJugador();
    }

    void FinalizarCombate(bool victoria)
    {
        StopAllCoroutines(); 
        estado = EstadoCombate.START; 

        if (panelFinCombate != null) panelFinCombate.SetActive(true);

        if (victoria)
        {
            estado = EstadoCombate.VICTORIA;
            if (objetoCartelVictoria != null) objetoCartelVictoria.SetActive(true);
            if (objetoCartelDerrota != null) objetoCartelDerrota.SetActive(false);
            
            if (textoResumenTurnos != null) textoResumenTurnos.text = numeroTurno.ToString("00");
            int oroGanado = Mathf.Max(100, 500 - (numeroTurno * 20)); 
            if (textoResumenOro != null) textoResumenOro.text = oroGanado.ToString("000");
        }
        else
        {
            estado = EstadoCombate.DERROTA;
            if (objetoCartelVictoria != null) objetoCartelVictoria.SetActive(false);
            if (objetoCartelDerrota != null) objetoCartelDerrota.SetActive(true);
            
            if (textoResumenTurnos != null) textoResumenTurnos.text = numeroTurno.ToString("00");
            if (textoResumenOro != null) textoResumenOro.text = "000"; 
        }
    }
}