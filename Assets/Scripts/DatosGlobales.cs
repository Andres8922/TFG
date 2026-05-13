using System.Collections.Generic;

public static class DatosGlobales
{
    // Datos del Mapa
    public static bool hayPartidaGuardada = false;
    public static int semillaMapa;

    // Datos del Jugador en el Mapa
    public static int pisoActualJugador = 0;
    public static int nodoActualJugador = 0;
    public static HashSet<string> nodosCompletados = new HashSet<string>();
    public static int oroJugador = 100;

    // �NUEVO! La mochila donde guardaremos las cartas/habilidades
    public static List<Habilidad> habilidadesJugador = new List<Habilidad>();

    // Contador de combates normales completados (para elegir enemigo secuencial)
    public static int combatesRealizados = 0;
}