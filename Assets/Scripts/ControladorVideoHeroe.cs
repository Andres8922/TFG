using UnityEngine;
using UnityEngine.Video; // Imprescindible
using UnityEngine.SceneManagement; // Imprescindible

public class ControladorVideoHeroe : MonoBehaviour
{
    [Header("UI Elementos")]
    [Tooltip("Arrastra aquí tu objeto Pantalla_Video (la RawImage oscura).")]
    public GameObject panelVideoUI; 
    
    [Tooltip("Arrastra aquí el NUEVO botón de 'JUGAR' que crearemos.")]
    public GameObject botonJugarUI; // Para encenderlo/apagarlo

    [Header("Reproductor")]
    public VideoPlayer reproductorVideo;

    [Header("Clips de Vídeo de los Héroes")]
    public VideoClip clipCaballero;
    public VideoClip clipMago;

    // --- VARIABLE INTERNA CLAVE ---
    // Aquí guardaremos qué clip hemos seleccionado ANTES de darle a Jugar
    private VideoClip clipSeleccionadoParaJugar;

    void Start()
    {
        // 1. Todo apagado al empezar la escena
        if (panelVideoUI != null) panelVideoUI.SetActive(false);
        if (botonJugarUI != null) botonJugarUI.SetActive(false);

        // 2. Nos subscribimos al evento de final de vídeo
        if (reproductorVideo != null)
        {
            reproductorVideo.loopPointReached += VideoFinalizado;
        }
    }

    // --- NUEVAS FUNCIONES DE SELECCIÓN ---
    // Estas funciones NO reproducen el vídeo, solo "guardan" el clip y muestran el botón Jugar
    
    private void PrepararSeleccion(VideoClip clip)
    {
        if (clip != null && botonJugarUI != null)
        {
            // Guardamos el clip en la variable interna
            clipSeleccionadoParaJugar = clip;
            
            // Hacemos aparecer el botón de JUGAR
            botonJugarUI.SetActive(true);
            
            Debug.Log("Héroe seleccionado. Clip preparado. Pulsa 'Jugar' para empezar.");
        }
    }

    // Estas son las que conectarás a tus botones de Caballero/Mago originales
    public void SeleccionarCaballero() { PrepararSeleccion(clipCaballero); }
    public void SeleccionarMago() { PrepararSeleccion(clipMago); }


    // --- LA FUNCIÓN DEL BOTÓN JUGAR ---
    // Esta función la llamará el botón "JUGAR"
    public void BotonJugarPresionado()
    {
        if (reproductorVideo != null && clipSeleccionadoParaJugar != null && panelVideoUI != null && botonJugarUI != null)
        {
            // 1. Apagamos el botón Jugar (ya no hace falta)
            botonJugarUI.SetActive(false);
            
            // 2. Encendemos la pantalla de vídeo
            panelVideoUI.SetActive(true);

            // 3. Cargamos el clip guardado, rebobinamos y ¡PLAY!
            reproductorVideo.clip = clipSeleccionadoParaJugar;
            reproductorVideo.time = 0; 
            reproductorVideo.Play();
            
            Debug.Log("Empezando cinemática...");
        }
    }

    // --- FUNCIÓN FINAL (Automatica) ---
    void VideoFinalizado(VideoPlayer vp)
    {
        Debug.Log("Cinemática terminada. Cargando el mapa...");
        
        if (Transicion.Instance != null)
        {
            Transicion.Instance.CargarEscena("Mapa"); // O el nombre exacto de tu escena
        }
        else
        {
            SceneManager.LoadScene("Mapa");
        }
    }
}