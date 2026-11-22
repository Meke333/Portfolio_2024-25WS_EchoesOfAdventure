using Helper;
using Interface;
using ModestTree;
using Player;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Interactable
{
    public class PlayerStompers : PlayerSystem
    {
        [SerializeField] private Transform _hitbox;
        [SerializeField] private LayerMask _layerMask = LayerAndMaskHelper.ConvertNumberIntoLayer(Constants.Layers.Interactables);

        private bool _isActive = true;

        private List<IStompable> _savedlist = new List<IStompable>();


        private void OnEnable()
        {
            _playerEvents.Input_OnJumpInput += JumpInput;
        }

        private void OnDisable()
        {
            _playerEvents.Input_OnJumpInput += JumpInput;
        }

        private void FixedUpdate()
        {
            if (!_isActive)
                return;

            EvaluateHitbox();
        }

        private void JumpInput(bool obj)
        {
            if (!obj)
                return;

            for (int i = 0; i < _savedlist.Count; i++)
            {
                _savedlist[i].BreakStomp();
            }

            _savedlist = new List<IStompable>();

        }

        private void EvaluateHitbox()
        {
            IStompable[] list = Collider2DHelper.GetHitArray<IStompable>(_hitbox, _layerMask);

            if (list.Length == 0)
                _savedlist.Clear();
            
            

            for (int i = 0; i < list.Length; i++)
            {
                if (!_savedlist.Contains(list[i]))
                    continue;
                
                _savedlist.Remove(list[i]);
            }

            for (int i = 0; i < list.Length; i++)
            {
                list[i].StartStomp();
            }

            for (int i = 0; i < _savedlist.Count; i++)
            {
                _savedlist[i].StopStomp();
            }

            _savedlist = list.ToList();
        }
    }

}
