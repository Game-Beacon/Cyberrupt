using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class DanmakuParticleData : ScriptableObject
{
    [SerializeField]
    private EmitModule _emitModule;
    public EmitModule emitModule { get { return _emitModule; } private set { } }

    [SerializeField]
    private ShapeModule _shapeModule;
    public ShapeModule shapeModule { get { return _shapeModule; } private set { } }

    [SerializeField]
    private PatternModule _patternModule;
    public PatternModule patternModule { get { return _patternModule; } private set { } }

    [SerializeField]
    private PathModule _pathModule;
    public PathModule pathModule { get { return _pathModule; } private set { } }

    [SerializeField]
    private bool _useSubEmitter;
    public bool useSubEmitter { get { return _useSubEmitter; } private set { } }

    [SerializeField]
    private SubEmitModule _subEmitModule;
    public SubEmitModule subEmitModule { get { return _subEmitModule; } private set { } }
}
