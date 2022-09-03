using System;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
    public static bool IsDefault(this Entity.State state) => state == Entity.State.Default;

    public static bool IsDead(this Entity.State state) => state == Entity.State.Dead;

    public static float Pow2(this float value) => Mathf.Pow(value, 2);

    public static Vector3 Flat(this Vector3 vector) => new Vector3(vector.x, 0.0f, vector.z);

    public static T RandomElement<T>(this IList<T> collection) => collection.Count > 0 ? collection[UnityEngine.Random.Range(0, collection.Count)] : default;

    public static bool TryFind<T>(this List<T> list, Predicate<T> match, out T element)
    {
        element = list.Find(match);
        return element != null;
    }
}
