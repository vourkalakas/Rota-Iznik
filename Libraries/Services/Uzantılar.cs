using Core;
using Core.Altyapı;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
//using System.Web.Mvc;

namespace Services
{
    public static class Uzantılar
    {
        public static SelectList ToSelectList<TEnum>(this TEnum enumObj,
          bool markCurrentAsSelected = true, int[] valuesToExclude = null, bool useLocalization = true) where TEnum : struct
        {
            if (!typeof(TEnum).IsEnum) throw new ArgumentException("Sıralanacak tip gereklidir.", "enumObj");

            var workContext = EngineContext.Current.Resolve<IWorkContext>();

            var values = from TEnum enumValue in Enum.GetValues(typeof(TEnum))
                         where valuesToExclude == null || !valuesToExclude.Contains(Convert.ToInt32(enumValue))
                         select new { ID = Convert.ToInt32(enumValue), Name = useLocalization ? enumValue.ToString() : GenelYardımcı.EnumDönüştür(enumValue.ToString()) };
            object selectedValue = null;
            if (markCurrentAsSelected)
                selectedValue = Convert.ToInt32(enumObj);
            return new SelectList(values, "ID", "Name", selectedValue);
        }
        public static SelectList ToSelectList<T>(this T objList, Func<TemelVarlık, string> selector) where T : IEnumerable<TemelVarlık>
        {
            return new SelectList(objList.Select(p => new { ID = p.Id, Name = selector(p) }), "ID", "Name");
        }
    }
}
