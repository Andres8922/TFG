using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectorHeroe : MonoBehaviour
{
    [Header("--- DEBUG ---")]
    [Tooltip("Actívalo para desbloquear todos los héroes en pruebas. Desactívalo antes de presentar.")]
    public bool desbloquearTodosEnPruebas = false;

    void Start()
    {
        if (desbloquearTodosEnPruebas && GameManager.Instance != null)
        {
            for (int i = 0; i < GameManager.Instance.heroesDesbloqueados.Length; i++)
                GameManager.Instance.heroesDesbloqueados[i] = true;
        }
    }

    public void ElegirHeroe(int indice)
    {
        GameManager.Instance.heroeSeleccionado = indice;
        Debug.Log("Héroe seleccionado: " + indice);
        Transicion.Instance.CargarEscena("Mapa");
    }
}
