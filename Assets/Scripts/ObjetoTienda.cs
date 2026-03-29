using UnityEngine;

// Esta línea mágica nos permitirá crear estos objetos haciendo clic derecho en Unity
[CreateAssetMenu(fileName = "NuevoObjetoTienda", menuName = "Tienda/Objeto de Tienda")]
public class ObjetoTienda : ScriptableObject
{
    [Header("Información Visual")]
    public string nombreObjeto;
    public int precio;
    public Sprite iconoObjeto;

    [TextArea(2, 3)]
    public string descripcion;

    [Header("Efectos del Objeto")]
    public TipoEfectoTienda tipoEfecto;
    public int valorEfecto; // Ej: Si es cura, ¿cuánto cura? Si es fuerza, ¿cuánta suma?
}

// Lista de posibles efectos que puede tener un objeto en tu juego
public enum TipoEfectoTienda
{
    Curacion,
    Mana,
    DefensaExtra,
    FuerzaExtra,
    MejoraFuerza,
    MejoraDefensa
}