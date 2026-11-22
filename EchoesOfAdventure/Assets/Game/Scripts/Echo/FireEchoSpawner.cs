using NUnit.Framework;
using ObjectPool;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Echo
{
    public class FireEchoSpawner : EchoSpawnerBase<FireEcho>
    {
        protected override void Awake()
        {
            base.Awake();
            SetValues(transform, generalPrefabConfigurations.FireEcho, _objectAmount);
        }
    }

}




