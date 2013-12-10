using System;

namespace PropertyMapper.Tests.TestDoubles
{
    public class FakeProperty<T> : IProperty
    {
        private readonly string _name;

        public FakeProperty(string name)
        {
            _name = name;
        }

        public string Name
        {
            get { return _name; }
        }

        public Type Type
        {
            get { return typeof(T); }
        }

        public object GetValue(object instance)
        {
            throw new NotImplementedException();
        }

        public void SetValue(object instance, object value)
        {
            throw new NotImplementedException();
        }
    }
}