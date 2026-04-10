using UnityEngine;
using UnityEngine.UI; // Necesario para la Imagen
using TMPro; // Necesario para los textos
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

    [Header("UI - Visualización (Derecha)")]
    public Image imagenVisualizacion;
    public TMP_Text textoNombre;
    public TMP_Text textoVida;
    public TMP_Text textoMana;
    public TMP_Text textoFuerza;

    [Header("Reproductor")]
    public VideoPlayer reproductorVideo;

    [Header("Clips de Vídeo de los Héroes")]
    public VideoClip clipCaballero;
    public VideoClip clipMago;

    [Header("Audio")]
    [Tooltip("Arrastra aquí el objeto que reproduce la música de fondo (AudioSource)")]
    public AudioSource musicaDeFondo;

    private VideoClip clipSeleccionadoParaJugar;

    void Start()
    {
        if (panelVideoUI != null) panelVideoUI.SetActive(false);
        if (botonJugarUI != null) botonJugarUI.SetActive(false);

        // Vaciamos la previsualización al empezar
        if (imagenVisualizacion != null) imagenVisualizacion.gameObject.SetActive(false);
        if (textoNombre != null) textoNombre.text = "Selecciona un héroe";
        if (textoVida != null) textoVida.text = "";
        if (textoMana != null) textoMana.text = "";
        if (textoFuerza != null) textoFuerza.text = "";

        if (reproductorVideo != null)
        {
            reproductorVideo.loopPointReached += VideoFinalizado;
        }
    }

    // Función centralizada que prepara el vídeo, guarda el héroe y actualiza la UI
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

            // 2. Extraemos los datos del prefab del héroe elegido
            if (listaHeroesPrefab != null && indiceHeroe < listaHeroesPrefab.Length)
            {
                GameObject heroePrefab = listaHeroesPrefab[indiceHeroe];
                UnidadCombate stats = heroePrefab.GetComponent<UnidadCombate>();
                SpriteRenderer spriteHeroe = heroePrefab.GetComponentInChildren<SpriteRenderer>();

                // 3. Pintamos la información en el panel derecho
                if (imagenVisualizacion != null && spriteHeroe != null)
                {
                    imagenVisualizacion.gameObject.SetActive(true);
                    imagenVisualizacion.sprite = spriteHeroe.sprite;
                    imagenVisualizacion.preserveAspect = true;
                }

                if (textoNombre != null) textoNombre.text = heroePrefab.name.ToUpper();
                if (textoVida != null && stats != null) textoVida.text = "Vida: " + stats.vidaMaxima;
                if (textoMana != null && stats != null) textoMana.text = "Maná: " + stats.manaMaximo;
                if (textoFuerza != null && stats != null) textoFuerza.text = "Fuerza: " + stats.dañoBase;
            }

            Debug.Log("Héroe seleccionado. Clip y UI preparados. Pulsa 'Jugar' para empezar.");
        }
    }

    // Funciones asignadas a los botones de retrato
    public void SeleccionarCaballero() { PrepararSeleccion(clipCaballero, 0); }
    public void SeleccionarMago() { PrepararSeleccion(clipMago, 1); }

    public void BotonJugarPresionado()
    {
        if (reproductorVideo != null && clipSeleccionadoParaJugar != null && panelVideoUI != null && botonJugarUI != null)
        {
            // 1. Apagamos el botón y encendemos la pantalla de vídeo
            botonJugarUI.SetActive(false);
            panelVideoUI.SetActive(true);

            // Buscamos TODOS los altavoces que existan y los pausamos
            AudioSource[] todosLosAudios = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
            foreach (AudioSource audio in todosLosAudios)
            {
                audio.Pause();
            }

            // 2. Cargamos el clip y le damos al Play
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
}