using UnityEngine;

public class ErasableLayerer : MonoBehaviour
{
    [SerializeField] private ErasableLayer layer;

    public ErasableLayer Layer { get { return layer; } }

    public enum ErasableLayer
    {
        Dirt,
        Mold,
        Soap,
        Slime
    }
}
