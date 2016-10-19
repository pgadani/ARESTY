using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using NPC;

namespace SupportUtils {

    public class NPCUtils {
        public static Vector3 CalculateObjectNormal(IPerceivable object1, IPerceivable object2) {
            Vector3 basicNormal = object2.GetPosition() - object1.GetPosition();
            if(Mathf.Abs(basicNormal.x) > Mathf.Abs(basicNormal.z)){
                // west or east
                return object2.GetPosition().x - object1.GetPosition().x <= 0 ?
                    new Vector3(1f, 0f, 0f) : new Vector3(-1f, 0f, 0f);
            } else {
                // north or south
                return object2.GetPosition().z - object1.GetPosition().z <= 0 ?
                    new Vector3(1f, 0f, -1f) : new Vector3(0f, 0f, 1f);
            }
        }
    }

    /// <summary>
    /// Comparer for comparing two keys, handling equality as beeing greater
    /// Use this Comparer e.g. with SortedLists or SortedDictionaries, that don't allow duplicate keys
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public class DuplicateKeyComparer<TKey>
                    :
                 IComparer<TKey> where TKey : IComparable {

        #region IComparer<TKey> Members
        public int Compare(TKey x, TKey y) {
            int result = x.CompareTo(y);

            if (result == 0)
                return 1;   // Handle equality as beeing greater
            else
                return result;
        }
        #endregion
    }

}