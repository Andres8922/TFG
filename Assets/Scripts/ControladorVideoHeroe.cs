using UnityEngine;
using UnityEngine.UI;
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

    [Tooltip("Arrastra aquí las imágenes del corazón, la gota y el puño")]
    public GameObject[] iconosEstadisticas;

    [Header("Reproductor")]
    public VideoPlayer reproductorVideo;

    [Header("Clips de Vídeo de los Héroes")]
    public VideoClip clipCaballero;
    public VideoClip clipMago;
    public VideoClip clipElfaArquera;
    public VideoClip clipEnano;

    [Header("Botón Enano (Bloqueado / Desbloqueado)")]
    public Image imagenBotonEnano;
    public Sprite spriteEnanoDesbloqueado;
    public Sprite spriteEnanoBloqueado;

    [Header("Audio")]
    [Tooltip("Arrastra aquí el objeto que reproduce la música de fondo (AudioSource)")]
    public AudioSource musicaDeFondo;

    private VideoClip clipSeleccionadoParaJugar;

    private GameObject heroeInstanciadoActual;

    void Start()
    {
        if (panelVideoUI != null) panelVideoUI.SetActive(false);
        if (botonJugarUI != null) botonJugarUI.SetActive(false);

        if (textoVida != null) textoVida.text = "";
        if (textoMana != null) textoMana.text = "";
        if (textoFuerza != null) textoFuerza.text = "";
        ActivarIconos(false);

        if (reproductorVideo != null)
        {
            reproductorVideo.loopPointReached += VideoFinalizado;
        }

        ActualizarBotonEnano();
    }

    void ActualizarBotonEnano()
    {
        if (imagenBotonEnano == null || GameManager.Instance == null) return;

        bool desbloqueado = GameManager.Instance.heroesDesbloqueados.Length > 3
                            && GameManager.Instance.heroesDesbloqueados[3];

        imagenBotonEnano.sprite = desbloqueado ? spriteEnanoDesbloqueado : spriteEnanoBloqueado;

        Button boton = imagenBotonEnano.GetComponent<Button>();
        if (boton != null)
        {
            ColorBlock colores = boton.colors;
            colores.disabledColor = Color.white;
            boton.colors = colores;
            boton.interactable = desbloqueado;
        }
    }

    private void PrepararSeleccion(VideoClip clip, int indiceHeroe)
    {
        if (clip != null && botonJugarUI != null)
        {
            clipSeleccionadoParaJugar = clip;
            botonJugarUI.SetActive(true);

            if (GameManager.Instance != null)
                GameManager.Instance.heroeSeleccionado = indiceHeroe;

            if (heroeInstanciadoActual != null)
                Destroy(heroeInstanciadoActual);

            if (listaHeroesPrefab != null && indiceHeroe < listaHeroesPrefab.Length && puntoVisualizacion != null)
            {
                heroeInstanciadoActual = Instantiate(listaHeroesPrefab[indiceHeroe], puntoVisualizacion.position, Quaternion.identity);

                UnidadCombate stats = heroeInstanciadoActual.GetComponent<UnidadCombate>();
                if (stats != null)
                {
                    if (textoVida != null) textoVida.text = "" + stats.vidaMaxima;
                    if (textoMana != null) textoMana.text = "" + stats.manaMaximo;
                    if (textoFuerza != null) textoFuerza.text = "" + stats.dañoBase;
                    ActivarIconos(true);
                }
            }
        }
    }

    public void SeleccionarCaballero()   { PrepararSeleccion(clipCaballero,    0); }
    public void SeleccionarMago()        { PrepararSeleccion(clipMago,         1); }
    public void SeleccionarElfaArquera() { PrepararSeleccion(clipElfaArquera,  2); }
    public void SeleccionarEnano()       { PrepararSeleccion(clipEnano,        3); }

    public void BotonJugarPresionado()
    {
        if (reproductorVideo != null && clipSeleccionadoParaJugar != null && panelVideoUI != null && botonJugarUI != null)
        {
            botonJugarUI.SetActive(false);
            panelVideoUI.SetActive(true);

            if (heroeInstanciadoActual != null)
                Destroy(heroeInstanciadoActual);

            ActivarIconos(false);

            // Pausar todo el audio para que no solape con la cinemática
            AudioSource[] todosLosAudios = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
            foreach (AudioSource audio in todosLosAudios)
                audio.Pause();

            reproductorVideo.clip = clipSeleccionadoParaJugar;
            reproductorVideo.time = 0;
            reproductorVideo.Play();
        }
    }

    void VideoFinalizado(VideoPlayer vp)
    {
        if (Transicion.Instance != null)
            Transicion.Instance.CargarEscena("Mapa");
        else
            SceneManager.LoadScene("Mapa");
    }

    private void ActivarIconos(bool estado)
    {
        if (iconosEstadisticas != null)
        {
            foreach (GameObject icono in iconosEstadisticas)
                if (icono != null) icono.SetActive(estado);
        }
    }
}
