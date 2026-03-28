using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GeneradorMapa : MonoBehaviour
{
    public static GeneradorMapa Instance;

    [Header("Configuración Visual")]
    public GameObject nodoPrefab;
    public GameObject playerIconPrefab;
    public Transform contenedor;
    public LineRenderer lineaPrefab;

    [Header("Iconos de los Nodos (¡NUEVO!)")]
    public Sprite iconoInicio;
    public Sprite iconoCombateFacil;
    public Sprite iconoCombateDificil;
    public Sprite iconoEvento;
    public Sprite iconoTienda;
    public Sprite iconoJefe;

    [Header("Matemáticas del Árbol Tumbado")]
    public int totalPisos = 10;
    public float distanciaHorizontal = 2.5f;
    public float distanciaVertical = 1.8f;

    [Header("Matemáticas de Conexiones")]
    [Range(0f, 1f)] public float probabilidadDeRamaExtra = 0.3f;

    // Estructura de datos para guardar el mapa generado
    private List<List<GameObject>> mapaGenerado = new List<List<GameObject>>();
    private List<LineRenderer> lineasGeneradas = new List<LineRenderer>();

    // Estado del jugador
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
        GenerarNodosFormales();
        EstablecerConexionesArbol();
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
    // ¡PRIORIDAD 2: CENTRADO, FORMA Y AZAR!
    //---------------------------------------------------------
    void GenerarNodosFormales()
    {
        float anchoTotalMapa = (totalPisos - 1) * distanciaHorizontal;
        float inicioX = -anchoTotalMapa / 2f;

        for (int p = 0; p < totalPisos; p++)
        {
            List<GameObject> nodosDeEstePiso = new List<GameObject>();

            int cantidadNodos = 0;
            if (p == 0) cantidadNodos = 1;
            else if (p == totalPisos - 1) cantidadNodos = 1;
            else
            {
                if (p < 3) cantidadNodos = Random.Range(2, 4);
                else if (p < totalPisos - 3) cantidadNodos = Random.Range(4, 7);
                else cantidadNodos = Random.Range(2, 5);
            }

            float posX = inicioX + (p * distanciaHorizontal);
            float altoTotalPiso = (cantidadNodos - 1) * distanciaVertical;
            float inicioY = altoTotalPiso / 2f;

            for (int i = 0; i < cantidadNodos; i++)
            {
                Vector3 posicion = new Vector3(posX, inicioY - (i * distanciaVertical), 0);

                GameObject nuevoNodo = Instantiate(nodoPrefab, posicion, Quaternion.identity);
                nuevoNodo.transform.SetParent(contenedor);
                nuevoNodo.name = $"Nodo_{p}_{i}";

                NodoMapa scriptNodo = nuevoNodo.GetComponent<NodoMapa>();
                if (scriptNodo != null)
                {
                    scriptNodo.pisoIndex = p;
                    scriptNodo.nodoIndex = i;

                    // 1. ASIGNAMOS EL TIPO ALEATORIAMENTE
                    if (p == 0) scriptNodo.tipoDeNodo = TipoNodo.Inicio;
                    else if (p == totalPisos - 1) scriptNodo.tipoDeNodo = TipoNodo.Jefe;
                    else
                    {
                        // Ruleta de probabilidades
                        float probabilidad = Random.value; // Saca un número del 0.0 al 1.0

                        if (probabilidad < 0.45f) scriptNodo.tipoDeNodo = TipoNodo.CombateFacil; // 45%
                        else if (probabilidad < 0.65f) scriptNodo.tipoDeNodo = TipoNodo.CombateDificil; // 20%
                        else if (probabilidad < 0.85f) scriptNodo.tipoDeNodo = TipoNodo.Evento; // 20%
                        else scriptNodo.tipoDeNodo = TipoNodo.Tienda; // 15%
                    }

                    // 2. LE PONEMOS SU DIBUJO CORRESPONDIENTE
                    SpriteRenderer sr = nuevoNodo.GetComponent<SpriteRenderer>();
                    switch (scriptNodo.tipoDeNodo)
                    {
                        case TipoNodo.Inicio: if (iconoInicio != null) sr.sprite = iconoInicio; break;
                        case TipoNodo.CombateFacil: if (iconoCombateFacil != null) sr.sprite = iconoCombateFacil; break;
                        case TipoNodo.CombateDificil: if (iconoCombateDificil != null) sr.sprite = iconoCombateDificil; break;
                        case TipoNodo.Evento: if (iconoEvento != null) sr.sprite = iconoEvento; break;
                        case TipoNodo.Tienda: if (iconoTienda != null) sr.sprite = iconoTienda; break;
                        case TipoNodo.Jefe: if (iconoJefe != null) sr.sprite = iconoJefe; break;
                    }
                }

                nodosDeEstePiso.Add(nuevoNodo);
            }
            mapaGenerado.Add(nodosDeEstePiso);
        }
    }

    //---------------------------------------------------------
    // ¡PRIORIDAD 3: LÓGICA DE ÁRBOL TUMBADO CON MEMORIA!
    //---------------------------------------------------------
    void EstablecerConexionesArbol()
    {
        for (int p = 0; p < totalPisos - 1; p++)
        {
            List<GameObject> nodosPisoActual = mapaGenerado[p];
            List<GameObject> nodosPisoSiguiente = mapaGenerado[p + 1];

            for (int i = 0; i < nodosPisoActual.Count; i++)
            {
                GameObject nodoOrigen = nodosPisoActual[i];
                NodoMapa scriptOrigen = nodoOrigen.GetComponent<NodoMapa>();

                int indiceSiguienteTarget = Mathf.Clamp(i, 0, nodosPisoSiguiente.Count - 1);
                GameObject nodoDestino1 = nodosPisoSiguiente[indiceSiguienteTarget];

                DibujarLineaConceptual(nodoOrigen, nodoDestino1);
                scriptOrigen.nodosConectados.Add(nodoDestino1.GetComponent<NodoMapa>());

                if (i < nodosPisoSiguiente.Count - 1 && Random.value < probabilidadDeRamaExtra)
                {
                    GameObject nodoDestino2 = nodosPisoSiguiente[indiceSiguienteTarget + 1];
                    DibujarLineaConceptual(nodoOrigen, nodoDestino2);
                    scriptOrigen.nodosConectados.Add(nodoDestino2.GetComponent<NodoMapa>());
                }
            }
        }
    }

    //---------------------------------------------------------
    // ¡PRIORIDAD 1: COLORES GRIS/AZUL Y ESTADO!
    //---------------------------------------------------------
    void InicializarEstadoJugador()
    {
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
        for (int p = 0; p < mapaGenerado.Count; p++)
        {
            for (int i = 0; i < mapaGenerado[p].Count; i++)
            {
                GameObject nodo = mapaGenerado[p][i];
                SpriteRenderer sr = nodo.GetComponent<SpriteRenderer>();
                if (sr == null) continue;

                string idNodo = $"{p}_{i}";

                if (p == 0) sr.color = Color.white;
                else if (p == totalPisos - 1) sr.color = Color.red;
                else if (nodosCompletados.Contains(idNodo))
                {
                    sr.color = new Color(0f, 0.7f, 1f);
                }
                else
                {
                    sr.color = Color.gray;
                }
            }
        }
    }

    void DibujarLineaConceptual(GameObject origen, GameObject destino)
    {
        LineRenderer linea = Instantiate(lineaPrefab, Vector3.zero, Quaternion.identity);
        linea.transform.SetParent(contenedor);
        linea.positionCount = 2;
        linea.SetPosition(0, origen.transform.position);
        linea.SetPosition(1, destino.transform.position);
        linea.startColor = Color.gray * 0.5f;
        linea.endColor = Color.gray * 0.5f;
        lineasGeneradas.Add(linea);
    }

    //---------------------------------------------------------
    // ¡INTERACCIÓN DEL JUGADOR Y ESCENAS CON VALIDACIÓN!
    //---------------------------------------------------------
    public void IntentarMoverJugador(NodoMapa nodoDestino)
    {
        string idNodoDestino = $"{nodoDestino.pisoIndex}_{nodoDestino.nodoIndex}";

        if (nodosCompletados.Contains(idNodoDestino) || (nodoDestino.pisoIndex == 0 && nodoDestino.nodoIndex == 0))
        {
            pisoActualJugador = nodoDestino.pisoIndex;
            nodoActualJugador = nodoDestino.gameObject;
            playerIcon.transform.position = nodoActualJugador.transform.position;

            Debug.Log("Has vuelto a una zona segura. No se carga ninguna escena.");
            return;
        }

        NodoMapa scriptNodoActual = nodoActualJugador.GetComponent<NodoMapa>();

        if (scriptNodoActual.nodosConectados.Contains(nodoDestino))
        {
            string idNodoActual = $"{pisoActualJugador}_{scriptNodoActual.nodoIndex}";
            nodosCompletados.Add(idNodoActual);

            pisoActualJugador = nodoDestino.pisoIndex;
            nodoActualJugador = nodoDestino.gameObject;
            playerIcon.transform.position = nodoActualJugador.transform.position;

            ActualizarVisibilidadMapa();
            EjecutarEscenaDelNodo(nodoDestino);
        }
        else
        {
            Debug.Log("Movimiento inválido: No hay un camino directo hacia ese punto.");
        }
    }

    void EjecutarEscenaDelNodo(NodoMapa nodo)
    {
        Debug.Log($"¡Avisando al SceneLoader para cargar nivel tipo: {nodo.tipoDeNodo}!");

        if (SceneLoader.Instance != null)
        {
            // Llamamos exactamente a tu función "CargarNivel" y le pasamos el TipoNodo
            SceneLoader.Instance.CargarNivel(nodo.tipoDeNodo);
        }
        else
        {
            Debug.LogError("¡Ojo! No hay ningún objeto con el script SceneLoader en la escena.");
        }
    }
}