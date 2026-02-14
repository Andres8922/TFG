using UnityEngine;
using UnityEngine.UI;

public class ControlVolumenJuego : MonoBehaviour
{
    // Referencia al slider de volumen
    public Slider sliderVolumen;

    // Valor por defecto
    public float volumenInicial = 0.5f;

    void Start()
    {
        // Configurar slider si existe
        if (sliderVolumen != null)
        {
            // Establecer valor inicial
            sliderVolumen.value = volumenInicial;

            // Agregar listener para cambios
            sliderVolumen.onValueChanged.AddListener(CambiarVolumen);

            // Aplicar volumen inicial
            CambiarVolumen(volumenInicial);
        }
    }

    // Método que cambia el volumen global del juego
    public void CambiarVolumen(float nuevoVolumen)
    {
        // 0 = silencio, 1 = volumen máximo
        AudioListener.volume = Mathf.Clamp01(nuevoVolumen);

        // Opcional: Mostrar en consola
        Debug.Log($"Volumen cambiado a: {AudioListener.volume * 100}%");
    }

    // Método para bajar volumen (desde botón)
    public void BajarVolumen()
    {
        float volumenActual = AudioListener.volume;
        float nuevoVolumen = Mathf.Max(0f, volumenActual - 0.1f);

        CambiarVolumen(nuevoVolumen);

        // Actualizar slider si existe
        if (sliderVolumen != null)
        {
            sliderVolumen.value = nuevoVolumen;
        }
    }

    // Método para subir volumen (desde botón)
    public void SubirVolumen()
    {
        float volumenActual = AudioListener.volume;
        float nuevoVolumen = Mathf.Min(1f, volumenActual + 0.1f);

        CambiarVolumen(nuevoVolumen);

        // Actualizar slider si existe
        if (sliderVolumen != null)
        {
            sliderVolumen.value = nuevoVolumen;
        }
    }

    // Método para silenciar/desilenciar
    public void ToggleSilencio()
    {
        if (AudioListener.volume > 0f)
        {
            // Guardar volumen actual antes de silenciar
            PlayerPrefs.SetFloat("VolumenAntesDeSilencio", AudioListener.volume);
            CambiarVolumen(0f);
        }
        else
        {
            // Restaurar volumen anterior
            float volumenAnterior = PlayerPrefs.GetFloat("VolumenAntesDeSilencio", 0.5f);
            CambiarVolumen(volumenAnterior);
        }

        // Actualizar slider si existe
        if (sliderVolumen != null)
        {
            sliderVolumen.value = AudioListener.volume;
        }
    }
}
