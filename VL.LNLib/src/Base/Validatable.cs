namespace VL.LNLib.Base
{
    public interface IValidatable
    {
        IReadOnlyList<object>? Errors { get; }
        bool IsValid { get; }
        void Validate();
    }

    public abstract record Validatable
    {
        public IReadOnlyList<object>? Errors { get; } = [];
        public bool IsValid => Errors == null || Errors.Count == 0;

        private readonly List<object> _errors = new();

        public abstract void Validate();

        protected void VALIDATE_ARGUMENT(bool condition, string name, object message)
        {
            if (!condition)
            {
                _errors.Add($"{name}: {message}");
            }
        }
    }
}
