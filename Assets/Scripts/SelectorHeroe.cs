using UnityEngine;
using UnityEngine.SceneManagement; // Necesario para cambiar de escena

public class SelectorHeroe : MonoBehaviour
{
    // Esta función la llamaremos desde los botones
    // indice: 0 para Caballero, 1 para Mago
    public void ElegirHeroe(int indice)
    {
        // 1. Guardamos la elección en el Cerebro
        GameManager.Instance.heroeSeleccionado = indice;

        Debug.Log("Héroe seleccionado: " + indice);

        // 2. Cargamos la escena del Mapa
        // Asegúrate de llamar a tu nueva escena "Mapa"
        Transicion.Instance.CargarEscena("Mapa");
    }
}