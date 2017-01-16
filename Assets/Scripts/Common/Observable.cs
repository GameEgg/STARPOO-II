using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;



public class O<T>
{
    T _value;
    public T value
    {
        get
        {
            return _value;
        }
        set
        {
            _value = value;
            if(onValueChanged != null)
                onValueChanged(value);
        }
    }
    public delegate void OnValueChanged(T value);
    OnValueChanged onValueChanged;

    public O(T initialValue)
    {
        _value = initialValue;
    }

    public O()
    {
    }

    public void AddListener(OnValueChanged action)
    {
        onValueChanged += action;
    }

    public void RemoveListener(OnValueChanged action)
    {
        onValueChanged -= action;
    }
}


public class Observable<T>
{
    T _value;
    public T value {
        get
        {
            return _value;
        }
        set
        {
            _value = value;
            onValueChanged.Invoke(value);
        }
    }
    class OnValueChanged : UnityEvent<T> { }
    OnValueChanged onValueChanged = new OnValueChanged();

    public void AddListener(UnityAction<T> action)
    {
        onValueChanged.AddListener(action);
    }

    public void RemoveListener(UnityAction<T> action)
    {
        onValueChanged.RemoveListener(action);
    }

    public void RemoveAllListeners()
    {
        onValueChanged.RemoveAllListeners();
    }
}
