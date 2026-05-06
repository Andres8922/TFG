using UnityEngine;
using TMPro;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class ControladorVideoHeroe : MonoBehaviour
{
    [Header("Base de Datos de Héroes")]
    [Tooltip("Arrastra aquí los prefabs: 0=Caballero, 1=Mago...")]
    public GameObject[] listaHeroesPrefab;

    [Header("UI Elementos - Cinemática")]
    public GameObject panelVideoUI;
    public GameObject botonJugarUI;

    [Header("Visualización Animada (Derecha)")]
    [Tooltip("El punto vacío en la escena donde aparecerá el héroe animado")]
    public Transform puntoVisualizacion;
    public TMP_Text textoVida;
    public TMP_Text textoMana;
    public TMP_Text textoFuerza;

    // --- NUEVO: ARRAY PARA LOS ICONOS ---
    [Tooltip("Arrastra aquí las imágenes del corazón, la gota y el puño")]
    public GameObject[] iconosEstadisticas;

    [Header("Reproductor")]
    public VideoPlayer reproductorVideo;

    [Header("Clips de Vídeo de los Héroes")]
    public VideoClip clipCaballero;
    public VideoClip clipMago;
    public VideoClip clipElfaArquera;
    public VideoClip clipEnano;

    [Header("Audio")]
    [Tooltip("Arrastra aquí el objeto que reproduce la música de fondo (AudioSource)")]
    public AudioSource musicaDeFondo;

    private VideoClip clipSeleccionadoParaJugar;

    // Variable para recordar qué héroe 2D hemos creado y borrarlo si seleccionas otro
    private GameObject heroeInstanciadoActual;

    void Start()
    {
        if (panelVideoUI != null) panelVideoUI.SetActive(false);
        if (botonJugarUI != null) botonJugarUI.SetActive(false);

        // Vaciamos los textos al empezar
        if (textoVida != null) textoVida.text = "";
        if (textoMana != null) textoMana.text = "";
        if (textoFuerza != null) textoFuerza.text = "";

        // --- NUEVO: APAGAMOS LOS ICONOS AL EMPEZAR ---
        ActivarIconos(false);

        if (reproductorVideo != null)
        {
            reproductorVideo.loopPointReached += VideoFinalizado;
        }
    }

    // Función centralizada que prepara el vídeo, guarda el héroe e invoca el modelo 3D/2D
    private void PrepararSeleccion(VideoClip clip, int indiceHeroe)
    {
        if (clip != null && botonJugarUI != null)
        {
            clipSeleccionadoParaJugar = clip;
            botonJugarUI.SetActive(true);

            // 1. Guardamos la elección en el GameManager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.heroeSeleccionado = indiceHeroe;
            }

            // 2. Si ya había un héroe en pantalla mirándonos, lo destruimos
            if (heroeInstanciadoActual != null)
            {
                Destroy(heroeInstanciadoActual);
            }

            // 3. Invocamos al nuevo héroe "vivo" en el punto exacto y sacamos sus stats
            if (listaHeroesPrefab != null && indiceHeroe < listaHeroesPrefab.Length && puntoVisualizacion != null)
            {
                GameObject prefabElegido = listaHeroesPrefab[indiceHeroe];

                // Instanciamos el clon animado
                heroeInstanciadoActual = Instantiate(prefabElegido, puntoVisualizacion.position, Quaternion.identity);

                // Leemos las estadísticas del componente UnidadCombate de ese clon
                UnidadCombate stats = heroeInstanciadoActual.GetComponent<UnidadCombate>();

                if (stats != null)
                {
                    if (textoVida != null) textoVida.text = "" + stats.vidaMaxima;
                    if (textoMana != null) textoMana.text = "" + stats.manaMaximo;
                    if (textoFuerza != null) textoFuerza.text = "" + stats.dañoBase;

                    // --- NUEVO: ENCENDEMOS LOS ICONOS AL SELECCIONAR HÉROE ---
                    ActivarIconos(true);
                }
            }

            Debug.Log("Héroe animado invocado. Clip preparado. Pulsa 'Jugar' para empezar.");
        }
    }

    // Funciones asignadas a los botones de retrato
    public void SeleccionarCaballero()   { PrepararSeleccion(clipCaballero,    0); }
    public void SeleccionarMago()        { PrepararSeleccion(clipMago,         1); }
    public void SeleccionarElfaArquera() { PrepararSeleccion(clipElfaArquera,  2); }
    public void SeleccionarEnano()       { PrepararSeleccion(clipEnano,        3); }

    public void BotonJugarPresionado()
    {
        if (reproductorVideo != null && clipSeleccionadoParaJugar != null && panelVideoUI != null && botonJugarUI != null)
        {
            // Apagamos el botón y encendemos la pantalla de vídeo
            botonJugarUI.SetActive(false);
            panelVideoUI.SetActive(true);

            // IMPORTANTE: Borramos al héroe de la pantalla para que no se quede ahí flotando durante el vídeo
            if (heroeInstanciadoActual != null)
            {
                Destroy(heroeInstanciadoActual);
            }

            // --- NUEVO: APAGAMOS LOS ICONOS PARA QUE NO PISEN EL VÍDEO ---
            ActivarIconos(false);

            // Buscamos TODOS los altavoces que existan y los pausamos
            AudioSource[] todosLosAudios = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
            foreach (AudioSource audio in todosLosAudios)
            {
                audio.Pause();
            }

            // Cargamos el clip y le damos al Play
            reproductorVideo.clip = clipSeleccionadoParaJugar;
            reproductorVideo.time = 0;
            reproductorVideo.Play();

            Debug.Log("Empezando cinemática y silenciando toda la música de fondo...");
        }
    }

    void VideoFinalizado(VideoPlayer vp)
    {
        Debug.Log("Cinemática terminada. Cargando el mapa...");

        if (Transicion.Instance != null)
        {
            Transicion.Instance.CargarEscena("Mapa");
        }
        else
        {
            SceneManager.LoadScene("Mapa");
        }
    }

    // --- NUEVA FUNCIÓN AUXILIAR PARA CONTROLAR LOS ICONOS ---
    private void ActivarIconos(bool estado)
    {
        if (iconosEstadisticas != null)
        {
            foreach (GameObject icono in iconosEstadisticas)
            {
                if (icono != null)
                {
                    icono.SetActive(estado);
                }
            }
        }
    }
}