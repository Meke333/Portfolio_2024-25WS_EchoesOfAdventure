using System;

namespace Player
{
    [Flags]
    public enum CharacterMask
    {
        Fire = 1 << 1,
        Stone = 1 << 2,
        Plant = 1 << 3,
    }
}

