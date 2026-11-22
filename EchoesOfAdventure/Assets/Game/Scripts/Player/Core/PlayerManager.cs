using Events;
using System;
using UnityEngine;
using Zenject;

namespace Player
{
    public class PlayerManager : MonoBehaviour
    {
        [Inject] private ProjectEvents _projectEvents;

        private PlayerBase _fireCharacter;
        private PlayerBase _stoneCharacter;
        private PlayerBase _plantCharacter;

        private PlayerBase _currentPlayer;

        [SerializeField] private Transform startPointTransform;

        private int _playerCharIndex;
        
        public PlayerBase CurrentPlayer
        {
            get => _currentPlayer;
            set
            {
                if (_currentPlayer != null)
                {
                    // Deactivate the currently active character
                    ToggleCharacterActivity(_currentPlayer.CharacterType, false);
                }

                _currentPlayer = value;

                // Activate the new character
                ToggleCharacterActivity(_currentPlayer.CharacterType, true);
                
                _projectEvents.Character_CharacterSelected?.Invoke(_currentPlayer.CharacterType);
                _projectEvents.Character_TransmitCurrent?.Invoke(_currentPlayer);
            }
        }

        #region Unity Methods
        private void Awake()
        {
            PlayerBase[] characters = transform.GetComponentsInChildren<PlayerBase>(true);

            if (characters.Length > 3)
                throw new InvalidOperationException("There are more than 3 Characters in the Scene!");

            for (int i = 0; i < characters.Length; i++)
            {
                switch (characters[i].CharacterType)
                {
                    case CharacterType.Fire:
                        if (_fireCharacter != null)
                            throw new InvalidOperationException("Fire Character was already filled!");

                        _fireCharacter = characters[i];
                        break;

                    case CharacterType.Stone:
                        if (_stoneCharacter != null)
                            throw new InvalidOperationException("Stone Character was already filled!");

                        _stoneCharacter = characters[i];
                        break;

                    case CharacterType.Plant:
                        if (_plantCharacter != null)
                            throw new InvalidOperationException("Plant Character was already filled!");

                        _plantCharacter = characters[i];
                        break;
                }

                if (!characters[i].gameObject.activeSelf)
                    continue;

                if (_currentPlayer != null)
                    throw new InvalidOperationException("There are more than 1 active Characters!");

                _currentPlayer = characters[i];

            }

            _projectEvents.Character_RequestCharacterSwitch += SwitchCharacter;
            _projectEvents.DeactivateCurrentCharacter += DeactivateCurrentCharacter;
            _projectEvents.Character_DeactivateAllCharacters += DeactivateAllCharacters;
        }


        private void OnDestroy()
        {
            _projectEvents.Character_RequestCharacterSwitch -= SwitchCharacter;
            _projectEvents.DeactivateCurrentCharacter -= DeactivateCurrentCharacter;
            _projectEvents.Character_DeactivateAllCharacters -= DeactivateAllCharacters;
        }

        #endregion

        private void SwitchCharacter(CharacterType type)
        {
            switch (type)
            {
                case CharacterType.Fire:
                    CurrentPlayer = _fireCharacter;
                    _projectEvents.SetCameraTarget(CurrentPlayer.gameObject);
                    break;
                case CharacterType.Stone:
                    CurrentPlayer = _stoneCharacter;
                    _projectEvents.SetCameraTarget(CurrentPlayer.gameObject);
                    break;
                case CharacterType.Plant:
                    CurrentPlayer = _plantCharacter;
                    _projectEvents.SetCameraTarget(CurrentPlayer.gameObject);
                    break;
            }
        }
        
        
        private void ToggleCharacterActivity(CharacterType characterType, bool isActive)
        {
            PlayerBase characterToToggle = characterType switch
            {
                CharacterType.Fire => _fireCharacter,
                CharacterType.Stone => _stoneCharacter,
                CharacterType.Plant => _plantCharacter,
                _ => throw new ArgumentOutOfRangeException()
            };

            // Set the position of the newly activated character
            SetPosition(characterToToggle.CharacterType);
            
            characterToToggle.gameObject.SetActive(isActive);
            characterToToggle.ResetCharacter();
        }
        
        private void SetPosition(CharacterType characterType)
        {
            // Set the position of the new character to the position of the current character
            if (_currentPlayer != null)
            {
                PlayerBase characterToSet = characterType switch
                {
                    CharacterType.Fire => _fireCharacter,
                    CharacterType.Stone => _stoneCharacter,
                    CharacterType.Plant => _plantCharacter,
                    _ => throw new ArgumentOutOfRangeException()
                };

                characterToSet.transform.position = startPointTransform.position;
            }
        }

        private void DeactivateCurrentCharacter()
        {
            if (_currentPlayer != null)
            {
                // Deactivate the currently active character
                ToggleCharacterActivity(_currentPlayer.CharacterType, false);
            }
        }


        private void DeactivateAllCharacters()
        {
            ToggleCharacterActivity(CharacterType.Plant, false);
            ToggleCharacterActivity(CharacterType.Stone, false);
            ToggleCharacterActivity(CharacterType.Fire, false);
        }
    }
}

