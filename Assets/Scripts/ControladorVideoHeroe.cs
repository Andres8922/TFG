using UnityEngine;
using UnityEngine.Video; 
using UnityEngine.SceneManagement; 

public class ControladorVideoHeroe : MonoBehaviour
{
    [Header("UI Elementos")]
    public GameObject panelVideoUI; 
    public GameObject botonJugarUI; 

    [Header("Reproductor")]
    public VideoPlayer reproductorVideo;

    [Header("Clips de Vídeo de los Héroes")]
    public VideoClip clipCaballero;
    public VideoClip clipMago;

    // --- ¡NUEVO: CONTROL DE AUDIO! ---
    [Header("Audio")]
    [Tooltip("Arrastra aquí el objeto que reproduce la música de fondo (AudioSource)")]
    public AudioSource musicaDeFondo;

    private VideoClip clipSeleccionadoParaJugar;

    void Start()
    {
        if (panelVideoUI != null) panelVideoUI.SetActive(false);
        if (botonJugarUI != null) botonJugarUI.SetActive(false);

        if (reproductorVideo != null)
        {
            reproductorVideo.loopPointReached += VideoFinalizado;
        }
    }
    
    private void PrepararSeleccion(VideoClip clip)
    {
        if (clip != null && botonJugarUI != null)
        {
            clipSeleccionadoParaJugar = clip;
            botonJugarUI.SetActive(true);
            Debug.Log("Héroe seleccionado. Clip preparado. Pulsa 'Jugar' para empezar.");
        }
    }

    public void SeleccionarCaballero() { PrepararSeleccion(clipCaballero); }
    public void SeleccionarMago() { PrepararSeleccion(clipMago); }

    public void BotonJugarPresionado()
    {
        if (reproductorVideo != null && clipSeleccionadoParaJugar != null && panelVideoUI != null && botonJugarUI != null)
        {
            // 1. Apagamos el botón y encendemos la pantalla
            botonJugarUI.SetActive(false);
            panelVideoUI.SetActive(true);

            // --- ¡NUEVA MAGIA DE AUDIO! ---
            // Buscamos TODOS los altavoces que existan ahora mismo en el juego (incluido el del menú)
            AudioSource[] todosLosAudios = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
            
            // Los pausamos todos para que haya silencio total
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