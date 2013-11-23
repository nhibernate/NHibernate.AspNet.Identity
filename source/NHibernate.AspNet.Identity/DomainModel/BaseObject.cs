namespace NHibernate.AspNet.Identity.DomainModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    ///     Provides a standard base class for facilitating comparison of objects.
    /// </summary>
    /// <remarks>
    ///     For a discussion of the implementation of Equals/GetHashCode, see
    ///     http://devlicio.us/blogs/billy_mccafferty/archive/2007/04/25/using-equals-gethashcode-effectively.aspx
    ///     and http://groups.google.com/group/sharp-architecture/browse_thread/thread/f76d1678e68e3ece?hl=en for 
    ///     an in depth and conclusive resolution.
    /// </remarks>
    [Serializable]
    public abstract class BaseObject
    {
        /// <summary>
        ///     To help ensure hash code uniqueness, a carefully selected random number multiplier 
        ///     is used within the calculation. Goodrich and Tamassia's Data Structures and
        ///     Algorithms in Java asserts that 31, 33, 37, 39 and 41 will produce the fewest number
        ///     of collissions. See http://computinglife.wordpress.com/2008/11/20/why-do-hash-functions-use-prime-numbers/
        ///     for more information.
        /// </summary>
        private const int HashMultiplier = 31;

        /// <summary>
        ///     This static member caches the domain signature properties to avoid looking them up for 
        ///     each instance of the same type.
        /// </summary>
        /// <remarks>
        ///     A description of the very slick ThreadStatic attribute may be found at 
        ///     http://www.dotnetjunkies.com/WebLog/chris.taylor/archive/2005/08/18/132026.aspx
        /// </remarks>
        [ThreadStatic]
        private static Dictionary<Type, IEnumerable<PropertyInfo>> signaturePropertiesDictionary;

        /// <summary>
        ///     Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="Object" /> to compare with the current <see cref="Object" />.</param>
        /// <returns><c>true</c> if the specified <see cref="Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            var compareTo = obj as BaseObject;

            if (ReferenceEquals(this, compareTo))
            {
                return true;
            }

            return compareTo != null && this.GetType().Equals(compareTo.GetTypeUnproxied()) &&
                   this.HasSameObjectSignatureAs(compareTo);
        }

        /// <summary>
        ///     Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        /// <remarks>
        ///     This is used to provide the hash code identifier of an object using the signature
        ///     properties of the object; although it's necessary for NHibernate's use, this can
        ///     also be useful for business logic purposes and has been included in this base
        ///     class, accordingly. Since it is recommended that GetHashCode change infrequently,
        ///     if at all, in an object's lifetime, it's important that properties are carefully
        ///     selected which truly represent the signature of an object.
        /// </remarks>
        public override int GetHashCode()
        {
            unchecked
            {
                var signatureProperties = this.GetSignatureProperties();

                // It's possible for two objects to return the same hash code based on 
                // identically valued properties, even if they're of two different types, 
                // so we include the object's type in the hash calculation
                var hashCode = this.GetType().GetHashCode();

                hashCode = signatureProperties.Select(property => property.GetValue(this, null))
                                              .Where(value => value != null)
                                              .Aggregate(hashCode, (current, value) => (current * HashMultiplier) ^ value.GetHashCode());

                if (signatureProperties.Any())
                {
                    return hashCode;
                }

                // If no properties were flagged as being part of the signature of the object,
                // then simply return the hash code of the base object as the hash code.
                return base.GetHashCode();
            }
        }

        /// <summary>
        ///     Returns the properties of the current object that make up the object's signature.
        /// </summary>
        /// <returns>A collection of <see cref="PropertyInfo"/> instances.</returns>
        public virtual IEnumerable<PropertyInfo> GetSignatureProperties()
        {
            IEnumerable<PropertyInfo> properties;

            // Init the signaturePropertiesDictionary here due to reasons described at 
            // http://blogs.msdn.com/jfoscoding/archive/2006/07/18/670497.aspx
            if (signaturePropertiesDictionary == null)
            {
                signaturePropertiesDictionary = new Dictionary<Type, IEnumerable<PropertyInfo>>();
            }

            if (signaturePropertiesDictionary.TryGetValue(this.GetType(), out properties))
            {
                return properties;
            }

            return signaturePropertiesDictionary[this.GetType()] = this.GetTypeSpecificSignatureProperties();
        }

        /// <summary>
        ///     Determines whether the current object has the same object signature as the specified object.
        /// </summary>
        /// <param name="compareTo">The object to compare to.</param>
        /// <returns><c>true</c> if the current object has the same object signature as the specified object; otherwise, <c>false</c>.</returns>
        /// <remarks>You may override this method to provide your own comparison routine.</remarks>
        public virtual bool HasSameObjectSignatureAs(BaseObject compareTo)
        {
            var signatureProperties = this.GetSignatureProperties();

            if ((from property in signatureProperties
                 let valueOfThisObject = property.GetValue(this, null)
                 let valueToCompareTo = property.GetValue(compareTo, null)
                 where valueOfThisObject != null || valueToCompareTo != null
                 where (valueOfThisObject == null ^ valueToCompareTo == null) || (!valueOfThisObject.Equals(valueToCompareTo))
                 select valueOfThisObject).Any())
            {
                return false;
            }

            // If we've gotten this far and signature properties were found, then we can
            // assume that everything matched; otherwise, if there were no signature 
            // properties, then simply return the default behavior of Equals.
            return signatureProperties.Any() || base.Equals(compareTo);
        }

        /// <summary>
        ///     Enforces the template method pattern to have child objects determine which specific 
        ///     properties should and should not be included in the object signature comparison.
        /// </summary>
        /// <remarks>
        ///     Note that the the BaseObject already takes care of performance caching, so this
        ///     method shouldn't worry about caching...just return the goods man!
        /// </remarks>
        protected abstract IEnumerable<PropertyInfo> GetTypeSpecificSignatureProperties();

        /// <summary>
        ///     Returns the unproxied type of the current object.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         When NHibernate proxies objects, it masks the type of the actual entity object.
        ///         This wrapper burrows into the proxied object to get its actual type.
        ///     </para>
        ///     <para>
        ///         Although this assumes NHibernate is being used, it doesn't require any NHibernate
        ///         related dependencies and has no bad side effects if NHibernate isn't being used.
        ///     </para>
        ///     <para>
        ///         Related discussion is at http://groups.google.com/group/sharp-architecture/browse_thread/thread/ddd05f9baede023a ...thanks Jay Oliver!
        ///     </para>
        /// </remarks>
        protected virtual Type GetTypeUnproxied()
        {
            return this.GetType();
        }
    }
}