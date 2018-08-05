using System.Data.Entity.ModelConfiguration;


namespace Data.Mapping
{
    public abstract class TSVarlıkTipiYapılandırması<T> : EntityTypeConfiguration<T> where T : class
    {
        protected TSVarlıkTipiYapılandırması()
        {
            PostInitialize();
        }
        protected virtual void PostInitialize()
        {

        }
    }
}
