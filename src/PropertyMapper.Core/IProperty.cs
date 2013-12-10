using System;

namespace PropertyMapper
{
    public interface IProperty
    {
        string Name { get; }
        Type Type { get; }

        object GetValue(object instance);
        void SetValue(object instance, object value);
    }
}