using System;
using System.Collections.Generic;

public enum Easing
{
    Flash = 0,
    Linear = 1,
    InSine = 2,
    OutSine = 3,
    InOutSine = 4,
    InQuad = 5,
    OutQuad = 6,
    InOutQuad = 7,
    InCubic = 8,
    OutCubic = 9,
    InOutCubic = 10,
    InQuart = 11,
    OutQuart = 12,
    InOutQuart = 13,
    InQuint = 14,
    OutQuint = 15,
    InOutQuint = 16,
    InExpo = 17,
    OutExpo = 18,
    InOutExpo = 19,
    InCirc = 20,
    OutCirc = 21,
    InOutCirc = 22,
    InBack = 23,
    OutBack = 24,
    InOutBack = 25,
}

public static class EaseLibrary
{
    private delegate float EasingFunction(float time);
    private static readonly Dictionary<Easing, EasingFunction> EaseDictionary = new Dictionary<Easing, EasingFunction>()
    {
        { Easing.Flash, new EasingFunction(Flash) },
        { Easing.Linear, new EasingFunction(Linear) },
        { Easing.InSine, new EasingFunction(InSine) },
        { Easing.OutSine, new EasingFunction(OutSine) },
        { Easing.InOutSine, new EasingFunction(InOutSine) },
        { Easing.InQuad, new EasingFunction(InQuad) },
        { Easing.OutQuad, new EasingFunction(OutQuad) },
        { Easing.InOutQuad, new EasingFunction(InOutQuad) },
        { Easing.InCubic, new EasingFunction(InCubic) },
        { Easing.OutCubic, new EasingFunction(OutCubic) },
        { Easing.InOutCubic, new EasingFunction(InOutCubic) },
        { Easing.InQuart, new EasingFunction(InQuart) },
        { Easing.OutQuart, new EasingFunction(OutQuart) },
        { Easing.InOutQuart, new EasingFunction(InOutQuart) },
        { Easing.InQuint, new EasingFunction(InQuint) },
        { Easing.OutQuint, new EasingFunction(OutQuint) },
        { Easing.InOutQuint, new EasingFunction(InOutQuint) },
        { Easing.InExpo, new EasingFunction(InExpo) },
        { Easing.OutExpo, new EasingFunction(OutExpo) },
        { Easing.InOutExpo, new EasingFunction(InOutExpo) },
        { Easing.InCirc, new EasingFunction(InCirc) },
        { Easing.OutCirc, new EasingFunction(OutCirc) },
        { Easing.InOutCirc, new EasingFunction(InOutCirc) },
        { Easing.InBack, new EasingFunction(InBack) },
        { Easing.OutBack, new EasingFunction(OutBack) },
        { Easing.InOutBack, new EasingFunction(InOutBack) },
    };

    public static float CallEaseFunction(Easing easeType, float time)
    {
        if (time <= 0) return 0;
        if (time >= 1) return 1;
        return EaseDictionary[easeType](time);
    }

    private static float Flash(float time)
    {
        return 1;
    }
    private static float Linear(float time)
    {
        return time;
    }
    private static float InSine(float time)
    {
        return 1 - (float)Math.Cos(time * Math.PI / 2); 
    }
    private static float OutSine(float time)
    {
        return (float)Math.Sin((time * Math.PI) / 2);
    }
    private static float InOutSine(float time)
    {
        return (float)-(Math.Cos(Math.PI * time) - 1) / 2;
    }
    private static float InQuad(float time)
    {
        return time * time;
    }
    private static float OutQuad(float time)
    {
        return 1 - (1 - time) * (1 - time);
    }
    private static float InOutQuad(float time)
    {
        return (time < 0.5f) ? 2 * time * time : 1 - 2 * (1 - time) * (1 - time);
    }
    private static float InCubic(float time)
    {
        return time * time * time;
    }
    private static float OutCubic(float time)
    {
        return 1 - (1 - time) * (1 - time) * (1 - time);
    }
    private static float InOutCubic(float time)
    {
        return (time < 0.5f) ? 4 * time * time * time : 1 - 4 * (1 - time) * (1 - time) * (1 - time);
    }
    private static float InQuart(float time)
    {
        return time * time * time * time;
    }
    private static float OutQuart(float time)
    {
        return 1 - (1 - time) * (1 - time) * (1 - time) * (1 - time);
    }
    private static float InOutQuart(float time)
    {
        return (time < 0.5f) ? 8 * time * time * time * time : 1 - 8 * (1 - time) * (1 - time) * (1 - time) * (1 - time);
    }
    private static float InQuint(float time)
    {
        return time * time * time * time * time;
    }
    private static float OutQuint(float time)
    {
        return 1 - (1 - time) * (1 - time) * (1 - time) * (1 - time) * (1 - time);
    }
    private static float InOutQuint(float time)
    {
        return (time < 0.5f) ? 16 * time * time * time * time * time : 1 - 16 * (1 - time) * (1 - time) * (1 - time) * (1 - time) * (1 - time);
    }
    private static float InExpo(float time)
    {
        return (time == 0) ? 0 : (float)Math.Pow(2, 10 * (time - 1));
    }
    private static float OutExpo(float time)
    {
        return (time == 1) ? 1 : 1 - (float)Math.Pow(2, -10 * time);
    }
    private static float InOutExpo(float time)
    {
        return (time < 0.5f)? (float)Math.Pow(2, 20 * time - 10) / 2 : (float)(2 - Math.Pow(2, -20 * time + 10)) / 2;
    }
    private static float InCirc(float time)
    {
        return 1 - (float)Math.Sqrt(1 - time * time);
    }
    private static float OutCirc(float time)
    {
        return (float)Math.Sqrt(1 - (time - 1) * (time - 1));
    }
    private static float InOutCirc(float time)
    {
        return (time < 0.5f)? (float)(1 - Math.Sqrt(1 - Math.Pow(2 * time, 2))) / 2 : (float)(Math.Sqrt(1 - Math.Pow(-2 * time + 2, 2)) + 1) / 2;
    }
    private static float InBack(float time)
    {
        const float c1 = 1.70158f;
        const float c3 = c1 + 1;
        return c3 * time * time * time - c1 * time * time;
    }
    private static float OutBack(float time)
    {
        const float c1 = 1.70158f;
        const float c3 = c1 + 1;
        return 1 + c3 * (time - 1) * (time - 1) * (time - 1) + c1 * (time - 1) * (time - 1);
    }
    private static float InOutBack(float time)
    {
        const float c1 = 1.70158f;
        const float c2 = c1 * 1.525f;
        return (time < 0.5f)? (float)(Math.Pow(2 * time, 2) * ((c2 + 1) * 2 * time - c2)) / 2 : (float)(Math.Pow(2 * time - 2, 2) * ((c2 + 1) * (time * 2 - 2) + c2) + 2) / 2;
    }
}