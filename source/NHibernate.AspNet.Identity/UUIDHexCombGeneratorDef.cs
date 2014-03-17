using System;
using NHibernate.Mapping.ByCode;

namespace NHibernate.AspNet.Identity
{
    public class UUIDHexCombGeneratorDef : IGeneratorDef
    {
        private readonly object param;

        public UUIDHexCombGeneratorDef(string format)
        {
            if (format == null)
                throw new ArgumentNullException("format");

            param = new { format = format };
        }

        #region Implementation of IGeneratorDef

        public string Class
        {
            get { return "uuid.hex"; }
        }

        public object Params
        {
            get { return param; }
        }

        public System.Type DefaultReturnType
        {
            get { return typeof(string); }
        }

        public bool SupportedAsCollectionElementId
        {
            get { return false; }
        }

        #endregion
    }

}
