namespace VL.LNLib.Helpers
{
    public class CachedValue<T>
        where T : struct
    {
        private T _value;
        public T Value
        {
            get => _value;
            set
            {
                if (!EqualityComparer<T>.Default.Equals(_value, value))
                {
                    _value = value;

                    _onValueChanged?.Invoke(_value);
                }
            }
        }

        private readonly Action<T> _onValueChanged;

        public CachedValue(T value, Action<T> onValueChanged)
        {
            _value = value;
            _onValueChanged = onValueChanged;
        }
    }
}
