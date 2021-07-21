using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UltEvents;

[System.Serializable]
public class GameEvent : UnityEvent { }
[System.Serializable]
public class IntEvent : UnityEvent<int> { }
[System.Serializable]
public class FloatEvent : UnityEvent<float> { }
[System.Serializable]
public class BoolEvent : UnityEvent<bool> { }
[System.Serializable]
public class GameObjectEvent : UnityEvent<GameObject> { }
[System.Serializable]
public class Vector2Event : UnityEvent<Vector2> { }
[System.Serializable]
public class Vector3Event : UnityEvent<Vector3> { }
[System.Serializable]
public class ColorEvent : UnityEvent<Color> { }
[System.Serializable]
public class ObjectEvent<T> : UnityEvent<T> { } 
[System.Serializable]
public class UltIntEvent : UltEvent<int> { }
[System.Serializable]
public class UltFloatEvent : UltEvent<float> { }

[System.Serializable]
public class UltBoolEvent : UltEvent<bool> { }

[System.Serializable]
public class UltGameObjectEvent : UltEvent<GameObject> { }

[System.Serializable]
public class UltVector2Event : UltEvent<Vector2> { }

[System.Serializable]
public class UltVector3Event : UltEvent<Vector3> { }

[System.Serializable]
public class UltColorEvent : UltEvent<Color> { }

public class AsyncEvent
{
    private List<Action> actions = new List<Action>();
    private List<IEnumerator> enumerators = new List<IEnumerator>();
    public UnityEvent OnEventComplete { get; } = new UnityEvent();

    public void AddAction(Action action) { actions.Add(action); }
    public void ClearAllActions() { actions.Clear(); }
    public void AddEnumerator(IEnumerator enumerator) { enumerators.Add(enumerator); }
    public void ClearAllEnumerators() { enumerators.Clear(); }

    public void Invoke(MonoBehaviour invoker)
    {
        invoker.StartCoroutine(StartAsyncEvent(invoker));
    }

    IEnumerator StartAsyncEvent(MonoBehaviour invoker)
    {
        foreach (Action action in actions)
            action();

        List<Coroutine> coroutines = new List<Coroutine>();

        foreach (IEnumerator enumerator in enumerators)
            coroutines.Add(invoker.StartCoroutine(enumerator));

        foreach (Coroutine coroutine in coroutines)
            yield return coroutine;

        OnEventComplete.Invoke();
    }
}