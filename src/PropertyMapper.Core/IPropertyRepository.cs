using System.Collections.Generic;

namespace PropertyMapper
{
    public interface IPropertyRepository
    {
        IEnumerable<IProperty> GetAll();
    }
}