using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData
{
    public readonly Property hp;
    public readonly Property atk;
    public readonly Property def;
    public readonly Property sht;

    public readonly float sps; //Shoot Per Second;

    public readonly Property eng;

    public PlayerData(PlayerBaseData Base)
    {
        hp = new Property(Base.hp);
        atk = new Property(Base.atk);
        def = new Property(Base.def);
        sht = new Property(Base.sht);
        sps = Base.sps;
        eng = new Property(Base.eng);
    }
}