using Configuration;
using Events;
using Interface;
using ObjectPool;
using Player;
using UnityEngine;
using Zenject;

namespace Echo
{
    public abstract class EchoSpawnerBase<T> : PoolManagerBase<T> where T : EchoBase
    {
        [Inject] protected GeneralPrefabConfigurations generalPrefabConfigurations;
        [Inject] protected PlayerConfiguration playerConfiguration;

        private Transform _parent;
        private GameObject  _prefab;
        [SerializeField] protected int _objectAmount;

        protected override void Start()
        {
            pool.Construct(_parent, _prefab, _objectAmount, this);

            _dataBase.APoolIsFilled();
        }
        protected void SetValues(Transform parent, GameObject prefab, int objectAmount)
        {
            _parent = parent;
            _prefab = prefab;
            _objectAmount = objectAmount;
        }
    }
}




