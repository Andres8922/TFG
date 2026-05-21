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
    [Tooltip("Marca esto si la habilidad es una Pasiva (efecto automatico). Dejalo desmarcado si es un Ataque Activo.")]
    public bool esPasiva;

    [Header("Si es Ataque Activo...")]
    public int costeMana;
    [Tooltip("Ataque: multiplica el danio base. Defensa: divide el danio recibido el siguiente turno. Buff: multiplica el danio del siguiente ataque.")]
    public int multiplicadorDanio = 2;
    public bool esDefensa;
    public bool esBuff;
    public bool esCuracion;
    [Tooltip("-1 = Cualquier heroe | 0 = Caballero | 1 = Mago | 2 = Elfa | 3 = Enano")]
    public int heroeRequerido = -1;
    [Tooltip("Trigger del Animator del heroe para esta habilidad. Si esta vacio usa AtqFuerte por defecto.")]
    public string triggerAnimacion;

    [Header("Si es Pasiva...")]
    public TipoPasiva tipoPasiva;
    public int valorPasiva;
}

public enum TipoPasiva
{
    Ninguna,
    RegeneracionVidaTurno,
    RegeneracionManaTurno,
    MenteClara,
    RegeneracionVidaAtaque,
    IncrementoFuerza,
    IncrementoDefensa,
    MultiplicacionOro,
    MultiplicacionExperiencia
}
