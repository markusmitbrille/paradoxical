using Paradoxical.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Paradoxical.Core;

public class Tag : IEquatable<Tag?>
{
    public string Name { get; }
    public string Value { get; }

    public Tag(string name, string value)
    {
        Name = name;
        Value = value;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Tag);
    }

    public bool Equals(Tag? other)
    {
        return other is not null &&
               Name == other.Name &&
               Value == other.Value;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Value);
    }

    public static bool operator ==(Tag? left, Tag? right)
    {
        return EqualityComparer<Tag>.Default.Equals(left, right);
    }

    public static bool operator !=(Tag? left, Tag? right)
    {
        return !(left == right);
    }
}
