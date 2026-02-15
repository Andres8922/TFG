using UnityEngine;

public class DJ : MonoBehaviour
{
    public static DJ Instance;

    void Awake()
    {
        // Si no hay DJ, YO soy el DJ
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // ¡Házme inmortal!
        }
        // Si ya había un DJ antes (al volver al menú), me destruyo para no repetir la música
        else
        {
            Destroy(gameObject);
        }
    }
}