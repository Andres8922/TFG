using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Esta instancia permite acceder al GameManager desde cualquier script
    public static GameManager Instance;

    // Aquí guardamos los datos importantes
    public int heroeSeleccionado; // 0 = Mago, 1 = Caballero
    public int dificultad;        // 0 = Fácil, 1 = Medio, 2 = Difícil

    private void Awake()
    {
        // El patrón SINGLETON: asegura que solo haya un GameManager y no se borre
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // ¡ESTA ES LA MAGIA! No se destruye al cargar escena
        }
        else
        {
            Destroy(gameObject); // Si ya existe uno, borra este para no duplicar
        }
    }
}