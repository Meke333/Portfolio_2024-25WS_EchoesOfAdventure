using UnityEngine;

namespace Player
{
    [CreateAssetMenu(fileName = nameof(CharacterSettingsConfiguration), menuName = Constants.Project.AssetMenuStart + nameof(CharacterSettingsConfiguration))]
    public class CharacterSettingsConfiguration : ScriptableObject
    {
        [field: SerializeField] public float FireAbilityStopCooldownSeconds { get; private set; } = 0.5f;

    
    }

    

}
