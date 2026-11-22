using Configuration;
using ObjectPool;
using UnityEngine;
using Zenject;

namespace VFX
{
    public class VFXManagerBase<T> : PoolManagerBase<T> where T : VFXObjectBase
    {
        [Inject] protected GeneralPrefabConfigurations generalPrefabConfigurations;

        private Transform _parent;
        private GameObject _prefab;
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