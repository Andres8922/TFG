using UnityEngine;
using UnityEngine.SceneManagement; // ¡IMPRESCINDIBLE para cargar escenas!

// Usaremos un Singleton para que este manager viva siempre
public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; } // El Singleton

    void Awake()
    {
        // Si ya hay un SceneLoader, destruimos este. Si no, lo guardamos y no lo destruimos al cambiar de escena.
        if (Instance != null && Instance != this) { Destroy(this.gameObject); }
        else { Instance = this; DontDestroyOnLoad(gameObject); }
    }

    // Esta es la función central que llama cada nodo
    public void CargarNivel(TipoNodo tipo)
    {
        switch (tipo)
        {
            case TipoNodo.Inicio:
                // No hacemos nada, el inicio no carga nivel
                break;
            case TipoNodo.CombateFacil:
            case TipoNodo.CombateDificil:
                // Cargamos la escena "Combate" que ya existe
                SceneManager.LoadScene("Combate");
                break;
            case TipoNodo.Tienda:
                // ¡ATENCIÓN! Debes crear una escena vacía llamada "Tienda"
                SceneManager.LoadScene("Tienda");
                break;
            case TipoNodo.Evento:
                // ¡ATENCIÓN! Debes crear una escena vacía llamada "Evento"
                SceneManager.LoadScene("Evento");
                break;
            case TipoNodo.Jefe:
                // Puedes cargar la misma de "Combate" o crear una para el jefe
                SceneManager.LoadScene("Combate");
                break;
            default:
                Debug.LogWarning("Tipo de nodo no configurado para carga de escena.");
                break;
        }
    }
}