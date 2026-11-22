
using NaughtyAttributes;
using UnityEngine;

namespace Configuration
{
    [CreateAssetMenu(fileName = nameof(GeneralPrefabConfigurations), menuName = Constants.Project.AssetMenuStart + nameof(GeneralPrefabConfigurations))]
	public class GeneralPrefabConfigurations : ScriptableObject
	{
		//Echo
		[field: SerializeField, Foldout("Echo")] public GameObject FireEcho { get; private set; }
        [field: SerializeField, Foldout("Echo")] public GameObject PlantEcho { get; private set; }
        [field: SerializeField, Foldout("Echo")] public GameObject StoneEcho { get; private set; }


        //VFX
        [field: SerializeField, Foldout("VFX")] public GameObject LandingVFX { get; private set; }
        [field: SerializeField, Foldout("VFX")] public AnimationCurve LandingVFXAirtimeCurve { get; private set; }
    }
}