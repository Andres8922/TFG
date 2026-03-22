using UnityEngine;
using UnityEngine.Video;

public class ControladorVideoHeroe : MonoBehaviour
{
    [Header("UI Elementos")]
    [Tooltip("Arrastra aquí tu objeto Pantalla_Video de la Jerarquía.")]
    public GameObject panelVideoUI; 

    [Header("Reproductor")]
    public VideoPlayer reproductorVideo;

    [Header("Clips de Vídeo de los Héroes")]
    public VideoClip clipCaballero;
    public VideoClip clipMago;

    void Start()
    {
        // Aseguramos que la pantalla de vídeo empiece apagada para no bloquear botones
        if (panelVideoUI != null) panelVideoUI.SetActive(false);
    }

    // Función unificada para no repetir código
    private void PrepararYReproducir(VideoClip clip)
    {
        if (reproductorVideo != null && clip != null && panelVideoUI != null)
        {
            panelVideoUI.SetActive(true);
            reproductorVideo.clip = clip;
            
            // ¡LA LÍNEA MÁGICA! Forzamos a que el vídeo rebobine al principio
            reproductorVideo.time = 0; 
            
            reproductorVideo.Play();
        }
    }

    public void ReproducirVideoCaballero()
    {
        PrepararYReproducir(clipCaballero);
    }

    public void ReproducirVideoMago()
    {
        PrepararYReproducir(clipMago);
    }

    // Función lista por si luego quieres poner un botón de "Cerrar" o "Atrás"
    public void CerrarVideo()
    {
        if (reproductorVideo != null) reproductorVideo.Stop();
        if (panelVideoUI != null) panelVideoUI.SetActive(false);
    }
}