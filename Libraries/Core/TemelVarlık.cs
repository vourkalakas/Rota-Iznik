using System;

namespace Core
{
    public abstract partial class TemelVarlık
    {
        public int Id { get; set; }
        public override bool Equals(object obj)
        {
            return Equals(obj as TemelVarlık);
        }
        private static bool IsTransient(TemelVarlık obj)
        {
            return obj != null && Equals(obj.Id, default(int));
        }
        private Type GetUnproxiedType()
        {
            return GetType();
        }
        public virtual bool Equals(TemelVarlık other)
        {
            if (other == null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (!IsTransient(this) &&
                !IsTransient(other) &&
                Equals(Id, other.Id))
            {
                var otherType = other.GetUnproxiedType();
                var thisType = GetUnproxiedType();
                return thisType.IsAssignableFrom(otherType) ||
                        otherType.IsAssignableFrom(thisType);
            }
            return false;
        }
        public override int GetHashCode()
        {
            if (Equals(Id, default(int)))
                return base.GetHashCode();
            return Id.GetHashCode();
        }
        public static bool operator ==(TemelVarlık x, TemelVarlık y)
        {
            return Equals(x, y);
        }

        public static bool operator !=(TemelVarlık x, TemelVarlık y)
        {
            return !(x == y);
        }
    }
}
