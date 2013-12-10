using System;
using System.Reflection;

namespace PropertyMapper
{
    public class PropertyInfoAdapter : IProperty
    {
        private readonly PropertyInfo _propertyInfo;

        public PropertyInfoAdapter(PropertyInfo propertyInfo)
        {
            _propertyInfo = propertyInfo;
        }

        public string Name
        {
            get { return _propertyInfo.Name; }
        }

        public Type Type
        {
            get { return _propertyInfo.PropertyType; }
        }

        public object GetValue(object instance)
        {
            return _propertyInfo.GetValue(instance, null);
        }

        public void SetValue(object instance, object value)
        {
            _propertyInfo.SetValue(instance, value, null);
        }
    }
}