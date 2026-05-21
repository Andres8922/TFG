using System.Collections.Generic;

public static class DatosGlobales
{
    // Mapa
    public static bool hayPartidaGuardada = false;
    public static int semillaMapa;

    // Posición del jugador en el mapa
    public static int pisoActualJugador = 0;
    public static int nodoActualJugador = 0;
    public static HashSet<string> nodosCompletados = new HashSet<string>();
    public static int oroJugador = 100;

    public static List<Habilidad> habilidadesJugador = new List<Habilidad>();

    // Índice de combate para elegir enemigo de forma secuencial
    public static int combatesRealizados = 0;

    public static TipoNodo tipoNodoActual = TipoNodo.CombateFacil;
}
