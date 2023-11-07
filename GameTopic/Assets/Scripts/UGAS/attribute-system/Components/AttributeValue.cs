using System;
using AttributeSystem.Authoring;

namespace AttributeSystem.Components
{
    [Serializable]
    public class AttributeValue
    {
        public AttributeScriptableObject Attribute;
        public float BaseValue;
        public float CurrentValue;
        public AttributeModifier Modifier;

        public void ResetModifier()
        {
            Modifier = new AttributeModifier()
            {
                Add = 0f,
                Multiply = 0f,
                Override = 0f
            };
        }
        public AttributeValue Clone()
        {
            return new AttributeValue()
            {
                Attribute = Attribute,
                BaseValue = BaseValue,
                CurrentValue = CurrentValue,
                Modifier = Modifier.Clone()
            };
        }
    }

    [Serializable]
    public class AttributeModifier
    {
        public float Add;
        public float Multiply;
        public float Override;
        public AttributeModifier Combine(AttributeModifier other)
        {
            other.Add += Add;
            other.Multiply += Multiply;
            other.Override = Override;
            return other;
        }
        public AttributeModifier Clone()
        {
            return new AttributeModifier()
            {
                Add = Add,
                Multiply = Multiply,
                Override = Override
            };
        }
    }

}
