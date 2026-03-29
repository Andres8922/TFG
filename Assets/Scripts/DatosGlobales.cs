using System.Collections.Generic;

// Al ser "static", no hereda de MonoBehaviour y no se arrastra a ningún objeto
public static class DatosGlobales
{
    // Datos del Mapa
    public static bool hayPartidaGuardada = false;
    public static int semillaMapa; // ¡La clave mágica para recrear el mismo mapa!

    // Datos del Jugador en el Mapa
    public static int pisoActualJugador = 0;
    public static int nodoActualJugador = 0;
    public static HashSet<string> nodosCompletados = new HashSet<string>();

    // ¡Aprovechamos para guardar el oro aquí! Así la tienda y el mapa comparten la misma cartera
    public static int oroJugador = 100;
}