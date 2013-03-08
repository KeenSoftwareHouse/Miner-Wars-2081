
namespace KeenSoftwareHouse.Library.Cloning
{
#if !NET20
    
    /// <summary>
    /// Extension method for DeepClone&lt;T>() on every object.
    /// </summary>
    public static class ClonableExtensions
    {
        /// <summary>
        /// Provides a deep-clone of objects using either serialization routines if available
        /// or memberwize cloning when serialization is not supported.
        /// See also: new SerializerClone().Clone&lt;T>(instance)
        /// </summary>
        public static T DeepClone<T>(this T instance)
        {
            return new SerializerClone().Clone<T>(instance);
        }
    }
#endif
}
