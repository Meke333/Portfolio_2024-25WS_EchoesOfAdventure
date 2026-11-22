using NaughtyAttributes;
using UnityEngine;


[CreateAssetMenu(fileName = nameof(InteractableConfigurations), menuName = Constants.Project.AssetMenuStart + nameof(InteractableConfigurations))]
public class InteractableConfigurations : ScriptableObject
{
    [field: SerializeField, Foldout("Woodbox")] public float TimeBeforeWoodboxBreaks { get; private set; }
    [field: SerializeField, Foldout("Woodbox")] public Vector2 WoodboxResetPositionWhenInteracted { get; private set; } = new Vector2(0, 0.065f);
    [field: SerializeField, Foldout("Waterfall")] public float TimeBeforeWaterHurtsFireCharacter { get; private set; }
    [field: SerializeField, Foldout("FireEcho")] public float TimeBeforeFireKillsCharacter { get; private set; }
    [field: SerializeField, Foldout("FireEcho")] public float TimeBeforeHigherJumpFades { get; private set; }

    [field: SerializeField, Foldout("Plants")] public float TimeForPlantEchoGrowsVisually { get; private set; } = 1f;
    [field: SerializeField, Foldout("Plants")] public AnimationCurve AnimationCurveOfgrowingPlantEcho { get; private set; }


}
