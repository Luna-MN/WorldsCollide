using Godot;
using System;

[Tool]
public partial class MyNode : Node
{
    private int _numberCount;

    [Export]
    public Stats jeff;
    [Export]
    public int NumberCount
    {
        get => _numberCount;
        set
        {
            _numberCount = value;
            _numbers.Resize(_numberCount);
            NotifyPropertyListChanged();
        }
    }

    private Godot.Collections.Array<int> _numbers = [];

    public override Godot.Collections.Array<Godot.Collections.Dictionary> _GetPropertyList()
    {
        Godot.Collections.Array<Godot.Collections.Dictionary> properties = [];

        // foreach (var pair in jeff.stats) 
        for (int i = 0; i < _numberCount; i++)
        {
            properties.Add(new Godot.Collections.Dictionary()
            {
                { "name", $"number_{i}" },
                { "type", (int)Variant.Type.Int },
                { "hint",  (int)PropertyHint.Enum},
                { "hint_string", "Zero,One,Two,Three,Four,Five" },
            });
        }

        return properties;
    }

    public override Variant _Get(StringName property)
    {
        string propertyName = property.ToString();
        if (propertyName.StartsWith("number_"))
        {
            int index = int.Parse(propertyName.Substring("number_".Length));
            return _numbers[index];
        }

        return default;
    }

    public override bool _Set(StringName property, Variant value)
    {
        string propertyName = property.ToString();
        if (propertyName.StartsWith("number_"))
        {
            int index = int.Parse(propertyName.Substring("number_".Length));
            _numbers[index] = value.As<int>();
            return true;
        }

        return false;
    }
}