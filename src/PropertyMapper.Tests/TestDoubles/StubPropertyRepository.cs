using System.Collections.Generic;

namespace PropertyMapper.Tests.TestDoubles
{
    internal class StubPropertyRepository : IPropertyRepository
    {
        private readonly IProperty[] _properties;

        public StubPropertyRepository(params IProperty[] properties)
        {
            _properties = properties;
        }

        public IEnumerable<IProperty> GetAll()
        {
            return _properties;
        }
    }
}