using System;
using System.Collections.Generic;
using System.Text;

namespace CustomAutoMapper
{
    public class MapperConfiguration
    {
        public Mapper Mapper { get; private set; }

        public MapperConfiguration CreateMap()
        {
            this.Mapper = new Mapper();
            return this;
        }
    }
}
