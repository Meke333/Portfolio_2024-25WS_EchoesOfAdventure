using Player;
using UnityEngine;

namespace Interface
{
    public interface IGrabable
    {
        public void TransmitRigidbody(Rigidbody2D body);
        public void NotifyOnRCancel(PlayerInteraction interaction);
    }
}

