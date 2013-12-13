using System.Collections.Generic;

namespace PropertyMapper
{
    public class InstancePropertyRepository : IPropertyRepository
    {
        private readonly object _sourceInstance;

        public InstancePropertyRepository(object sourceInstance)
        {
            _sourceInstance = sourceInstance;
        }

        public IEnumerable<IProperty> GetAll()
        {
            return PropertyHelpers.GetAvailablePropertiesFrom(_sourceInstance);
        }
    }
}