using System;
using System.Collections.Generic;
using AutoMapper;

namespace Core.Altyapı.Mapper
{
    public static class AutoMapperAyarları
    {
        public static IMapper Mapper { get; private set; }
        public static MapperConfiguration MapperConfiguration { get; private set; }
        public static void Init(MapperConfiguration config)
        {
            MapperConfiguration = config;
            Mapper = config.CreateMapper();
        }

    }
}
