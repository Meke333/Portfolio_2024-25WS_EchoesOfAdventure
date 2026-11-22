using Interface;
using Player;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Echo
{
    public class FireEcho : EchoBase
    {
        [Inject] private InteractableConfigurations _interactableConfigurations;

        [SerializeField] private Transform _hitbox;
        [SerializeField] private LayerMask _layerMask;
        [SerializeField] private CharacterMask _characterTypeLayer;
        [SerializeField] private CharacterMask _ignoredCharacterMask;

        private bool _isActive = true;

        private float _killTime;
        private float killTime
        { 
            get => _killTime;
            set
            {
                if (value < 0)
                {
                    return;
                }
                else if (value >= _interactableConfigurations.TimeBeforeFireKillsCharacter)
                {

                    HurtPlayer();
                    return;
                }

                debugTime = value;
                _killTime = value;
            }
        }

        public float debugTime;

        private List<IHurtBox> _savedHurtboxList = new List<IHurtBox>();

        private List<IFireJumpable> _savedFireJumpableList = new List<IFireJumpable>();
        
        
        private void Update()
        {
            killTime += _savedHurtboxList.Count > 0 ? Time.deltaTime :-Time.deltaTime;
        }

        private void FixedUpdate()
        {
            if (!_isActive)
                return;

            EvaluateHitbox();
        }

        private void EvaluateHitbox()
        {
            IHurtBox[] hurtlist = Collider2DHelper.GetHitArray<IHurtBox>(_hitbox, _layerMask);
            
           if (hurtlist.Length == 0)
            {
                _savedHurtboxList.Clear();
            }
                        
            for (int i = 0; i < hurtlist.Length; i++)
            {
                if (!_savedHurtboxList.Contains(hurtlist[i]))
                    continue;

                _savedHurtboxList.Remove(hurtlist[i]);
            }
            
            for (int i = 0; i < hurtlist.Length; i++)
            {
                IHurtBox temp = hurtlist[i];


                // Skip HurtBox if it matches the ignored character mask
                /*if (temp.CanGetHurt<CharacterMask>(_ignoredCharacterMask))
                {
                    Debug.Log("Fire Echo: CharacterType: " + temp.GetCharacterType() + "; ignoreMask: " + _ignoredCharacterMask);
                    Debug.Log("Fire Echo: Ignoring Character");
                    continue;
                }*/

               
                if (!temp.CanGetHurt<CharacterMask>(_characterTypeLayer))
                {
                    continue;
                }
                if (_savedHurtboxList.Contains(temp))
                {
                    continue;    
                }


                _savedHurtboxList.Add(temp);
            }

            

            IFireJumpable[] firelist = Collider2DHelper.GetHitArray<IFireJumpable>(_hitbox, _layerMask);
            List<IFireJumpable> removedJumpables = new List<IFireJumpable>();

            for (int i = 0; i < _savedFireJumpableList.Count; i++)
            {
                removedJumpables.Add(_savedFireJumpableList[i]);
            }

            for (int i = 0; i < firelist.Length; i++)
            {
                if (_savedFireJumpableList.Contains(firelist[i]))
                {
                    removedJumpables.Remove(firelist[i]);
                    continue;
                }

                firelist[i].SetFireJumpValue(true);
                _savedFireJumpableList.Add(firelist[i]);
            }

            for (int i = 0; i < removedJumpables.Count; i++)
            {
                removedJumpables[i].SetFireJumpValue(false);
                _savedFireJumpableList.Remove(removedJumpables[i]);
            }

            IBurnable[] burnlist = Collider2DHelper.GetHitArray<IBurnable>(_hitbox, _layerMask);

            for (int i = 0; i < burnlist.Length; i++)
            {
                if (burnlist[i].CanBurnDown())
                {
                    burnlist[i].BurnDown();
                    echoEvents.Fire_Burning?.Invoke(transform.position);
                }
            }
        }

        private void HurtPlayer()
        {


            for (int i = 0; i < _savedHurtboxList.Count; i++)
            {
                var hurtbox = _savedHurtboxList[i];
                _savedHurtboxList[i].GetDamage();
            }

            _savedHurtboxList.Clear();
        }



        public void SetFireActivity(bool isActive)
        {
            _isActive = isActive;
        }

        #region PoolObjectBase
        public override void ResetObject()
        {
            _isActive = true;
        }
        #endregion

        #region EchoBase
        protected override void NormalStateBehaviour()
        {
            return;
        }

        protected override void DestroyedStateBehaviour()
        {
            SetActivity(false);
        }

        protected override void BurnedStateBehaviour()
        {
            return;
        }
        #endregion
    }


}




