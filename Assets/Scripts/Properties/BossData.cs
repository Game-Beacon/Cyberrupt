using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossData
{
    public readonly Property hp;
    public readonly Property atk;
    public readonly Property def;

    public BossData(BossBaseData Base)
    {
        hp = new Property(Base.hp);
        atk = new Property(Base.atk);
        def = new Property(Base.def);
    }
}
