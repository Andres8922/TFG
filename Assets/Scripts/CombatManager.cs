using UnityEngine;

public class CombatManager : MonoBehaviour
{
    [Header("Héroe")]
    public GameObject[] listaHeroes; // Tus prefabs de héroes
    public Transform puntoHeroe;     // Donde nace el héroe

    [Header("Enemigo")]
    public GameObject enemigoPrefab; // <--- ESTE ES EL HUECO NUEVO
    public Transform puntoEnemigo;   // <--- Y ESTE TAMBIÉN

    void Start()
    {
        GenerarCombate();
    }

    void GenerarCombate()
    {
        // 1. GENERAR HÉROE
        int indice = 0;
        if (GameManager.Instance != null)
        {
            indice = GameManager.Instance.heroeSeleccionado;
        }

        Instantiate(listaHeroes[indice], puntoHeroe.position, Quaternion.identity);

        // 2. GENERAR ENEMIGO
        Instantiate(enemigoPrefab, puntoEnemigo.position, Quaternion.identity);
    }
}