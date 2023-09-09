using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameplayTagNamespace.Authoring
{
    [CreateAssetMenu(menuName = "Gameplay Ability System/Tag")]
    public class GameplayTag : ScriptableObject
    {
        [SerializeField]
        public string TagName;
        [SerializeField]
        private GameplayTag _parent;
        public GameplayTag Parent { get { return _parent; } }


        /// <summary>
        /// <para>Check is this gameplay tag is a descendant of another gameplay tag.</para>
        /// By default, only 4 levels of ancestors are searched.
        /// </summary>
        /// <param name="other">Ancestor gameplay tag</param>
        /// <returns>True if this gameplay tag is a descendant of the other gameplay tag</returns>
        public bool IsDescendantOf(GameplayTag other, int nSearchLimit = 4)
        {
            int i = 0;
            GameplayTag tag = Parent;
            while (nSearchLimit > i++)
            {
                // tag will be invalid once we are at the root ancestor
                if (!tag) return false;

                // Match found, so we can return true
                if (tag == other) return true;

                // No match found, so try again with the next ancestor
                tag = tag.Parent;
            }

            // If we've exhausted the search limit, no ancestor was found
            return false;
        }

    }
}
