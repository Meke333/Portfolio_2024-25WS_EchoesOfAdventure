using Events;
using UnityEngine;
using Zenject;


namespace Player
{
    public class PlayerFactory : MonoBehaviour
    {
        //Inject
        [Inject] private ProjectEvents _projectEvents;
        private PlayerConfiguration _playerConfiguration;
        [Inject] private IInstantiator _instantiator;

        public PlayerBase InstantiatePlayer()
        {
            PlayerBase result;
            result = _instantiator.InstantiatePrefab(_playerConfiguration, transform).GetComponent<PlayerBase>();
            return result;
        }
    }
}

