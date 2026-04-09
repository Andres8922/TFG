using UnityEngine;

[CreateAssetMenu(fileName = "NuevaHabilidad", menuName = "Habilidades/Nueva Habilidad")]
public class Habilidad : ScriptableObject
{
    [Header("Datos Generales")]
    public string nombreHabilidad;
    public Sprite iconoHabilidad;

    [TextArea(2, 3)]
    public string descripcion;

    [Header("Tipo de Habilidad")]
    [Tooltip("Marca esto si la habilidad es una Pasiva (efecto automático). Déjalo desmarcado si es un Ataque Activo.")]
    public bool esPasiva;

    [Header("Si es Ataque Activo...")]
    public int costeMana;
    public int multiplicadorDaño = 2; // Por si quieres que unas peguen más fuerte que otras

    [Header("Si es Pasiva...")]
    public TipoPasiva tipoPasiva;
    public int valorPasiva; // Ej: Cuánta vida regenera por turno
}

public enum TipoPasiva
{
    Ninguna, // Para los ataques activos
    RegeneracionVidaTurno,
    RegeneracionManaTurno,
    MenteClara // Ej: Empieza con maná al máximo
}

