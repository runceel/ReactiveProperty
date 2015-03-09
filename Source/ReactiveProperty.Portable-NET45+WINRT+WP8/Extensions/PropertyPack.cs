using System;
using System.Reflection;



namespace Reactive.Bindings.Extensions
{
    /// <summary>
    /// Represents property and instance package.
    /// </summary>
    /// <typeparam name="TInstance">Type of instance</typeparam>
    /// <typeparam name="TValue">Type of property value</typeparam>
    public class PropertyPack<TInstance, TValue>
    {
        #region Properies
        /// <summary>
        /// Gets or sets instance which has property.
        /// </summary>
        public TInstance Instance { get; private set; }


        /// <summary>
        /// Gets or sets target property info.
        /// </summary>
        public PropertyInfo Property { get; private set; }


        /// <summary>
        /// Gets or sets target property value.
        /// </summary>
        public TValue Value { get; private set; }
        #endregion


        #region Constructor
        /// <summary>
        /// Create instance.
        /// </summary>
        /// <param name="instance">Target instance</param>
        /// <param name="property">Target property info</param>
        /// <param name="value">Property value</param>
        public PropertyPack(TInstance instance, PropertyInfo property, TValue value)
        {
            if (instance == null) throw new ArgumentNullException("instance");
            if (property == null) throw new ArgumentNullException("property");

            this.Instance = instance;
            this.Property = property;
            this.Value    = value;
        }
        #endregion
    }



    /// <summary>
    /// Provides PropertyPack static members.
    /// </summary>
    public static class PropertyPack
    {
        /// <summary>
        /// Create instance.
        /// </summary>
        /// <param name="instance">Target instance</param>
        /// <param name="property">Target property info</param>
        /// <param name="value">Property value</param>
        /// <returns>Created instance</returns>
        public static PropertyPack<TInstance, TValue> Create<TInstance, TValue>(TInstance instance, PropertyInfo property, TValue value)
        {
            return new PropertyPack<TInstance,TValue>(instance, property, value);
        }
    }
}