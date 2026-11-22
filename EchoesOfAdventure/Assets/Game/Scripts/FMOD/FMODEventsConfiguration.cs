using UnityEngine;
using FMODUnity;
using Player;
using System;
using Echo;
using Interactable;
using UI;

namespace Sounds
{
    [CreateAssetMenu(fileName = nameof(FMODEventsConfiguration), menuName = Constants.Project.AssetMenuStart + nameof(FMODEventsConfiguration))]
    public class FMODEventsConfiguration : ScriptableObject
    {
        [field: Header("Music")]
        [field: SerializeField] public EventReference Music_Menu { get; private set; }
        [field: SerializeField] public EventReference Music_Level { get; private set; }



        [field: Header("Collectible SFX")]
        [field: SerializeField] public EventReference Collectible_Collected { get; private set; }
        [field: SerializeField] public EventReference Collectible_Ambience { get; private set; }


        [field: Header("Player SFX")]

        [field: SerializeField] public EventReference Player_Steps { get; private set; }
        [field: SerializeField] public EventReference Character_Jump { get; private set; }
        [field: SerializeField] public EventReference Character_Land { get; private set; }

        [field: Header("Echo SFX")]
        [field: SerializeField] public EventReference Fire_Echo { get; private set; }
        [field: SerializeField] public EventReference Plant_Echo { get; private set; }
        [field: SerializeField] public EventReference Stone_Echo { get; private set; }

        [field: SerializeField] public EventReference Fire_Emitter { get; private set; }
        [field: SerializeField] public EventReference Plant_Emitter { get; private set; }
        [field: SerializeField] public EventReference Stone_Emitter { get; private set; }


        [field: Header("Mukao SFX")]
        [field: SerializeField] public EventReference Mukao_Ability { get; private set; }
        [field: SerializeField] public EventReference Mukao_FireJump { get; private set; }


        [field: Header("Tamo SFX")]
        [field: SerializeField] public EventReference Tamo_Ability { get; private set; }
        [field: SerializeField] public EventReference Tamo_Climb { get; private set; }
        [field: SerializeField] public EventReference Tamo_WaterWalk { get; private set; }
        [field: SerializeField] public EventReference Tamo_TurnToStone { get; private set; }


        [field: Header("Cenali SFX")]
        [field: SerializeField] public EventReference Cenali_Ability { get; private set; }
        [field: SerializeField] public EventReference Cenali_Climb { get; private set; }
        [field: SerializeField] public EventReference Cenali_WaterWalk { get; private set; }


        [field: Header("WoodenBox SFX")]
        [field: SerializeField] public EventReference WoodenBox_Break { get; private set; }
        [field: SerializeField] public EventReference WoodenBox_Drag { get; private set; }
        [field: SerializeField] public EventReference WoodenBox_Fall { get; private set; }

        [field: Header("Waterfall SFX")]
        [field: SerializeField] public EventReference Waterfall_Emitter { get; private set; }
        [field: SerializeField] public EventReference Waterfall_Enter { get; private set; }
        [field: SerializeField] public EventReference Waterfall_Exit { get; private set; }

        [field: Header("UI")]
        [field: SerializeField] public EventReference UI_Back { get; private set; }
        [field: SerializeField] public EventReference UI_Select { get; private set; }
        [field: SerializeField] public EventReference UI_Reset { get; private set; }
        [field: SerializeField] public EventReference UI_Switch { get; private set; }
    
        public EventReference GetEventReference(PlayerSoundType type)
        {
            EventReference eventReference = default;

            switch (type)
            {
                case PlayerSoundType.Player_Steps:
                    eventReference = Player_Steps;
                    break;
                case PlayerSoundType.Character_Jump:
                    eventReference = Character_Jump;
                    break;
                case PlayerSoundType.Character_Land:
                    eventReference = Character_Land;
                    break;
                case PlayerSoundType.Mukao_Ability:
                    eventReference = Mukao_Ability;
                    break;
                case PlayerSoundType.Mukao_FireJump:
                    eventReference = Mukao_FireJump;
                    break;
                case PlayerSoundType.Tamo_Ability:
                    eventReference = Tamo_Ability;
                    break;
                case PlayerSoundType.Tamo_Climb:
                    eventReference = Tamo_Climb;
                    break;
                case PlayerSoundType.Tamo_WaterWalk:
                    eventReference = Tamo_WaterWalk;
                    break;
                case PlayerSoundType.Tamo_TurnToStone:
                    eventReference = Tamo_TurnToStone;
                    break;
                case PlayerSoundType.Cenali_Ability:
                    eventReference = Cenali_Ability;
                    break;
                case PlayerSoundType.Cenali_Climb:
                    eventReference = Cenali_Climb;
                    break;
                case PlayerSoundType.Cenali_WaterWalk:
                    eventReference = Cenali_WaterWalk;
                    break;
                case PlayerSoundType.Canali_GrowingVine:
                    //eventReference = 
                    break;
                default:
                    throw new InvalidOperationException("PlayerSoundType does not contain it.");
            }

            return eventReference;
        }

        public EventReference GetEventReference(EchoSoundType type)
        {
            EventReference eventReference = default;

            //To-Do: Fill in other Types
            switch (type)
            {
                case EchoSoundType.Fire:
                    eventReference = Mukao_Ability;
                    break;
                case EchoSoundType.Plant:
                    eventReference = Cenali_Ability;
                    break;
                case EchoSoundType.Stone:
                    eventReference = Tamo_Ability;
                    break;
                case EchoSoundType.Fire_Ambience:
                    eventReference = Fire_Emitter;
                    break;
                case EchoSoundType.Stone_Ambience:
                    eventReference = Stone_Emitter;
                    break;
                case EchoSoundType.Plant_Ambience:
                    eventReference = Plant_Emitter;
                    break;
                case EchoSoundType.Fire_Ambience_Echo:
                    eventReference = Fire_Echo;
                    break;
                case EchoSoundType.Stone_Ambience_Echo:
                    eventReference = Stone_Echo;
                    break;
                case EchoSoundType.Plant_Ambience_Echo:
                    eventReference = Plant_Echo;
                    break;
                default:
                    throw new InvalidOperationException("EchoSoundType does not contain it.");
            }

            return eventReference;
        }

        public EventReference GetEventReference(InteractableSoundType type)
        {
            EventReference eventReference = default;

            switch (type)
            {
                case InteractableSoundType.Collectible_Collected:
                    eventReference = Collectible_Collected;
                    break;
                case InteractableSoundType.WoodenBox_Break:
                    eventReference = WoodenBox_Break;
                    break;
                case InteractableSoundType.WoodenBox_Drag:
                    eventReference = WoodenBox_Drag;
                    break;
                case InteractableSoundType.WoodenBox_Fall:
                    eventReference = WoodenBox_Fall;
                    break;
                case InteractableSoundType.Collectible_Ambience:
                    eventReference = Collectible_Ambience;
                    break;
                case InteractableSoundType.Waterfall_Emitter:
                    eventReference = Waterfall_Emitter;
                    break;
                case InteractableSoundType.Fire_Burning:
                    eventReference = Fire_Echo;
                    break;
                default:
                    throw new InvalidOperationException("InteractableSoundType does not contain it.");
            }

            return eventReference;
        }

        public EventReference GetEventReference(MusicType type)
        {
            EventReference eventReference = default;

            switch (type)
            {
                case MusicType.Music_MainMenu:
                    eventReference = Music_Menu;
                    break;
                case MusicType.Music_Level:
                    eventReference = Music_Level;
                    break;
                default:
                    throw new InvalidOperationException("MusicType does not contain it.");
            }

            return eventReference;
        }

        public EventReference GetEventReference(UISoundType type)
        {
            EventReference eventReference = default;

            switch (type)
            {
                case UISoundType.UI_Select:
                    eventReference = UI_Select;
                    break;
                case UISoundType.UI_Back:
                    eventReference = UI_Back;
                    break;
                case UISoundType.UI_Reset:
                    eventReference = UI_Reset;
                    break;
                case UISoundType.UI_Switch:
                    eventReference = UI_Switch;
                    break;
                default:
                    throw new InvalidOperationException("UISoundType does not contain it.");
            }

            return eventReference;
        }

    }

}
