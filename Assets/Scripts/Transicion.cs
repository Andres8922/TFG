using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; // Necesario para los tiempos de espera

public class Transicion : MonoBehaviour
{
    public static Transicion Instance;
    public CanvasGroup telonNegro; // Aquí conectaremos el Panel negro
    public float velocidad = 1f;

    void Awake()
    {
        // Esto es para que el Telón sobreviva al cambiar de escena
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // Si ya hay uno, borramos el nuevo
        }
    }

    void Start()
    {
        // Al empezar el juego, quitamos lo negro (Fade In)
        StartCoroutine(Fade(0));
    }

    // Esta función la llamarás desde tus botones
    public void CargarEscena(string nombreEscena)
    {
        StartCoroutine(ProcesoCarga(nombreEscena));
    }

    IEnumerator ProcesoCarga(string escena)
    {
        // 1. Oscurecer (Fade Out)
        telonNegro.blocksRaycasts = true; // Bloquea el ratón
        yield return StartCoroutine(Fade(1)); // Espera a que sea negro

        // 2. Cargar escena
        SceneManager.LoadScene(escena);

        // 3. Esperar un poco (opcional)
        yield return new WaitForSeconds(0.5f);

        // 4. Aclarar (Fade In)
        yield return StartCoroutine(Fade(0));
        telonNegro.blocksRaycasts = false; // Desbloquea el ratón
    }

    IEnumerator Fade(float objetivoAlpha)
    {
        // Mientras no lleguemos al objetivo (0 o 1)...
        while (Mathf.Abs(telonNegro.alpha - objetivoAlpha) > 0.01f)
        {
            // Cambiamos el alpha poco a poco
            telonNegro.alpha = Mathf.MoveTowards(telonNegro.alpha, objetivoAlpha, velocidad * Time.deltaTime);
            yield return null; // Esperar al siguiente frame
        }
        telonNegro.alpha = objetivoAlpha; // Asegurar que llega exacto
    }
}