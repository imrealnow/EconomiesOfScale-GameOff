using UnityEngine;

[CreateAssetMenu(fileName = "MaterialCollisionSound", menuName = "CollisionEffect/MaterialCollisionSound", order = 1)]
public class MaterialCollisionSound : CollisionEffect
{
    [Header("Sound")]
    public SoundManager soundManager;
    public AudioClip defaultSound;
    public float volume = 1f;
    public float pitch = 1f;

    [Header("Material Sounds")]
    public MaterialSound[] materialSounds;

    public override void ApplyEffect(CollisionContext context)
    {
        // Find MaterialTag on the other object
        MaterialTag tagger = context.collider.GetComponent<MaterialTag>();
        if (tagger == null)
        {
            soundManager.PlaySFX(defaultSound, context.point, volume, pitch);
        }
        else
        {
            MaterialSound materialSound = GetMaterialSound(tagger.MaterialType);
            if (materialSound == null)
            {
                soundManager.PlaySFX(defaultSound, context.point, volume, pitch);
                return;
            }
            materialSound.clip = materialSound.clip == null ? defaultSound : materialSound.clip;
            soundManager.PlaySFX(materialSound.clip, context.point, materialSound.volume, materialSound.pitch);
        }
    }

    private MaterialSound GetMaterialSound(MaterialType materialType)
    {
        foreach (MaterialSound materialSound in materialSounds)
        {
            if (materialSound.materialType == materialType)
            {
                return materialSound;
            }
        }
        return null;
    }
}

[System.Serializable]
public class MaterialSound
{
    public MaterialType materialType;
    public AudioClip clip;
    public float volume = 1f;
    public float pitch = 1f;
}
