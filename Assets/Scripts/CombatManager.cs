using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public enum EstadoCombate { START, TURNO_JUGADOR, TURNO_ENEMIGO, VICTORIA, DERROTA }

[System.Serializable]
public class ConfiguracionHeroe
{
    public List<GameObject> enemigosPrefabs;
    public GameObject jefePrefab;
    public List<Sprite> fondosCombate;
    public Sprite fondoJefe;
}

[System.Serializable]
public class ImagenesFinCombate
{
    public Sprite imagenVictoria;
    public Sprite imagenDerrota;
}

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance;
    public EstadoCombate estado;

    [Header("Configuración")]
    public GameObject[] listaHeroes;
    public Transform puntoHeroe;
    public Transform puntoEnemigo;

    [Header("Configuración por Héroe")]
    public ConfiguracionHeroe[] configuracionesPorHeroe;
    public Image fondoCombate;

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
    public Button[] botonesHabilidades;

    [Header("UI - Feedback Visual (Buffs)")]
    public GameObject iconoBuffFuerza;
    public GameObject iconoBuffDefensa;

    [Header("Inventarios de Combate")]
    public ObjetoTienda[] mochilaPociones = new ObjetoTienda[5];
    public Habilidad[] mochilaPasivas = new Habilidad[5];
    public Habilidad[] mochilaHabilidades = new Habilidad[3];

    [Header("Texto Flotante de Daño")]
    public GameObject prefabTextoFlotante;

    [Header("UI - Pantalla Fin de Combate")]
    public GameObject panelFinCombate;
    public GameObject objetoCartelVictoria;
    public GameObject objetoCartelDerrota;
    public TMP_Text textoResumenTurnos;
    public TMP_Text textoResumenOro;

    [Header("UI - Imagen Final por Héroe")]
    public ImagenesFinCombate[] imagenesPorHeroe = new ImagenesFinCombate[4];
    public GameObject panelImagenFinal;
    public Image imagenFinalCompleta;

    private int numeroTurno = 0;

    [Header("Estados Alterados (Buffs)")]
    public bool buffFuerzaActivo = false;
    public bool buffDefensaActivo = false;
    private int valorBuffFuerza = 2;
    private int valorBuffDefensa = 2;

    [HideInInspector] public UnidadCombate unidadHeroe;
    [HideInInspector] public UnidadCombate unidadEnemigo;

    private Animator animatorHeroe;
    private Animator animatorEnemigo;

    void Awake() { Instance = this; }

    void Start()
    {
        estado = EstadoCombate.START;

        if (panelFinCombate != null) panelFinCombate.SetActive(false);
        if (panelImagenFinal != null) panelImagenFinal.SetActive(false);
        if (iconoBuffFuerza != null) iconoBuffFuerza.SetActive(false);
        if (iconoBuffDefensa != null) iconoBuffDefensa.SetActive(false);

        CargarInventarioGlobal();

        ActualizarMenuPociones();
        ActualizarMenuPasivas();
        ActualizarMenuHabilidades();

        StartCoroutine(ConfigurarCombate());
    }

    void CargarInventarioGlobal()
    {
        if (GameManager.Instance == null) return;

        int indexPociones = 0;
        for (int i = 0; i < mochilaPociones.Length; i++)
        {
            if (mochilaPociones[i] == null && indexPociones < GameManager.Instance.pocionesGlobales.Count)
            {
                mochilaPociones[i] = GameManager.Instance.pocionesGlobales[indexPociones];
                indexPociones++;
            }
        }

        List<Habilidad> activas = new List<Habilidad>();
        List<Habilidad> pasivas = new List<Habilidad>();

        foreach (Habilidad hab in GameManager.Instance.habilidadesGlobales)
        {
            if (hab.esPasiva) pasivas.Add(hab);
            else activas.Add(hab);
        }

        int indexPasivas = 0;
        for (int i = 0; i < mochilaPasivas.Length; i++)
        {
            if (mochilaPasivas[i] == null && indexPasivas < pasivas.Count)
            {
                mochilaPasivas[i] = pasivas[indexPasivas];
                indexPasivas++;
            }
        }

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

        if (indice < 0 || indice >= listaHeroes.Length || listaHeroes[indice] == null)
        {
            Debug.LogError($"CombatManager: no hay prefab asignado en listaHeroes[{indice}]. Asigna los 4 héroes en el Inspector.");
            yield break;
        }

        GameObject heroeGO = Instantiate(listaHeroes[indice], puntoHeroe.position, Quaternion.identity);
        unidadHeroe = heroeGO.GetComponent<UnidadCombate>();
        animatorHeroe = heroeGO.GetComponent<Animator>();

        if (configuracionesPorHeroe == null || indice >= configuracionesPorHeroe.Length || configuracionesPorHeroe[indice] == null)
        {
            Debug.LogError($"CombatManager: no hay configuración para el héroe [{indice}]. Asígnalos en el Inspector.");
            yield break;
        }

        ConfiguracionHeroe config = configuracionesPorHeroe[indice];
        GameObject prefabEnemigo;

        if (DatosGlobales.tipoNodoActual == TipoNodo.Jefe)
        {
            if (config.jefePrefab == null)
            {
                Debug.LogError($"CombatManager: no hay jefePrefab para el héroe [{indice}]. Asígnalo en el Inspector.");
                yield break;
            }
            prefabEnemigo = config.jefePrefab;
        }
        else
        {
            if (config.enemigosPrefabs == null || config.enemigosPrefabs.Count == 0)
            {
                Debug.LogError($"CombatManager: no hay enemigos en la lista para el héroe [{indice}]. Asígnalos en el Inspector.");
                yield break;
            }
            int enemigoIdx = DatosGlobales.combatesRealizados % config.enemigosPrefabs.Count;
            prefabEnemigo = config.enemigosPrefabs[enemigoIdx];
        }

        Vector3 offset = Vector3.zero;
        ConfiguracionEnemigo cfgEnemigo = prefabEnemigo.GetComponent<ConfiguracionEnemigo>();
        if (cfgEnemigo != null) offset = cfgEnemigo.offsetSpawn;

        GameObject enemigoGO = Instantiate(prefabEnemigo, puntoEnemigo.position + offset, Quaternion.identity);
        unidadEnemigo = enemigoGO.GetComponent<UnidadCombate>();
        animatorEnemigo = enemigoGO.GetComponent<Animator>();

        if (fondoCombate != null)
        {
            if (DatosGlobales.tipoNodoActual == TipoNodo.Jefe)
            {
                if (config.fondoJefe != null)
                    fondoCombate.sprite = config.fondoJefe;
            }
            else if (config.fondosCombate != null && config.fondosCombate.Count > 0)
            {
                int fondoIdx = DatosGlobales.combatesRealizados % config.fondosCombate.Count;
                fondoCombate.sprite = config.fondosCombate[fondoIdx];
            }
        }

        if (GameManager.Instance != null)
        {
            unidadHeroe.dañoBase  += GameManager.Instance.bonusDaño;
            unidadHeroe.vidaMaxima += GameManager.Instance.bonusVida;
            unidadHeroe.vidaActual  = unidadHeroe.vidaMaxima;

            float[] multiplicadoresDificultad = { 1f, 1.25f, 1.5f };
            int dif = Mathf.Clamp(GameManager.Instance.dificultad, 0, multiplicadoresDificultad.Length - 1);
            float mult = multiplicadoresDificultad[dif];
            unidadEnemigo.dañoBase    = Mathf.RoundToInt(unidadEnemigo.dañoBase    * mult);
            unidadEnemigo.vidaMaxima  = Mathf.RoundToInt(unidadEnemigo.vidaMaxima  * mult);
            unidadEnemigo.vidaActual  = unidadEnemigo.vidaMaxima;
        }

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

    public void ActualizarMenuHabilidades()
    {
        for (int i = 0; i < botonesHabilidades.Length; i++)
        {
            if (i < mochilaHabilidades.Length && mochilaHabilidades[i] != null && !mochilaHabilidades[i].esPasiva)
            {
                botonesHabilidades[i].gameObject.SetActive(true);
                botonesHabilidades[i].GetComponent<Image>().sprite = mochilaHabilidades[i].iconoHabilidad;

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
                if (GameManager.Instance != null) GameManager.Instance.manaTotalPartida += habElegida.costeMana;
                unidadHeroe.GastarMana(habElegida.costeMana);
                StartCoroutine(SecuenciaHabilidadMochila(habElegida));
            }
        }
    }

    IEnumerator SecuenciaHabilidadMochila(Habilidad hab)
    {
        estado = EstadoCombate.START;

        MenuAccionesManager menuManager = FindFirstObjectByType<MenuAccionesManager>();
        if (menuManager != null) menuManager.VolverAlInicio();

        ActualizarUI();

        string trigger = (!string.IsNullOrEmpty(hab.triggerAnimacion)) ? hab.triggerAnimacion : "AtqFuerte";
        if (animatorHeroe != null) animatorHeroe.SetTrigger(trigger);

        yield return new WaitForSeconds(1f);

        if (hab.esDefensa)
        {
            valorBuffDefensa = hab.multiplicadorDanio;
            buffDefensaActivo = true;
            if (iconoBuffDefensa != null) iconoBuffDefensa.SetActive(true);
            ActualizarUI();
            yield return new WaitForSeconds(0.5f);
            estado = EstadoCombate.TURNO_ENEMIGO;
            StartCoroutine(TurnoEnemigo());
        }
        else if (hab.esBuff)
        {
            valorBuffFuerza = hab.multiplicadorDanio;
            buffFuerzaActivo = true;
            if (iconoBuffFuerza != null) iconoBuffFuerza.SetActive(true);
            ActualizarUI();
            yield return new WaitForSeconds(0.5f);
            estado = EstadoCombate.TURNO_ENEMIGO;
            StartCoroutine(TurnoEnemigo());
        }
        else if (hab.esCuracion)
        {
            int vidaRecuperada = unidadHeroe.dañoBase * hab.multiplicadorDanio;
            unidadHeroe.vidaActual = Mathf.Min(unidadHeroe.vidaActual + vidaRecuperada, unidadHeroe.vidaMaxima);
            MostrarDaño(vidaRecuperada, unidadHeroe.transform.position, Color.green);
            ActualizarUI();
            yield return new WaitForSeconds(0.5f);
            estado = EstadoCombate.TURNO_ENEMIGO;
            StartCoroutine(TurnoEnemigo());
        }
        else
        {
            int dañoEspecial = unidadHeroe.dañoBase * hab.multiplicadorDanio;

            if (buffFuerzaActivo)
            {
                dañoEspecial *= valorBuffFuerza;
                buffFuerzaActivo = false;
                valorBuffFuerza = 2;
                if (iconoBuffFuerza != null) iconoBuffFuerza.SetActive(false);
            }

            if (GameManager.Instance != null) GameManager.Instance.dañoTotalPartida += dañoEspecial;
            bool enemigoMuerto = unidadEnemigo.RecibirDaño(dañoEspecial);
            MostrarDaño(dañoEspecial, unidadEnemigo.transform.position, Color.red);
            if (animatorEnemigo != null) animatorEnemigo.SetTrigger(enemigoMuerto ? "Muerte" : "Dañado");
            ActualizarUI();
            ComprobarPasivasDeAtaque();

            yield return new WaitForSeconds(1f);

            if (enemigoMuerto) FinalizarCombate(true);
            else
            {
                estado = EstadoCombate.TURNO_ENEMIGO;
                StartCoroutine(TurnoEnemigo());
            }
        }
    }

    void ComprobarPasivasDeInicio()
    {
        foreach (Habilidad pasiva in mochilaPasivas)
        {
            if (pasiva != null && pasiva.esPasiva)
            {
                if (pasiva.tipoPasiva == TipoPasiva.MenteClara)
                {
                    unidadHeroe.manaActual = unidadHeroe.manaMaximo;
                }
                else if (pasiva.tipoPasiva == TipoPasiva.IncrementoFuerza)
                {
                    unidadHeroe.dañoBase += pasiva.valorPasiva;
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
                }
                else if (pasiva.tipoPasiva == TipoPasiva.RegeneracionManaTurno)
                {
                    unidadHeroe.manaActual += pasiva.valorPasiva;
                    if (unidadHeroe.manaActual > unidadHeroe.manaMaximo) unidadHeroe.manaActual = unidadHeroe.manaMaximo;
                    requiereActualizarUI = true;
                }
            }
        }

        if (requiereActualizarUI) ActualizarUI();
    }

    void ComprobarPasivasDeAtaque()
    {
        foreach (Habilidad pasiva in mochilaPasivas)
        {
            if (pasiva != null && pasiva.tipoPasiva == TipoPasiva.RegeneracionVidaAtaque)
            {
                unidadHeroe.vidaActual = Mathf.Min(unidadHeroe.vidaActual + pasiva.valorPasiva, unidadHeroe.vidaMaxima);
                MostrarDaño(pasiva.valorPasiva, unidadHeroe.transform.position, Color.green);
            }
        }
        ActualizarUI();
    }

    void MostrarDaño(int cantidad, Vector3 posicion, Color color)
    {
        if (prefabTextoFlotante == null) return;
        Vector3 posOffset = posicion + new Vector3(Random.Range(-0.3f, 0.3f), 0.5f, 0f);
        GameObject texto = Instantiate(prefabTextoFlotante, posOffset, Quaternion.identity);
        texto.GetComponent<TextoFlotante>().Inicializar(cantidad, color);
    }

    void EmpezarTurnoJugador()
    {
        numeroTurno++;
        if (GameManager.Instance != null) GameManager.Instance.turnosTotalesPartida++;
        estado = EstadoCombate.TURNO_JUGADOR;

        ComprobarPasivasDeTurno();

        unidadHeroe.RegenerarManaTurno();
        ActualizarUI();
    }

    public void BotonAtacar() { if (estado == EstadoCombate.TURNO_JUGADOR) StartCoroutine(AtacarEnemigo()); }

    public void BotonHabilidad()
    {
        if (estado != EstadoCombate.TURNO_JUGADOR) return;

        int coste = 10;
        if (unidadHeroe.manaActual >= coste)
        {
            if (GameManager.Instance != null) GameManager.Instance.manaTotalPartida += coste;
            unidadHeroe.GastarMana(coste);
            StartCoroutine(UsarHabilidadEspecial());
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
        if (animatorHeroe != null) animatorHeroe.SetTrigger("BeberPocion");
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

        if (animatorHeroe != null) animatorHeroe.SetTrigger("AtqFuerte");

        yield return new WaitForSeconds(1f);

        int dañoEspecial = unidadHeroe.dañoBase * 2;

        if (buffFuerzaActivo)
        {
            dañoEspecial *= valorBuffFuerza;
            buffFuerzaActivo = false;
            valorBuffFuerza = 2;
            if (iconoBuffFuerza != null) iconoBuffFuerza.SetActive(false);
        }

        if (GameManager.Instance != null) GameManager.Instance.dañoTotalPartida += dañoEspecial;
        bool enemigoMuerto = unidadEnemigo.RecibirDaño(dañoEspecial);
        MostrarDaño(dañoEspecial, unidadEnemigo.transform.position, Color.red);
        if (animatorEnemigo != null) animatorEnemigo.SetTrigger(enemigoMuerto ? "Muerte" : "Dañado");
        ActualizarUI();
        ComprobarPasivasDeAtaque();

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

        if (animatorHeroe != null) animatorHeroe.SetTrigger("AtqBasico");

        yield return new WaitForSeconds(0.8f);

        int dañoFinal = unidadHeroe.dañoBase;

        if (buffFuerzaActivo)
        {
            dañoFinal *= valorBuffFuerza;
            buffFuerzaActivo = false;
            valorBuffFuerza = 2;
            if (iconoBuffFuerza != null) iconoBuffFuerza.SetActive(false);
        }

        if (GameManager.Instance != null) GameManager.Instance.dañoTotalPartida += dañoFinal;
        bool enemigoMuerto = unidadEnemigo.RecibirDaño(dañoFinal);
        MostrarDaño(dañoFinal, unidadEnemigo.transform.position, Color.red);
        if (animatorEnemigo != null) animatorEnemigo.SetTrigger(enemigoMuerto ? "Muerte" : "Dañado");
        ActualizarUI();
        ComprobarPasivasDeAtaque();
        yield return new WaitForSeconds(1f);

        if (enemigoMuerto) FinalizarCombate(true);
        else { estado = EstadoCombate.TURNO_ENEMIGO; StartCoroutine(TurnoEnemigo()); }
    }

    IEnumerator TurnoEnemigo()
    {
        yield return new WaitForSeconds(1f);
        string triggerAtaque = (Random.value < 0.5f) ? "AtacarEnemigo1" : "AtacarEnemigo2";
        if (animatorEnemigo != null) animatorEnemigo.SetTrigger(triggerAtaque);
        yield return new WaitForSeconds(0.5f);

        int dañoRecibido = unidadEnemigo.dañoBase;

        foreach (Habilidad pasiva in mochilaPasivas)
        {
            if (pasiva != null && pasiva.tipoPasiva == TipoPasiva.IncrementoDefensa)
                dañoRecibido = Mathf.Max(1, dañoRecibido - pasiva.valorPasiva);
        }

        if (buffDefensaActivo)
        {
            dañoRecibido /= valorBuffDefensa;
            buffDefensaActivo = false;
            valorBuffDefensa = 2;
            if (iconoBuffDefensa != null) iconoBuffDefensa.SetActive(false);
        }

        bool heroeMuerto = unidadHeroe.RecibirDaño(dañoRecibido);
        MostrarDaño(dañoRecibido, unidadHeroe.transform.position, new Color(1f, 0.5f, 0f));
        if (animatorHeroe != null) animatorHeroe.SetTrigger(heroeMuerto ? "Muerte" : "Dañado");
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

        if (victoria)
        {
            estado = EstadoCombate.VICTORIA;
            if (objetoCartelVictoria != null) objetoCartelVictoria.SetActive(true);
            if (objetoCartelDerrota != null) objetoCartelDerrota.SetActive(false);

            if (textoResumenTurnos != null) textoResumenTurnos.text = numeroTurno.ToString("00");

            int oroGanado = Mathf.Max(100, 500 - (numeroTurno * 20));
            int expGanada = Mathf.Max(20, 100 - (numeroTurno * 5));

            foreach (Habilidad pasiva in mochilaPasivas)
            {
                if (pasiva == null) continue;
                if (pasiva.tipoPasiva == TipoPasiva.MultiplicacionOro) oroGanado *= pasiva.valorPasiva;
                if (pasiva.tipoPasiva == TipoPasiva.MultiplicacionExperiencia) expGanada *= pasiva.valorPasiva;
            }

            if (textoResumenOro != null) textoResumenOro.text = oroGanado.ToString("000");

            if (GameManager.Instance != null)
            {
                GameManager.Instance.oroTotal += oroGanado;
                GameManager.Instance.GanarExperiencia(expGanada);
                if (DatosGlobales.tipoNodoActual == TipoNodo.Jefe)
                    GameManager.Instance.victoriaJefe = true;
            }

            if (DatosGlobales.tipoNodoActual != TipoNodo.Jefe)
                DatosGlobales.combatesRealizados++;
        }
        else
        {
            estado = EstadoCombate.DERROTA;
            if (objetoCartelVictoria != null) objetoCartelVictoria.SetActive(false);
            if (objetoCartelDerrota != null) objetoCartelDerrota.SetActive(true);

            if (textoResumenTurnos != null) textoResumenTurnos.text = numeroTurno.ToString("00");
            if (textoResumenOro != null) textoResumenOro.text = "000";
        }

        MostrarImagenFinal(victoria);
    }

    void MostrarImagenFinal(bool victoria)
    {
        if (panelImagenFinal == null || imagenFinalCompleta == null ||
            DatosGlobales.tipoNodoActual != TipoNodo.Jefe)
        {
            if (panelFinCombate != null) panelFinCombate.SetActive(true);
            return;
        }

        int indiceHeroe = (GameManager.Instance != null) ? GameManager.Instance.heroeSeleccionado : 0;

        Sprite sprite = null;
        if (indiceHeroe >= 0 && indiceHeroe < imagenesPorHeroe.Length)
        {
            sprite = victoria
                ? imagenesPorHeroe[indiceHeroe].imagenVictoria
                : imagenesPorHeroe[indiceHeroe].imagenDerrota;
        }

        if (sprite != null && Transicion.Instance != null)
        {
            imagenFinalCompleta.sprite = sprite;
            Transicion.Instance.FadeSoloYAccion(() => panelImagenFinal.SetActive(true));
            return;
        }

        // Sin imagen asignada o sin Transicion: pasar directamente al panel de resumen
        if (panelFinCombate != null) panelFinCombate.SetActive(true);
    }

    public void BotonContinuarImagenFinal()
    {
        if (GameManager.Instance != null) GameManager.Instance.PararCronometro();
        if (Transicion.Instance != null)
            Transicion.Instance.CargarEscena("Estadisticas");
        else
            SceneManager.LoadScene("Estadisticas");
    }

    public void BotonHuir()
    {
        if (estado != EstadoCombate.TURNO_JUGADOR) return;
        FinalizarCombate(false);
    }

    public void BotonSalirFinCombate()
    {
        if (estado == EstadoCombate.VICTORIA)
        {
            if (Transicion.Instance != null)
                Transicion.Instance.CargarEscena(nombreEscenaMapa);
            else
                SceneManager.LoadScene(nombreEscenaMapa);
        }
        else if (estado == EstadoCombate.DERROTA)
        {
            if (GameManager.Instance != null)
                GameManager.Instance.ResetearPartida();
            if (Transicion.Instance != null)
                Transicion.Instance.CargarEscena(nombreEscenaMenu);
            else
                SceneManager.LoadScene(nombreEscenaMenu);
        }
    }
}