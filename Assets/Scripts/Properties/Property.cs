using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Property
{
    [SerializeField]
    private float _baseValue;
    public float baseValue { get { return _baseValue; } private set { } }

    private bool isDirty;
    private List<PropertyModifier> modifiers;

    private float _value;
    public float value
    {
        get
        {
            if(isDirty)
            {
                CalculateValue();
                isDirty = false;
                return _value;
            }
            return _value;
        }
    }

    public Property(float Base)
    {
        _baseValue = Base;
        _value = _baseValue;
        isDirty = false;
        modifiers = new List<PropertyModifier>();
    }

    private void CalculateValue()
    {
        _value = _baseValue;
        int len = modifiers.Count;

        for(int i = 0; i < len; i++)
        {
            if (modifiers[i].modType == PropertyModType.PercentageMul)
                _value *= 1 + modifiers[i].value;
            else if (modifiers[i].modType == PropertyModType.PercentageAdd)
                _value += _baseValue * modifiers[i].value;
            else if (modifiers[i].modType == PropertyModType.Flat)
                _value += modifiers[i].value;
        }

        _value = (float)System.Math.Round(_value, 4);  //去除浮點數誤差
    }

    public void AddModifier(PropertyModifier modifier)
    {
        isDirty = true;
        modifiers.Add(modifier);
        modifiers.Sort(SortModifier);
    }

    public bool RemoveModifier(PropertyModifier modifier)
    {
        if(modifiers.Contains(modifier))
        {
            isDirty = true;
            modifiers.Remove(modifier);
            return true;
        }
        return false;
    }

    public bool RemoveAllModifiersFromSource(Object source)
    {
        bool remove = false;
        int len = modifiers.Count - 1;
        for(int i = len; i >= 0; i --)
        {
            if(modifiers[i].source == source)
            {
                modifiers.RemoveAt(i);
                isDirty = true;
                remove = true;
            }
        }
        return remove;
    }

    private int SortModifier(PropertyModifier a, PropertyModifier b)
    {
        if (a.order < b.order)
            return -1;
        else if (a.order > b.order)
            return 1;
        return 0;
    }
}

public enum PropertyModType
{
    PercentageMul = 1000,
    PercentageAdd = 2000,
    Flat = 3000
}

public class PropertyModifier
{
    private float _value;
    public float value { get { return _value; } private set { } }

    private PropertyModType _modType;
    public PropertyModType modType { get { return _modType; } private set { } }

    private int _order;
    public int order { get { return _order; } private set { } }

    private Object _source;
    public Object source { get { return _source; } private set { } }

    public PropertyModifier(float Value, PropertyModType ModType, int Order = -1, Object Source = null)
    {
        _value = Value;
        _modType = ModType;
        _order = (Order < 0) ? (int)_modType : Order;
        _source = Source;
    }
}


