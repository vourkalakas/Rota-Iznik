using Core.Domain.Test;

namespace Data.Mapping.Teklifler
{
    public class TestMap : TSVarlıkTipiYapılandırması<Test>
    {
        public TestMap()
        {
            this.ToTable("Test");
            this.HasKey(t => t.Id);
        }
    }
}
