using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

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

    [Header("Transiciones de Escenas")]
    public string nombreEscenaMapa = "Mapa";
    public string nombreEscenaMenu = "MenuPrincipal";

    [Header("UI - Barras de Estado")]
    public Image imagenVidaHeroe;
    public Image imagenManaHeroe;
    public Image imagenVidaEnemigo;

    [Header("UI - Textos de Números")]
    public TMP_Text textoNumerosVidaHeroe;
    public TMP_Text textoNumerosManaHeroe;
    public TMP_Text textoNumerosVidaEnemigo;

    [Header("UI - Estandarte de Turnos")]
    public TMP_Text textoNumeroTurnoEstandarte;

    [Header("UI - Menú Pociones")]
    public Button[] botonesPociones;
    public TMP_Text[] textosCantidadPociones;

    [Header("UI - Menú Pasivas")]
    public Button[] botonesPasivas;

    [Header("UI - Menú Habilidades (Activas)")]
    public Button[] botonesHabilidades; // ¡AQUÍ ARRASTRARÁS TUS BOTONES Habilidad1, 2 y 3!

    [Header("UI - Feedback Visual (Buffs)")]
    public GameObject iconoBuffFuerza;
    public GameObject iconoBuffDefensa;

    [Header("Inventarios de Combate (Deja huecos llenos para testear)")]
    public ObjetoTienda[] mochilaPociones = new ObjetoTienda[5];
    public Habilidad[] mochilaPasivas = new Habilidad[5];
    public Habilidad[] mochilaHabilidades = new Habilidad[3];

    [Header("UI - Pantalla Fin de Combate")]
    public GameObject panelFinCombate;
    public GameObject objetoCartelVictoria;
    public GameObject objetoCartelDerrota;
    public TMP_Text textoResumenTurnos;
    public TMP_Text textoResumenOro;

    private int numeroTurno = 0;

    [Header("Estados Alterados (Buffs)")]
    public bool buffFuerzaActivo = false;
    public bool buffDefensaActivo = false;

    [HideInInspector] public UnidadCombate unidadHeroe;
    [HideInInspector] public UnidadCombate unidadEnemigo;

    private Animator animatorHeroe;
    private Animator animatorEnemigo;

    void Awake() { Instance = this; }

    void Start()
    {
        estado = EstadoCombate.START;

        if (panelFinCombate != null) panelFinCombate.SetActive(false);
        if (iconoBuffFuerza != null) iconoBuffFuerza.SetActive(false);
        if (iconoBuffDefensa != null) iconoBuffDefensa.SetActive(false);

        CargarInventarioGlobal();

        ActualizarMenuPociones();
        ActualizarMenuPasivas();
        ActualizarMenuHabilidades(); // Inicializamos los botones de las habilidades

        StartCoroutine(ConfigurarCombate());
    }

    void CargarInventarioGlobal()
    {
        if (GameManager.Instance == null) return;

        // 1. Cargar Pociones
        int indexPociones = 0;
        for (int i = 0; i < mochilaPociones.Length; i++)
        {
            if (mochilaPociones[i] == null && indexPociones < GameManager.Instance.pocionesGlobales.Count)
            {
                mochilaPociones[i] = GameManager.Instance.pocionesGlobales[indexPociones];
                indexPociones++;
            }
        }

        // Filtro de Habilidades
        List<Habilidad> activas = new List<Habilidad>();
        List<Habilidad> pasivas = new List<Habilidad>();

        foreach (Habilidad hab in GameManager.Instance.habilidadesGlobales)
        {
            if (hab.esPasiva) pasivas.Add(hab);
            else activas.Add(hab);
        }

        // 2. Cargar Pasivas
        int indexPasivas = 0;
        for (int i = 0; i < mochilaPasivas.Length; i++)
        {
            if (mochilaPasivas[i] == null && indexPasivas < pasivas.Count)
            {
                mochilaPasivas[i] = pasivas[indexPasivas];
                indexPasivas++;
            }
        }

        // 3. Cargar Habilidades Activas
        int indexActivas = 0;
        for (int i = 0; i < mochilaHabilidades.Length; i++)
        {
            if (mochilaHabilidades[i] == null && indexActivas < activas.Count)
            {
                mochilaHabilidades[i] = activas[indexActivas];
                indexActivas++;
            }
        }
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

        ComprobarPasivasDeInicio();

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

    public void ActualizarMenuPasivas()
    {
        for (int i = 0; i < botonesPasivas.Length; i++)
        {
            if (i < mochilaPasivas.Length && mochilaPasivas[i] != null && mochilaPasivas[i].esPasiva)
            {
                botonesPasivas[i].gameObject.SetActive(true);
                botonesPasivas[i].GetComponent<Image>().sprite = mochilaPasivas[i].iconoHabilidad;
            }
            else
            {
                botonesPasivas[i].gameObject.SetActive(false);
            }
        }
    }

    // --- NUEVA LÓGICA: MENÚ HABILIDADES ACTIVAS ---
    public void ActualizarMenuHabilidades()
    {
        for (int i = 0; i < botonesHabilidades.Length; i++)
        {
            if (i < mochilaHabilidades.Length && mochilaHabilidades[i] != null && !mochilaHabilidades[i].esPasiva)
            {
                botonesHabilidades[i].gameObject.SetActive(true);
                botonesHabilidades[i].GetComponent<Image>().sprite = mochilaHabilidades[i].iconoHabilidad;

                // Conectamos el botón para que ejecute esta habilidad en concreto
                int indiceHueco = i;
                botonesHabilidades[i].onClick.RemoveAllListeners();
                botonesHabilidades[i].onClick.AddListener(() => UsarHabilidadDesdeHueco(indiceHueco));
            }
            else
            {
                botonesHabilidades[i].gameObject.SetActive(false);
            }
        }
    }

    public void UsarHabilidadDesdeHueco(int indice)
    {
        if (estado != EstadoCombate.TURNO_JUGADOR) return;

        Habilidad habElegida = mochilaHabilidades[indice];

        if (habElegida != null)
        {
            if (unidadHeroe.manaActual >= habElegida.costeMana)
            {
                unidadHeroe.GastarMana(habElegida.costeMana);
                StartCoroutine(SecuenciaHabilidadMochila(habElegida));
            }
            else
            {
                Debug.Log("¡No tienes suficiente maná para " + habElegida.nombreHabilidad + "!");
            }
        }
    }

    IEnumerator SecuenciaHabilidadMochila(Habilidad hab)
    {
        estado = EstadoCombate.START;

        // Cierra el menú visualmente
        MenuAccionesManager menuManager = FindFirstObjectByType<MenuAccionesManager>();
        if (menuManager != null) menuManager.VolverAlInicio();

        ActualizarUI();
        if (animatorHeroe != null) animatorHeroe.SetTrigger("Atacar");

        yield return new WaitForSeconds(1f);

        // Calculamos el daño usando el multiplicador de tu ScriptableObject
        int dañoEspecial = unidadHeroe.dañoBase * hab.multiplicadorDaño;

        if (buffFuerzaActivo)
        {
            dañoEspecial *= 2;
            buffFuerzaActivo = false;
            if (iconoBuffFuerza != null) iconoBuffFuerza.SetActive(false);
        }

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
    // ----------------------------------------------

    void ComprobarPasivasDeInicio()
    {
        foreach (Habilidad pasiva in mochilaPasivas)
        {
            if (pasiva != null && pasiva.esPasiva)
            {
                if (pasiva.tipoPasiva == TipoPasiva.MenteClara)
                {
                    unidadHeroe.manaActual = unidadHeroe.manaMaximo;
                    Debug.Log("¡Pasiva Mente Clara activada!");
                }
            }
        }
    }

    void ComprobarPasivasDeTurno()
    {
        bool requiereActualizarUI = false;

        foreach (Habilidad pasiva in mochilaPasivas)
        {
            if (pasiva != null && pasiva.esPasiva)
            {
                if (pasiva.tipoPasiva == TipoPasiva.RegeneracionVidaTurno)
                {
                    unidadHeroe.vidaActual += pasiva.valorPasiva;
                    if (unidadHeroe.vidaActual > unidadHeroe.vidaMaxima) unidadHeroe.vidaActual = unidadHeroe.vidaMaxima;
                    requiereActualizarUI = true;
                    Debug.Log("¡Pasiva Regeneración de Vida activada!");
                }
                else if (pasiva.tipoPasiva == TipoPasiva.RegeneracionManaTurno)
                {
                    unidadHeroe.manaActual += pasiva.valorPasiva;
                    if (unidadHeroe.manaActual > unidadHeroe.manaMaximo) unidadHeroe.manaActual = unidadHeroe.manaMaximo;
                    requiereActualizarUI = true;
                    Debug.Log("¡Pasiva Regeneración de Maná activada!");
                }
            }
        }

        if (requiereActualizarUI) ActualizarUI();
    }

    void EmpezarTurnoJugador()
    {
        numeroTurno++;
        estado = EstadoCombate.TURNO_JUGADOR;

        ComprobarPasivasDeTurno();

        unidadHeroe.RegenerarManaTurno();
        ActualizarUI();
    }

    public void BotonAtacar() { if (estado == EstadoCombate.TURNO_JUGADOR) StartCoroutine(AtacarEnemigo()); }

    // Ataque Fuerte Estándar (El que ya tenías)
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

    public void ActualizarMenuPociones()
    {
        for (int i = 0; i < botonesPociones.Length; i++)
        {
            if (i < mochilaPociones.Length && mochilaPociones[i] != null && mochilaPociones[i].cantidadEnInventario > 0)
            {
                botonesPociones[i].gameObject.SetActive(true);
                botonesPociones[i].GetComponent<Image>().sprite = mochilaPociones[i].iconoObjeto;
                textosCantidadPociones[i].text = "x" + mochilaPociones[i].cantidadEnInventario;
            }
            else
            {
                botonesPociones[i].gameObject.SetActive(false);
            }
        }
    }

    public void UsarPocionDesdeHueco(int indiceHueco)
    {
        if (estado != EstadoCombate.TURNO_JUGADOR) return;

        ObjetoTienda pocion = mochilaPociones[indiceHueco];

        if (pocion != null && pocion.cantidadEnInventario > 0)
        {
            pocion.cantidadEnInventario--;
            StartCoroutine(SecuenciaUsarPocion(pocion));
        }
    }

    IEnumerator SecuenciaUsarPocion(ObjetoTienda pocion)
    {
        estado = EstadoCombate.START;

        MenuAccionesManager menuManager = FindFirstObjectByType<MenuAccionesManager>();
        if (menuManager != null) menuManager.VolverAlInicio();

        AplicarEfectoPocion(pocion);

        ActualizarUI();
        ActualizarMenuPociones();

        yield return new WaitForSeconds(1f);

        estado = EstadoCombate.TURNO_ENEMIGO;
        StartCoroutine(TurnoEnemigo());
    }

    void AplicarEfectoPocion(ObjetoTienda pocion)
    {
        switch (pocion.tipoEfecto)
        {
            case TipoEfectoTienda.Curacion:
                unidadHeroe.vidaActual += pocion.valorEfecto;
                if (unidadHeroe.vidaActual > unidadHeroe.vidaMaxima) unidadHeroe.vidaActual = unidadHeroe.vidaMaxima;
                break;

            case TipoEfectoTienda.Mana:
                unidadHeroe.manaActual += pocion.valorEfecto;
                if (unidadHeroe.manaActual > unidadHeroe.manaMaximo) unidadHeroe.manaActual = unidadHeroe.manaMaximo;
                break;

            case TipoEfectoTienda.FuerzaExtra:
                buffFuerzaActivo = true;
                if (iconoBuffFuerza != null) iconoBuffFuerza.SetActive(true);
                break;

            case TipoEfectoTienda.DefensaExtra:
                buffDefensaActivo = true;
                if (iconoBuffDefensa != null) iconoBuffDefensa.SetActive(true);
                break;

            default:
                break;
        }
    }

    IEnumerator UsarHabilidadEspecial()
    {
        estado = EstadoCombate.START;
        ActualizarUI();

        if (animatorHeroe != null) animatorHeroe.SetTrigger("Atacar");

        yield return new WaitForSeconds(1f);

        int dañoEspecial = unidadHeroe.dañoBase * 2;

        if (buffFuerzaActivo)
        {
            dañoEspecial *= 2;
            buffFuerzaActivo = false;
            if (iconoBuffFuerza != null) iconoBuffFuerza.SetActive(false);
        }

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

        int dañoFinal = unidadHeroe.dañoBase;

        if (buffFuerzaActivo)
        {
            dañoFinal *= 2;
            buffFuerzaActivo = false;
            if (iconoBuffFuerza != null) iconoBuffFuerza.SetActive(false);
        }

        bool enemigoMuerto = unidadEnemigo.RecibirDaño(dañoFinal);
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

        int dañoRecibido = unidadEnemigo.dañoBase;

        if (buffDefensaActivo)
        {
            dañoRecibido /= 2;
            buffDefensaActivo = false;
            if (iconoBuffDefensa != null) iconoBuffDefensa.SetActive(false);
        }

        bool heroeMuerto = unidadHeroe.RecibirDaño(dañoRecibido);
        ActualizarUI();

        if (heroeMuerto) FinalizarCombate(false);
        else EmpezarTurnoJugador();
    }

    void FinalizarCombate(bool victoria)
    {
        StopAllCoroutines();
        estado = EstadoCombate.START;

        if (iconoBuffFuerza != null) iconoBuffFuerza.SetActive(false);
        if (iconoBuffDefensa != null) iconoBuffDefensa.SetActive(false);

        if (panelFinCombate != null) panelFinCombate.SetActive(true);

        if (victoria)
        {
            estado = EstadoCombate.VICTORIA;
            if (objetoCartelVictoria != null) objetoCartelVictoria.SetActive(true);
            if (objetoCartelDerrota != null) objetoCartelDerrota.SetActive(false);

            if (textoResumenTurnos != null) textoResumenTurnos.text = numeroTurno.ToString("00");

            int oroGanado = Mathf.Max(100, 500 - (numeroTurno * 20));
            if (textoResumenOro != null) textoResumenOro.text = oroGanado.ToString("000");

            // --- NUEVO: CÁLCULO DE EXPERIENCIA ---
            // Damos 50 de XP base, más un bonus si lo matas rápido (en pocos turnos)
            int expGanada = Mathf.Max(20, 100 - (numeroTurno * 5));

            if (GameManager.Instance != null)
            {
                GameManager.Instance.oroTotal += oroGanado;
                GameManager.Instance.GanarExperiencia(expGanada); // Mandamos la XP al cerebro
                Debug.Log("Oro y XP guardados en el GameManager.");
            }
        }
        else
        {
            estado = EstadoCombate.DERROTA;
            if (objetoCartelVictoria != null) objetoCartelVictoria.SetActive(false);
            if (objetoCartelDerrota != null) objetoCartelDerrota.SetActive(true);

            if (textoResumenTurnos != null) textoResumenTurnos.text = numeroTurno.ToString("00");
            if (textoResumenOro != null) textoResumenOro.text = "000";

            // Opcional: Podrías darle un poquito de XP al jugador (ej: 10 XP) aunque pierda, 
            // como premio de consolación. Lo dejo a tu elección como diseñador.
        }
    }

    public void BotonHuir()
    {
        if (estado != EstadoCombate.TURNO_JUGADOR) return;

        Debug.Log("El jugador se ha rendido. Fin del combate.");
        FinalizarCombate(false);
    }

    public void BotonSalirFinCombate()
    {
        if (estado == EstadoCombate.VICTORIA)
        {
            Debug.Log("¡Victoria! Cargando el mapa...");
            SceneManager.LoadScene(nombreEscenaMapa);
        }
        else if (estado == EstadoCombate.DERROTA)
        {
            Debug.Log("¡Derrota! Volviendo al menú principal...");
            SceneManager.LoadScene(nombreEscenaMenu);
        }
    }
}