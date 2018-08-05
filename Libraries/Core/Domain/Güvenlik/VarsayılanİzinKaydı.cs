using System.Collections.Generic;


namespace Core.Domain.Güvenlik
{
    public class VarsayılanİzinKaydı
    {
        public VarsayılanİzinKaydı()
        {
            this.İzinKayıtları = new List<İzinKaydı>();
        }
        public string KullanıcıRolüSistemAdı { get; set; }
        public IEnumerable<İzinKaydı> İzinKayıtları { get; set; }
    }
}
