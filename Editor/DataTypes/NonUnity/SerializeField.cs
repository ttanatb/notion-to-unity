#if UNITY_EDITOR
#else
using System;

namespace NotionToUnity.DataTypes.NonUnity
{
    /// <summary>
    /// A workaround to make sure SerializeField is defined in non-Unity systems.
    /// </summary>
    public sealed class SerializeField : Attribute
    {
        
    }
}
#endif
