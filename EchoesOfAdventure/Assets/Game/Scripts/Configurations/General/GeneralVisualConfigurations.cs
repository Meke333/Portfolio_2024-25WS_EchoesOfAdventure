
using NaughtyAttributes;
using UnityEngine;

namespace Configuration
{
    [CreateAssetMenu(fileName = nameof(GeneralVisualConfigurations), menuName = Constants.Project.AssetMenuStart + nameof(GeneralVisualConfigurations))]
    public class GeneralVisualConfigurations : ScriptableObject
	{
        //Material
        [field: SerializeField, Foldout("Material")] public Material NormalWoodboxMaterial { get; private set; }
        [field: SerializeField, Foldout("Material")] public Material StoneWoodboxMaterial { get; private set; }


        //VFX


    }
}