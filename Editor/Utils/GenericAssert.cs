// TODO: make this not required (figure out how to gate it behind #if #endif)
using JetBrains.Annotations;
using UnityEngine.Assertions;

namespace NotionToUnity.Utils
{
    /// <summary>
    /// Abstract wrapper for asserting.
    /// 
    /// Asserter? I barely even know 'er!
    /// </summary>
    public static class Asserter
    {
        // Attributes are used for rider
        [AssertionMethod] [ContractAnnotation("value:null => halt")]
        public static void IsNotNull(object value)
        {
            #if UNITY_EDITOR
            Assert.IsNotNull(value);
            #endif
        }
        
        // Attributes are used for rider
        [AssertionMethod] [ContractAnnotation("condition:false => halt")]
        public static void IsTrue(bool condition)
        {
            #if UNITY_EDITOR
            Assert.IsTrue(condition);
            #endif
        }
        
        // Attributes are used for rider
        [AssertionMethod]
        public static void AreEqual(object expected, object actual)
        {
            #if UNITY_EDITOR
            Assert.AreEqual(expected, actual);
            #endif
        }
    }
}
