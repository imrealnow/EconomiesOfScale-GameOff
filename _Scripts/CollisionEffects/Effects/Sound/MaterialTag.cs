using UnityEngine;
public class MaterialTag : MonoBehaviour
{
    [SerializeField] public MaterialType materialType;

    public MaterialType MaterialType { get => materialType; }
}
