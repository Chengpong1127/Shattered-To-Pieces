using System.Collections.Generic;
using AttributeSystem.Authoring;
using UnityEngine;
using System.Linq;
using System;
using Unity.Netcode;
using Cysharp.Threading.Tasks;

namespace AttributeSystem.Components
{

    /// <summary>
    /// Manages the attributes for a game character
    /// </summary>
    public class AttributeSystemComponent : MonoBehaviour
    {
        [SerializeField]
        private List<AbstractAttributeEventHandler> AttributeSystemEvents;

        /// <summary>
        /// Attribute sets assigned to the game character
        /// </summary>
        [SerializeField]
        private List<AttributeScriptableObject> Attributes;

        private Dictionary<AttributeScriptableObject, AttributeValue> AttributeDictionary = new();

        public Dictionary<AttributeScriptableObject, AttributeValue> GetAttributeDictionaryCopy()
        {
            return AttributeDictionary.ToDictionary(entry => entry.Key, entry => entry.Value);
        }

        /// <summary>
        /// Gets the value of an attribute.  Note that the returned value is a copy of the struct, so modifying it
        /// does not modify the original attribute
        /// </summary>
        /// <param name="attribute">Attribute to get value for</param>
        /// <param name="value">Returned attribute</param>
        /// <returns>True if attribute was found, false otherwise.</returns>
        public bool GetAttributeValue(AttributeScriptableObject attribute, out AttributeValue value)
        {
            return AttributeDictionary.TryGetValue(attribute, out value);
        }



        public void SetAttributeBaseValue(AttributeScriptableObject attribute, float value)
        {
            if (AttributeDictionary.TryGetValue(attribute, out var attributeValue))
            {
                var oldValue = attributeValue.Clone();
                attributeValue.BaseValue = value;
                AttributeDictionary[attribute] = attributeValue;
                UpdateCurrentValue(attribute);
                TriggerChangedEvents(attribute, oldValue, attributeValue);
            }else{
                Debug.LogWarning($"Attribute {attribute} not found in AttributeDictionary");
            }
        }

        public void SetAttributeValue(AttributeScriptableObject attribute, AttributeValue value)
        {
            if (AttributeDictionary.ContainsKey(attribute))
            {
                var oldValue = AttributeDictionary[attribute].Clone();
                AttributeDictionary[attribute] = value;
                UpdateCurrentValue(attribute);
                TriggerChangedEvents(attribute, oldValue, value);
            }else{
                Debug.LogWarning($"Attribute {attribute} not found in AttributeDictionary");
            }
        }

        /// <summary>
        /// Sets value of an attribute.  Note that the out value is a copy of the struct, so modifying it
        /// does not modify the original attribute
        /// </summary>
        /// <param name="attribute">Attribute to set</param>
        /// <param name="modifierType">How to modify the attribute</param>
        /// <param name="value">Copy of newly modified attribute</param>
        /// <returns>True, if attribute was found.</returns>
        public bool UpdateAttributeModifiers(AttributeScriptableObject attribute, AttributeModifier modifier, out AttributeValue value)
        {
            if (AttributeDictionary.TryGetValue(attribute, out value))
            {
                var oldValue = value.Clone();
                value.Modifier = value.Modifier.Combine(modifier);
                AttributeDictionary[attribute] = value;
                UpdateCurrentValue(attribute);
                TriggerChangedEvents(attribute, oldValue, value);
                return true;
            }
            return false;
        }

        public void ResetAttributeModifiers()
        {
            AttributeDictionary.Values.ToList().ForEach(v => v.ResetModifier());
        }
        public void ResetAttributeModifiers(AttributeScriptableObject attributeScriptableObject)
        {
            AttributeDictionary[attributeScriptableObject].ResetModifier();
        }

        private void InitialiseAttributeValues(List<AttributeScriptableObject> attributes)
        {
            AttributeDictionary.Clear();
            attributes.ForEach(a => AttributeDictionary.Add(a, GetDefaultAttributeValue(a)));
        }
        private AttributeValue GetDefaultAttributeValue(AttributeScriptableObject attribute){
            return new AttributeValue()
            {
                Attribute = attribute,
                Modifier = new AttributeModifier()
                {
                    Add = 0f,
                    Multiply = 0f,
                    Override = 0f
                }
            };
        }
        private void TriggerChangedEvents(AttributeScriptableObject attribute, AttributeValue oldValue, AttributeValue newValue)
        {
            if (oldValue.BaseValue != newValue.BaseValue || oldValue.CurrentValue != newValue.CurrentValue){
                AttributeSystemEvents.ForEach(e => e.AttributeChangedHandler(this, attribute, oldValue, newValue));
            }
        }
        private void UpdateCurrentValue(AttributeScriptableObject attribute){
            if (AttributeDictionary.TryGetValue(attribute, out var attributeValue))
            {
                AttributeDictionary[attribute] = attribute.CalculateCurrentAttributeValue(attributeValue);
            }else{
                Debug.LogWarning($"Attribute {attribute} not found in AttributeDictionary");
            }
        }
        private void Awake()
        {
            InitialiseAttributeValues(Attributes);
        }
    }
}
