using UnityEngine;

[CreateAssetMenu(fileName = "NuevoObjetoTienda", menuName = "Tienda/Objeto de Tienda")]
public class ObjetoTienda : ScriptableObject
{
    [Header("Información Visual")]
    public string nombreObjeto;
    public int precio;
    public Sprite iconoObjeto;

    [TextArea(2, 3)]
    public string descripcion;

    [Header("Inventario y Combate")]
    public int cantidadEnInventario;

    [Header("Efectos del Objeto")]
    public TipoEfectoTienda tipoEfecto;
    public int valorEfecto;
}

public enum TipoEfectoTienda
{
    Curacion,
    Mana,
    DefensaExtra,
    FuerzaExtra,
    MejoraFuerza,
    MejoraDefensa
}
