using UnityEngine;
using Zenject;


namespace Player
{
    public abstract class PlayerSystem : MonoBehaviour
    {
        [Inject] protected PlayerBase _player;
        [Inject] protected PlayerEvents _playerEvents;
    }
}

