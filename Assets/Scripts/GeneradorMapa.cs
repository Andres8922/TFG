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

    [Header("Iconos de los Nodos")]
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

    private List<List<GameObject>> mapaGenerado = new List<List<GameObject>>();
    private List<LineRenderer> lineasGeneradas = new List<LineRenderer>();

    private GameObject playerIcon;
    private GameObject nodoActualJugador;

    void Awake() { Instance = this; }

    void Start()
    {
        // ¡LA MAGIA DE LA PERSISTENCIA ESTÁ AQUÍ!
        if (!DatosGlobales.hayPartidaGuardada)
        {
            // Primera vez: inventamos una semilla aleatoria
            DatosGlobales.semillaMapa = Random.Range(0, 999999);
            DatosGlobales.hayPartidaGuardada = true;
        }

        // Obligamos a Unity a usar esa semilla. Así el mapa se construirá IGUAL que la última vez.
        Random.InitState(DatosGlobales.semillaMapa);

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

                    if (p == 0) scriptNodo.tipoDeNodo = TipoNodo.Inicio;
                    else if (p == totalPisos - 1) scriptNodo.tipoDeNodo = TipoNodo.Jefe;
                    else
                    {
                        float probabilidad = Random.value;

                        if (probabilidad < 0.45f) scriptNodo.tipoDeNodo = TipoNodo.CombateFacil;
                        else if (probabilidad < 0.65f) scriptNodo.tipoDeNodo = TipoNodo.CombateDificil;
                        else if (probabilidad < 0.85f) scriptNodo.tipoDeNodo = TipoNodo.Evento;
                        else scriptNodo.tipoDeNodo = TipoNodo.Tienda;
                    }

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

    void InicializarEstadoJugador()
    {
        if (playerIconPrefab != null && mapaGenerado.Count > 0)
        {
            // Colocamos al jugador donde diga la Tarjeta de Memoria
            int p = DatosGlobales.pisoActualJugador;
            int i = DatosGlobales.nodoActualJugador;

            nodoActualJugador = mapaGenerado[p][i];
            playerIcon = Instantiate(playerIconPrefab, nodoActualJugador.transform.position, Quaternion.identity);
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
                else if (DatosGlobales.nodosCompletados.Contains(idNodo)) // Lee la memoria global
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

    public void IntentarMoverJugador(NodoMapa nodoDestino)
    {
        string idNodoDestino = $"{nodoDestino.pisoIndex}_{nodoDestino.nodoIndex}";

        // Comprobamos en la memoria global si ya completamos esto
        if (DatosGlobales.nodosCompletados.Contains(idNodoDestino) || (nodoDestino.pisoIndex == 0 && nodoDestino.nodoIndex == 0))
        {
            DatosGlobales.pisoActualJugador = nodoDestino.pisoIndex;
            DatosGlobales.nodoActualJugador = nodoDestino.nodoIndex;
            nodoActualJugador = nodoDestino.gameObject;
            playerIcon.transform.position = nodoActualJugador.transform.position;

            Debug.Log("Has vuelto a una zona segura. No se carga ninguna escena.");
            return;
        }

        NodoMapa scriptNodoActual = nodoActualJugador.GetComponent<NodoMapa>();

        if (scriptNodoActual.nodosConectados.Contains(nodoDestino))
        {
            // Guardamos el progreso en la memoria global antes de irnos
            string idNodoActual = $"{DatosGlobales.pisoActualJugador}_{scriptNodoActual.nodoIndex}";
            DatosGlobales.nodosCompletados.Add(idNodoActual);

            DatosGlobales.pisoActualJugador = nodoDestino.pisoIndex;
            DatosGlobales.nodoActualJugador = nodoDestino.nodoIndex;

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
            SceneLoader.Instance.CargarNivel(nodo.tipoDeNodo);
        }
        else
        {
            Debug.LogError("¡Ojo! No hay ningún objeto con el script SceneLoader en la escena.");
        }
    }
}