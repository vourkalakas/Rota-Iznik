using System;
using System.Data.Entity.Core.Objects;
using Core;

namespace Data
{
    public static class Extensions
    {
        public static Type GetUnproxiedEntityType(this TemelVarlık varlık)
        {
            var KullanıcıTipi = System.Data.Entity.Core.Objects.ObjectContext.GetObjectType(varlık.GetType());
            return KullanıcıTipi;
        }
    }
}
