using UnityEngine;

public class GeneradorMapa : MonoBehaviour
{
    public GameObject nodoPrefab; // Aquí meteremos tu molde azul
    public Transform contenedor;  // Una "carpeta" para que no se desperdiguen en la jerarquía

    void Start()
    {
        // 1. Intentamos leer la dificultad. Si no existe el GameManager, usamos 0 (Fácil)
        int dificultad = 0;
        if (GameManager.Instance != null)
        {
            dificultad = GameManager.Instance.dificultad;
        }

        // 2. Decidimos cuántos pisos tiene la torre
        int pisos = 5; // Por defecto fácil
        if (dificultad == 1) pisos = 8;  // Medio
        if (dificultad == 2) pisos = 12; // Difícil

        // 3. ¡A construir!
        for (int i = 0; i < pisos; i++)
        {
            GenerarPiso(i);
        }
    }

    void GenerarPiso(int numeroPiso)
    {
        // Calculamos la posición: 
        // Y = Empezamos abajo (-3) y subimos 1.5 metros por cada piso
        float altura = -3f + (numeroPiso * 1.5f);

        // X = Un poco de aleatoriedad izquierda/derecha (-1.5 a 1.5)
        float desvio = Random.Range(-1.5f, 1.5f);

        // Z = 0 (¡VITAL! Para que esté delante del fondo que está en 100)
        Vector3 posicion = new Vector3(desvio, altura, 0);

        // Creamos la copia
        GameObject nuevoNodo = Instantiate(nodoPrefab, posicion, Quaternion.identity);

        // Lo guardamos en su carpeta
        nuevoNodo.transform.SetParent(contenedor);
        nuevoNodo.name = "Nivel_" + numeroPiso;
    }
}