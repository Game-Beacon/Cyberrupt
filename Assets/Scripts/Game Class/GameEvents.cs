using UnityEngine;
using UnityEngine.Events;

public class GameEvent : UnityEvent { } 
public class IntEvent : UnityEvent<int> { }
public class FloatEvent : UnityEvent<float> { }
public class BoolEvent : UnityEvent<bool> { }
public class GameObjectEvent : UnityEvent<GameObject> { }
