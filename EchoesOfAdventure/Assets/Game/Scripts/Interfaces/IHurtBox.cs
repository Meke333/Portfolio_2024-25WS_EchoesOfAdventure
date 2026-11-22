
using DependencyInjection;
using Player;
using System;

namespace Interface
{
    public interface IHurtBox
    {
        public bool CanGetHurt<T>(T layers) where T : Enum;
        public void GetDamage();
    }

}

