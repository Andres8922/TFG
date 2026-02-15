using UnityEngine;
using UnityEngine.SceneManagement; // Necesario para cambiar de escena

public class SelectorDificultad : MonoBehaviour
{
    // Esta función la llamaremos desde los 3 botones (Fácil, Medio, Difícil)
    // indice: 0 = Fácil, 1 = Medio, 2 = Difícil
    public void ElegirDificultad(int indice)
    {
        // 1. Guardamos la dificultad en el GameManager
        // Como el GameManager no se destruye, este número se queda guardado para siempre
        GameManager.Instance.dificultad = indice;

        Debug.Log("Dificultad seleccionada: " + indice);

        // 2. Cargamos la siguiente pantalla: La Selección de Héroe
        // Asegúrate de que tu escena se llame exactamente así (o como la hayas nombrado)
        Transicion.Instance.CargarEscena("SeleccionHeroe");
    }
}