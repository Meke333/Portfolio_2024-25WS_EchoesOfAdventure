using NaughtyAttributes;
using UnityEngine;

namespace Player
{

    [CreateAssetMenu(fileName = nameof(PlayerConfiguration), menuName = Constants.Project.AssetMenuStart + nameof(PlayerConfiguration))]
    public class PlayerConfiguration : ScriptableObject
    {
        //Prefabs
        [field: SerializeField, Foldout("Player Prefabs")] 
        public GameObject FireCharacterPrefab { get; private set; }
        [field: SerializeField, Foldout("Player Prefabs")]
        public GameObject PlantCharacterPrefab { get; private set; }
        [field: SerializeField, Foldout("Player Prefabs")]
        public GameObject StoneCharacterPrefab { get; private set; }


        [field: SerializeField, Foldout("Ability Prefabs")]
        public GameObject FireBall { get; private set; }
        [field: SerializeField, Foldout("Ability Prefabs")]
        public GameObject FireEcho { get; private set; }
        [field: SerializeField, Foldout("Ability Prefabs")]
        public GameObject PlantEcho { get; private set; }
        [field: SerializeField, Foldout("Ability Prefabs")]
        public GameObject StoneEcho { get; private set; }

        //Input
        [field: SerializeField, Foldout("Input")]
        [Tooltip("Makes all Input snap to an integer. Prevents gamepads from walking slowly. Recommended value is true to ensure gamepad/keybaord parity.")]
        public bool SnapInput { get; private set; }

        
    }

}
