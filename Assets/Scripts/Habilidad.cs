using UnityEngine;

// Esto nos permitirá crear habilidades haciendo clic derecho > Habilidades > Nueva Habilidad
[CreateAssetMenu(fileName = "NuevaHabilidad", menuName = "Habilidades/Nueva Habilidad")]
public class Habilidad : ScriptableObject
{
    [Header("Datos de la Carta")]
    public string nombreHabilidad;
    public int costeMana;
    public Sprite iconoHabilidad;

    [TextArea(2, 3)]
    public string descripcion;

    // Aquí en el futuro puedes añadir cosas como "int danoBase" o "TipoAtaque tipo"
}