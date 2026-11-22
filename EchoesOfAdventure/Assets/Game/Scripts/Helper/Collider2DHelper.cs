using Helper;
using System.Collections.Generic;
using UnityEngine;

public static class Collider2DHelper
{
    public static T[] GetHitArray<T>(Transform hitbox, int layerNumber)
    {
        List<T> result = new List<T>();

        Collider2D[] hitColliders;
        hitColliders = Physics2D.OverlapBoxAll(hitbox.position, hitbox.localScale / 2, 0, LayerAndMaskHelper.ConvertNumberIntoLayer(layerNumber));

        if (hitColliders.Length <= 0)
        {
            return result.ToArray();
        }
        for (int i = 0; i < hitColliders.Length; i++)
        {
            if (hitColliders[i] == null)
                continue;

            T searchedComponent = hitColliders[i].GetComponentInChildren<T>();

            if (searchedComponent == null)
            {
                searchedComponent = hitColliders[i].GetComponentInParent<T>();
            }


            if (searchedComponent == null)
                continue;

            result.Add(searchedComponent);
        }

        return result.ToArray();
    }

    public static T[] GetHitArray<T>(Vector3 position, Vector3 scale, int layerNumber)
    {
        List<T> result = new List<T>();

        Collider2D[] hitColliders;
        hitColliders = Physics2D.OverlapBoxAll(position, scale, 0, LayerAndMaskHelper.ConvertNumberIntoLayer(layerNumber));

        if (hitColliders.Length <= 0)
        {
            return result.ToArray();
        }
        for (int i = 0; i < hitColliders.Length; i++)
        {
            if (hitColliders[i] == null)
                continue;

            T searchedComponent = hitColliders[i].GetComponentInChildren<T>();

            if (searchedComponent == null)
            {
                searchedComponent = hitColliders[i].GetComponentInParent<T>();
            }


            if (searchedComponent == null)
                continue;

            result.Add(searchedComponent);
        }

        return result.ToArray();
    }

    public static T[] GetHitArray<T>(Transform hitbox, LayerMask layerMask, bool thorough = false)
    {
        List<T> result = new List<T>();

        Collider2D[] hitColliders;
        hitColliders = Physics2D.OverlapBoxAll(hitbox.position, hitbox.localScale / 2, 0, layerMask.value);

        if (hitColliders.Length <= 0)
        {
            return result.ToArray();
        }
        for (int i = 0; i < hitColliders.Length; i++)
        {
            if (hitColliders[i] == null)
                continue;

            T searchedComponent = hitColliders[i].GetComponentInChildren<T>();

            if (searchedComponent == null)
            {
                if (thorough)
                {
                    Transform temp = hitColliders[i].transform;
                    while (searchedComponent == null && temp.parent != null)
                    {
                        searchedComponent = temp.GetComponent<T>();
                        temp = temp.parent;
                    }
                }
                else
                {
                    searchedComponent = hitColliders[i].GetComponentInParent<T>();
                }
            }

            if (searchedComponent == null)
                continue;

            result.Add(searchedComponent);
        }

        return result.ToArray();
    }

}
