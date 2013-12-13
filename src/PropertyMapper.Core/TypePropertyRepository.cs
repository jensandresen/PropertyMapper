using System;
using System.Collections.Generic;

namespace PropertyMapper
{
    public class TypePropertyRepository : IPropertyRepository
    {
        private readonly Type _sourceType;

        public TypePropertyRepository(Type sourceType)
        {
            _sourceType = sourceType;
        }

        public IEnumerable<IProperty> GetAll()
        {
            return PropertyHelpers.GetAvailablePropertiesFrom(_sourceType);
        }
    }
}