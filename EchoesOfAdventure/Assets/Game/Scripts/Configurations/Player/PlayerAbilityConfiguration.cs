using UnityEngine;

namespace Player
{
    [CreateAssetMenu(fileName = nameof(PlayerAbilityConfiguration), menuName = Constants.Project.AssetMenuStart + nameof(PlayerAbilityConfiguration))]
    public class PlayerAbilityConfiguration : ScriptableObject
    {
        [field: SerializeField]
        [field: Tooltip("Cooldown for Ability Triggering in seconds")]
        public float AbilityCooldown { get; private set; } = 0.25f;
    }
}

