using UnityEngine;

public class GeneradorMapa : MonoBehaviour
{
    public GameObject nodoPrefab;
    public Transform contenedor;

    void Start()
    {
        int dificultad = 0;
        if (GameManager.Instance != null) dificultad = GameManager.Instance.dificultad;

        // Pisos según dificultad
        int pisos = 5;
        if (dificultad == 1) pisos = 8;
        if (dificultad == 2) pisos = 12;

        for (int i = 0; i < pisos; i++)
        {
            GenerarPiso(i, pisos);
        }
    }

    void GenerarPiso(int pisoActual, int totalPisos)
    {
        // 1. Posición
        float altura = -3f + (pisoActual * 1.5f);
        float desvio = Random.Range(-1.5f, 1.5f);
        Vector3 posicion = new Vector3(desvio, altura, 0);

        // 2. Crear
        GameObject nuevoNodo = Instantiate(nodoPrefab, posicion, Quaternion.identity);
        nuevoNodo.transform.SetParent(contenedor);
        nuevoNodo.name = "Piso_" + pisoActual;

        // 3. Decidir QUÉ ES (La lógica del mapa)
        NodoMapa scriptDelNodo = nuevoNodo.GetComponent<NodoMapa>();

        if (pisoActual == totalPisos - 1)
        {
            // ¡El último siempre es el JEFE!
            scriptDelNodo.ConfigurarNodo(NodoMapa.TipoNodo.Jefe);
            // Lo ponemos en el centro para que quede épico
            nuevoNodo.transform.position = new Vector3(0, altura, 0);
        }
        else if (pisoActual == 0)
        {
            // El primero siempre es un enemigo facilito
            scriptDelNodo.ConfigurarNodo(NodoMapa.TipoNodo.Enemigo);
        }
        else
        {
            // Los de en medio: Azar (80% enemigo, 10% tienda, 10% tesoro)
            float azar = Random.Range(0f, 100f);
            if (azar < 80) scriptDelNodo.ConfigurarNodo(NodoMapa.TipoNodo.Enemigo);
            else if (azar < 90) scriptDelNodo.ConfigurarNodo(NodoMapa.TipoNodo.Tienda);
            else scriptDelNodo.ConfigurarNodo(NodoMapa.TipoNodo.Tesoro);
        }
    }
}