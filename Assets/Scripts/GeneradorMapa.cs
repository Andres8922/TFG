using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GeneradorMapa : MonoBehaviour
{
    public static GeneradorMapa Instance;

    [Header("Configuración Visual")]
    public GameObject nodoPrefab;
    public GameObject playerIconPrefab; // ¡NUEVO! Arrastra aquí un pequeño icono de pixel art (p.ej un escudo)
    public Transform contenedor;
    public LineRenderer lineaPrefab;

    [Header("Matemáticas del Árbol Tumbado")]
    public int totalPisos = 10;
    public float distanciaHorizontal = 2.5f;
    public float distanciaVertical = 1.8f;

    [Header("Matemáticas de Conexiones (¡Prioridad 3!)")]
    // Cuántos nodos de branching tendrá el mapa (divergencia)
    [Range(0f, 1f)] public float probabilidadDeRamaExtra = 0.3f;

    // Estructura de datos para guardar el mapa generado
    private List<List<GameObject>> mapaGenerado = new List<List<GameObject>>();
    private List<LineRenderer> lineasGeneradas = new List<LineRenderer>();

    // Estado del jugador (¡Prioridad 1!)
    private GameObject playerIcon;
    private int pisoActualJugador = -1;
    private GameObject nodoActualJugador;
    private HashSet<string> nodosCompletados = new HashSet<string>();

    void Awake() { Instance = this; }

    void Start()
    {
        GenerarMapaArbolTumbado();
    }

    [ContextMenu("Generar Nuevo Mapa")]
    public void GenerarMapaArbolTumbado()
    {
        LimpiarMapaAnterior();

        // Fase 1: Creación de Nodos y Posicionamiento (¡Prioridad 2 Centrado!)
        GenerarNodosFormales();

        // Fase 2: Conexiones de Árbol (¡Prioridad 3 Lógica!)
        EstablecerConexionesArbol();

        // Fase 3: Estado Inicial (¡Prioridad 1 Colores!)
        InicializarEstadoJugador();
    }

    void LimpiarMapaAnterior()
    {
        foreach (Transform hijo in contenedor) Destroy(hijo.gameObject);
        mapaGenerado.Clear();
        lineasGeneradas.Clear();
        if (playerIcon != null) Destroy(playerIcon);
    }

    //---------------------------------------------------------
    // ¡PRIORIDAD 2: CENTRADO Y FORMA ORGÁNICA!
    //---------------------------------------------------------
    void GenerarNodosFormales()
    {
        // ... (el código matemático del centrado que ya tenías está bien) ...
        float anchoTotalMapa = (totalPisos - 1) * distanciaHorizontal;
        float inicioX = -anchoTotalMapa / 2f;

        for (int p = 0; p < totalPisos; p++)
        {
            List<GameObject> nodosDeEstePiso = new List<GameObject>();

            //---------------------------------------------------------
            // NUEVA LÓGICA DE CANTIDAD DE NODOS (Árbol Tumbado)
            //---------------------------------------------------------
            int cantidadNodos = 0;
            if (p == 0) cantidadNodos = 1; // Inicio (1 nodo)
            else if (p == totalPisos - 1) cantidadNodos = 1; // Jefe (1 nodo)
            else
            {
                // Queremos que el árbol se abra mucho en el centro (la copa) y sea caótico.
                // Pisos 1-2 (Trunk): Se abre un poco (2-3 ramas)
                if (p < 3) cantidadNodos = Random.Range(2, 4);
                // Pisos 3 a (N-3) (Copa del árbol): ¡Ramas locas! (4-6 nodos)
                else if (p < totalPisos - 3) cantidadNodos = Random.Range(4, 7);
                // Pisos finales (Estrechamiento): Vuelve a estrecharse hacia el jefe (2-4 nodos)
                else cantidadNodos = Random.Range(2, 5);
            }

            // ... (el resto del código de posicionamiento X/Y sigue igual de bien) ...
            float posX = inicioX + (p * distanciaHorizontal);
            float altoTotalPiso = (cantidadNodos - 1) * distanciaVertical;
            float inicioY = altoTotalPiso / 2f;

            for (int i = 0; i < cantidadNodos; i++)
            {
                Vector3 posicion = new Vector3(posX, inicioY - (i * distanciaVertical), 0);

                GameObject nuevoNodo = Instantiate(nodoPrefab, posicion, Quaternion.identity);
                nuevoNodo.transform.SetParent(contenedor);
                nuevoNodo.name = $"Nodo_{p}_{i}";

                nodosDeEstePiso.Add(nuevoNodo);
            }
            mapaGenerado.Add(nodosDeEstePiso);
        }
    }

    //---------------------------------------------------------
    // ¡PRIORIDAD 3: LÓGICA DE ÁRBOL TUMBADO!
    //---------------------------------------------------------
    // Aquí implementamos reglas para que no haya 4 conexiones locas
    void EstablecerConexionesArbol()
    {
        for (int p = 0; p < totalPisos - 1; p++) // De izquierda a derecha
        {
            List<GameObject> nodosPisoActual = mapaGenerado[p];
            List<GameObject> nodosPisoSiguiente = mapaGenerado[p + 1];

            // Regla 1: Todo nodo DEBE tener al menos una salida
            for (int i = 0; i < nodosPisoActual.Count; i++)
            {
                // Conectamos [p][i] con el nodo [p+1] que esté más cerca verticalmente (i o i+1)
                int indiceSiguienteTarget = Mathf.Clamp(i, 0, nodosPisoSiguiente.Count - 1);
                DibujarLineaConceptual(nodosPisoActual[i], nodosPisoSiguiente[indiceSiguienteTarget]);

                // Regla 2: Branching controlado (divergencia)
                // Solo algunos nodos crean una bifurcación hacia el siguiente nodo
                if (i < nodosPisoSiguiente.Count - 1 && Random.value < probabilidadDeRamaExtra)
                {
                    DibujarLineaConceptual(nodosPisoActual[i], nodosPisoSiguiente[indiceSiguienteTarget + 1]);
                }
            }
        }
    }

    //---------------------------------------------------------
    // ¡PRIORIDAD 1: COLORES GRIS/AZUL Y JUGADOR!
    //---------------------------------------------------------
    void InicializarEstadoJugador()
    {
        // 1. Ponemos el icono en el Start
        if (playerIconPrefab != null && mapaGenerado.Count > 0)
        {
            playerIcon = Instantiate(playerIconPrefab, mapaGenerado[0][0].transform.position, Quaternion.identity);
            pisoActualJugador = 0;
            nodoActualJugador = mapaGenerado[0][0];
        }

        ActualizarVisibilidadMapa();
    }

    public void ActualizarVisibilidadMapa()
    {
        // Regla: Todo lo que no sea el Start o el Boss es Color.gray por defecto (niebla)
        for (int p = 0; p < mapaGenerado.Count; p++)
        {
            for (int i = 0; i < mapaGenerado[p].Count; i++)
            {
                GameObject nodo = mapaGenerado[p][i];
                SpriteRenderer sr = nodo.GetComponent<SpriteRenderer>();
                if (sr == null) continue;

                string idNodo = $"{p}_{i}";

                if (p == 0) sr.color = Color.white; // Start es visible
                else if (p == totalPisos - 1) sr.color = Color.red; // Boss es visible
                else if (nodosCompletados.Contains(idNodo))
                {
                    // ¡NUEVO: Azulado al completar!
                    sr.color = new Color(0f, 0.7f, 1f); // Azul cian suave
                }
                else
                {
                    sr.color = Color.gray; // NIEBLA POR DEFECTO
                }
            }
        }

        // 2. Pintamos las líneas
        // (conceptual: las líneas que salen del nodo actual se pintan de blanco, el resto gris oscuro)
    }

    // Función auxiliar para dibujar (reemplaza tu DibujarLinea)
    void DibujarLineaConceptual(GameObject origen, GameObject destino)
    {
        LineRenderer linea = Instantiate(lineaPrefab, Vector3.zero, Quaternion.identity);
        linea.transform.SetParent(contenedor);
        linea.positionCount = 2;
        linea.SetPosition(0, origen.transform.position);
        linea.SetPosition(1, destino.transform.position);
        // (Conceptual: por defecto gris oscuro)
        linea.startColor = Color.gray * 0.5f;
        linea.endColor = Color.gray * 0.5f;
        lineasGeneradas.Add(linea);
    }
}