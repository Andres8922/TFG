using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(this.gameObject); }
        else { Instance = this; DontDestroyOnLoad(gameObject); }
    }

    public void CargarNivel(TipoNodo tipo)
    {
        switch (tipo)
        {
            case TipoNodo.Inicio:
                break;
            case TipoNodo.CombateFacil:
            case TipoNodo.CombateDificil:
                DatosGlobales.tipoNodoActual = tipo;
                Transicion.Instance.CargarEscena("Combate");
                break;
            case TipoNodo.Tienda:
                Transicion.Instance.CargarEscena("Tienda");
                break;
            case TipoNodo.Evento:
                Transicion.Instance.CargarEscena("Evento");
                break;
            case TipoNodo.Jefe:
                DatosGlobales.tipoNodoActual = tipo;
                Transicion.Instance.CargarEscena("Combate");
                break;
            default:
                Debug.LogWarning("Tipo de nodo no configurado para carga de escena.");
                break;
        }
    }
}
