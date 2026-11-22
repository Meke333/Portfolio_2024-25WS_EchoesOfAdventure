using Player;
using System;
using System.Collections.Generic;
using SceneLoader;
using UnityEngine;

namespace Events
{
    public class ProjectEvents
    {
        public Action<bool> Input_TogglePlayerInput;

        public Action<CharacterType> Character_RequestCharacterSwitch;
        public Action<CharacterType> Character_CharacterSelected;
        public Action Character_DeactivateAllCharacters;
        public Action Interactions_DeactivateAllInteracttions;

        public Action<int> OpenSelectionScreen;
        public Action OpenCurrentRunSelectionScreen;
        public Action CloseSelectionScreen;
        public Action ScreenClosed;
        public Action DeactivateCurrentCharacter;

        public Action FinishedLevel;
        public Action ResetGoal;

        public Action<int> ResetCurrentRun;

        public Action LoadNextLevel;
        public Action<int> LoadToLevelByIndex;
        public Action<SceneSetups> LoadToLevelByScene;
        
        // Save and load events
        public Action<int> SaveRequested;
        public Action<int> LoadRequested;
        public Action SavePossible;

        public Action LoadToCurrentRun;

        public Action ActivateSaveSystem;

        public Action LevelStart;

        public Action TutorialBoxTurnToStone;

        public Action PlayerFinishedLevel;
        public Action ReadyToSwitchLevel;
        
        // Camera Events
        public Action<GameObject> SetCameraTarget;
        
        // Tutorial Events
        public Action FinishedTutorial;

        public Action<PlayerBase> Character_TransmitCurrent;
        
        // Event to notify the UI of run and character updates
        public Action<int, CharacterType> UIChangeRunAndCharacterInformation;
        public Action UICollectibleCollected;
        public Action<InGameUI.UIState> UISetInGameState;

        public Action OpenLevelEndScreen;

        public Action<Vector3, float> VFX_Landing;
    }

}
