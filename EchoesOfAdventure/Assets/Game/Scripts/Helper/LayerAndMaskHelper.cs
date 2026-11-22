using System;
using UnityEngine;

namespace Helper
{
    public static class LayerAndMaskHelper
    {
        public static int ConvertNumberIntoLayer(int layerNumber)
        {
            LayerMask layer = 0;
            layer |= 0x1 << layerNumber;

            return layer.value;
        }

        public static T GetMaskFromType<T,U>(U type) where T : Enum where U : Enum
        {
            return (T)(object)(int)(0x1 << (int)(object)type);
        }

        public static bool IsTypeInMask<T, U>(T mask, U type) where T : Enum where U : Enum
        {
            T typeIntoMask = (T)(object)(int)(0x1 << (int)(object)type);

            return typeIntoMask.HasFlag(mask);
        }

    }

}
