using UnityEngine;
using UnityEngine.Events;

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
public class ObjectEvent<T> : UnityEvent<T> { } 
