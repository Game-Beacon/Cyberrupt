using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProperty
{
    public readonly Property hp;
    public readonly Property atk;
    public readonly Property spread;
    public readonly Property sps;
    public readonly Property cps;

    public PlayerProperty(PlayerBaseProperty Base)
    {
        hp = new Property(Base.hp);
        atk = new Property(Base.atk);
        spread = new Property(Base.spread);
        sps = new Property(Base.sps);
        cps = new Property(Base.cps);
    }
}