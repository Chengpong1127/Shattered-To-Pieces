using System.Collections.Generic;
using AbilitySystem.Authoring;
using AttributeSystem.Authoring;
using UnityEngine;

namespace AttributeSystem.Components
{
    public abstract class AbstractAttributeEventHandler : ScriptableObject
    {
        public abstract void AttributeChangedHandler(AttributeSystemComponent AttributeSystemComponent, AttributeScriptableObject attribute, AttributeValue prevAttributeValue, AttributeValue currentAttributeValue);
    }
}
