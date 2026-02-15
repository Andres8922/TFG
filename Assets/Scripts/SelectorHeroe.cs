using UnityEngine;
using UnityEngine.SceneManagement; // Necesario para cambiar de escena

public class SelectorHeroe : MonoBehaviour
{
    // Esta función la llamaremos desde los botones
    // indice: 0 para Mago, 1 para Caballero
    public void ElegirHeroe(int indice)
    {
        // 1. Guardamos la elección en el Cerebro
        GameManager.Instance.heroeSeleccionado = indice;

        Debug.Log("Héroe seleccionado: " + indice);

        // 2. Cargamos la escena del Mapa
        // Asegúrate de llamar a tu nueva escena "Mapa"
        SceneManager.LoadScene("Mapa");
    }
}