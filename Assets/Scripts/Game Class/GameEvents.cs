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

