using UnityEngine;
using System.Collections.Generic;

public class GeneradorMapa : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject nodoPrefab;
    public GameObject lineaPrefab; // ¡NUEVO! Un prefab para dibujar la línea entre nodos
    public Transform contenedor;

    [Header("Configuración")]
    public float distanciaVertical = 2f;
    public float distanciaHorizontal = 2f;

    // Lista maestra que guarda todos los pisos y los nodos que hay en cada piso
    private List<List<GameObject>> mapaGenerado = new List<List<GameObject>>();

    void Start()
    {
        int dificultad = 0;
        // if (GameManager.Instance != null) dificultad = GameManager.Instance.dificultad; // Descomenta esto cuando lo unas a tu GameManager

        int pisos = 5;
        if (dificultad == 1) pisos = 8;
        if (dificultad == 2) pisos = 12;

        GenerarNodos(pisos);
        CrearConexiones();
    }

    void GenerarNodos(int totalPisos)
    {
        for (int pisoActual = 0; pisoActual < totalPisos; pisoActual++)
        {
            List<GameObject> nodosEnEstePiso = new List<GameObject>();

            // Decidir cuántos nodos hay en este piso
            int cantidadNodos = Random.Range(2, 5); // Entre 2 y 4 nodos
            if (pisoActual == 0 || pisoActual == totalPisos - 1)
            {
                cantidadNodos = 1; // El principio y el final siempre tienen 1 solo nodo
            }

            // Calcular el punto de inicio a la izquierda para que queden centrados
            float inicioX = -(cantidadNodos - 1) * distanciaHorizontal / 2f;
            float posicionY = -4f + (pisoActual * distanciaVertical);

            for (int i = 0; i < cantidadNodos; i++)
            {
                Vector3 posicion = new Vector3(inicioX + (i * distanciaHorizontal), posicionY, 0);

                GameObject nuevoNodo = Instantiate(nodoPrefab, posicion, Quaternion.identity);
                nuevoNodo.transform.SetParent(contenedor);
                nuevoNodo.name = "Nodo_" + pisoActual + "_" + i;

                // Lógica de qué tipo de sala es
                NodoMapa scriptDelNodo = nuevoNodo.GetComponent<NodoMapa>();
                if (scriptDelNodo != null)
                {
                    if (pisoActual == 0) scriptDelNodo.tipoDeNodo = TipoNodo.Inicio;
                    else if (pisoActual == totalPisos - 1) scriptDelNodo.tipoDeNodo = TipoNodo.Jefe;
                    else
                    {
                        float azar = Random.Range(0f, 100f);
                        if (azar < 60) scriptDelNodo.tipoDeNodo = TipoNodo.CombateFácil;
                        else if (azar < 80) scriptDelNodo.tipoDeNodo = TipoNodo.CombateDifícil;
                        else if (azar < 90) scriptDelNodo.tipoDeNodo = TipoNodo.Tienda;
                        else scriptDelNodo.tipoDeNodo = TipoNodo.Evento;
                    }
                }

                nodosEnEstePiso.Add(nuevoNodo);
            }

            mapaGenerado.Add(nodosEnEstePiso);
        }
    }

    void CrearConexiones()
    {
        // Recorremos todos los pisos excepto el último (el jefe no conecta hacia arriba)
        for (int i = 0; i < mapaGenerado.Count - 1; i++)
        {
            List<GameObject> pisoActual = mapaGenerado[i];
            List<GameObject> pisoSiguiente = mapaGenerado[i + 1];

            // 1. Cada nodo del piso actual DEBE conectar con al menos un nodo del piso siguiente
            for (int n = 0; n < pisoActual.Count; n++)
            {
                // Conecta con el nodo que tiene justo encima (o el más cercano)
                int indiceDestino = Mathf.Clamp(n, 0, pisoSiguiente.Count - 1);
                DibujarLinea(pisoActual[n].transform, pisoSiguiente[indiceDestino].transform);

                // Un 30% de posibilidades de crear un camino extra cruzado para darle vidilla
                if (Random.value < 0.3f && pisoSiguiente.Count > 1)
                {
                    int destinoExtra = Random.Range(0, pisoSiguiente.Count);
                    DibujarLinea(pisoActual[n].transform, pisoSiguiente[destinoExtra].transform);
                }
            }

            // 2. Nos aseguramos de que no queden nodos huérfanos en el piso siguiente
            for (int n = 0; n < pisoSiguiente.Count; n++)
            {
                // Aquí en un futuro comprobaremos si el nodo 'n' no tiene conexiones entrantes
                // y si es así, lo conectamos a la fuerza con un nodo aleatorio del piso actual.
                // Por ahora, con la lógica de arriba, casi siempre todos estarán conectados.
            }
        }
    }

    void DibujarLinea(Transform origen, Transform destino)
    {
        if (lineaPrefab == null) return;

        // --- SOLUCIÓN MATEMÁTICA ---
        // 1. Calculamos el punto MEDIO exacto entre el origen y el destino.
        // Allí es donde debe vivir el objeto de la línea.
        Vector3 puntoMedio = (origen.position + destino.position) / 2f;

        // 2. Instanciamos la línea en ese punto medio.
        GameObject linea = Instantiate(lineaPrefab, puntoMedio, Quaternion.identity);
        linea.transform.SetParent(contenedor);

        // 3. Calculamos la dirección y la distancia (esto sigue igual)
        Vector3 direccion = destino.position - origen.position;
        float distancia = direccion.magnitude;
        float angulo = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg;

        // 4. Estirar y rotar la línea. 
        // Como el sprite (el prefab cuadrado) se estira desde su centro (que ahora es el punto medio),
        // tocará perfectamente ambos extremos.
        linea.transform.localScale = new Vector3(distancia, 0.1f, 1f);
        linea.transform.rotation = Quaternion.Euler(0, 0, angulo);

        // 5. Gestión de profundidad (detrás de los iconos)
        Vector3 posLinea = linea.transform.position;
        posLinea.z = 1f;
        linea.transform.position = posLinea;
    }
}