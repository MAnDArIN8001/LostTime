using System;

namespace UI.Runtime
{
    public struct PanelId : IEquatable<PanelId>
    {
        public string Value { get; private set; }

        public PanelId(string value)
        {
            Value = value ?? string.Empty;
        }

        public static PanelId From<TPanel>()
        {
            return From(typeof(TPanel));
        }

        public static PanelId From(Type panelType)
        {
            return new PanelId(panelType.FullName ?? panelType.Name);
        }

        public bool Equals(PanelId other)
        {
            return string.Equals(Value, other.Value, StringComparison.Ordinal);
        }

        public override bool Equals(object obj)
        {
            return obj is PanelId other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Value == null ? 0 : StringComparer.Ordinal.GetHashCode(Value);
        }

        public override string ToString()
        {
            return Value;
        }

        public static bool operator ==(PanelId left, PanelId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(PanelId left, PanelId right)
        {
            return !left.Equals(right);
        }
    }
}
