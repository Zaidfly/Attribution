namespace Attribution.UserActionService.Tests.Builders
{
    public struct OptionValue<T>
    {
        private T _value;

        public bool HasValue { get; private set; }

        public T OrDefault() => HasValue ? _value : default;
        
        public T OrValue(T newValue) => HasValue ? _value : newValue;

        public TBuilder WithValue<TBuilder>(T value, TBuilder builder)
        {
            HasValue = true;
            _value = value;
            return builder;
        }

        public static implicit operator T(OptionValue<T> value) => value._value;
        public static implicit operator OptionValue<T>(T value) => new OptionValue<T> {_value = value};
    }
}