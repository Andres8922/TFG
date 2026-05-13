using UnityEngine;

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

    // Esta es la funci�n central que llama cada nodo
    public void CargarNivel(TipoNodo tipo)
    {
        switch (tipo)
        {
            case TipoNodo.Inicio:
                break;
            case TipoNodo.CombateFacil:
            case TipoNodo.CombateDificil:
                Transicion.Instance.CargarEscena("Combate");
                break;
            case TipoNodo.Tienda:
                Transicion.Instance.CargarEscena("Tienda");
                break;
            case TipoNodo.Evento:
                Transicion.Instance.CargarEscena("Evento");
                break;
            case TipoNodo.Jefe:
                Transicion.Instance.CargarEscena("Combate");
                break;
            default:
                Debug.LogWarning("Tipo de nodo no configurado para carga de escena.");
                break;
        }
    }
}