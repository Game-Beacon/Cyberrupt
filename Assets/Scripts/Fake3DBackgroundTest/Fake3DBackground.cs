using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Fake3DBackground : GameBehaviour
{
    [SerializeField]
    private Color color;
    [SerializeField]
    private SpriteRenderer bg;
    [SerializeField]
    private ParticleSystem ps;
    [SerializeField]
    private GameObject vertical;
    [SerializeField]
    private GameObject horizontal;
    [SerializeField]
    private GameObject LinePrefab;

    [Space(10),SerializeField]
    private float minY;
    [SerializeField]
    private float maxY;
    [SerializeField]
    private float xGap;
    [SerializeField]
    private float xShiftRatio;
    [SerializeField]
    private int ySegment;
    [SerializeField]
    private Easing yEase;

    [Space(10), SerializeField]
    private float verticalMoveSpeed;
    [SerializeField]
    private float horizontalMoveTime;

    [Space(10), SerializeField]
    private List<SpriteRenderer> circuits = new List<SpriteRenderer>();
    [SerializeField]
    private float circuitWidth;

    [Space(10), SerializeField]
    private Color bossColor;
    [SerializeField]
    private float bossVerticalSpeed;
    [SerializeField]
    private float bossHorizontalTime;
    [SerializeField]
    private float enterBossTime;
    [SerializeField]
    private float exitBossTime;

    private Color normalColor;
    private float normalVerticalSpeed;
    private float normalHorizontalTime;


    private class HorizontalLineData
    {
        public LineRenderer lr;
        public float progress;
    }

    private Color previousColor;
    private Color lrColor { get { return new Color(color.r * 0.75f, color.g * 0.75f, color.b * 0.75f, 1); } }
    private Color bgColor { get { return new Color(color.r / 6, color.g / 6, color.b / 6, 1); } }
    private Color psColor { get { return new Color(color.r / 4, color.g / 4, color.b / 4, 1); } }

    private Color circuitColor { get { return new Color(color.r / 3, color.g / 3, color.b / 3, 1); } }

    private float xMin;
    private float xMax;

    private Camera cam;
    private float camWidth;
    private float camHeight;
    private List<LineRenderer> verticals = new List<LineRenderer>();
    private List<HorizontalLineData> horizontals = new List<HorizontalLineData>();

    public override void GameStart()
    {
        cam = Camera.main;
        camHeight = cam.orthographicSize * 2;
        camWidth = camHeight * cam.aspect;

        int xCount = Mathf.CeilToInt((camWidth / 2) / xGap);
        xMin = -xGap * xCount;
        xMax = xGap * xCount;

        LineRenderer lr;
        lr = Instantiate(LinePrefab, vertical.transform).GetComponent<LineRenderer>();
        SetLine(lrColor, new Color(lrColor.r, lrColor.g, lrColor.b, 0), new Vector3(0, maxY), new Vector3(0, minY), ref lr);
        verticals.Add(lr);
        lr = Instantiate(LinePrefab, vertical.transform).GetComponent<LineRenderer>();
        SetLine(lrColor, new Color(lrColor.r, lrColor.g, lrColor.b, 0), new Vector3(0, -maxY), new Vector3(0, -minY), ref lr);
        verticals.Add(lr);

        for (int i = 1; i <= xCount; i ++)
        {
            lr = Instantiate(LinePrefab, vertical.transform).GetComponent<LineRenderer>();
            SetLine(lrColor, new Color(lrColor.r, lrColor.g, lrColor.b, 0), new Vector3(xGap * i * xShiftRatio, maxY), new Vector3(xGap * i, minY), ref lr);
            verticals.Add(lr);
            lr = Instantiate(LinePrefab, vertical.transform).GetComponent<LineRenderer>();
            SetLine(lrColor, new Color(lrColor.r, lrColor.g, lrColor.b, 0), new Vector3(xGap * i * xShiftRatio, -maxY), new Vector3(xGap * i, -minY), ref lr);
            verticals.Add(lr);
            lr = Instantiate(LinePrefab, vertical.transform).GetComponent<LineRenderer>();
            SetLine(lrColor, new Color(lrColor.r, lrColor.g, lrColor.b, 0), new Vector3(-xGap * i * xShiftRatio, maxY), new Vector3(-xGap * i, minY), ref lr);
            verticals.Add(lr);
            lr = Instantiate(LinePrefab, vertical.transform).GetComponent<LineRenderer>();
            SetLine(lrColor, new Color(lrColor.r, lrColor.g, lrColor.b, 0), new Vector3(-xGap * i * xShiftRatio, -maxY), new Vector3(-xGap * i, -minY), ref lr);
            verticals.Add(lr);
        }

        for(int i = 0; i < ySegment; i++)
        {
            float progress = (float)i / ySegment;
            float trueProgress = EaseLibrary.CallEaseFunction(yEase, progress);
            float y = Mathf.Lerp(minY, maxY, trueProgress);
            lr = Instantiate(LinePrefab, horizontal.transform).GetComponent<LineRenderer>();
            SetLine(new Color(lrColor.r, lrColor.g, lrColor.b, trueProgress), new Color(lrColor.r, lrColor.g, lrColor.b, trueProgress), new Vector3((-camWidth / 2) - 1, y), new Vector3((camWidth / 2) + 1, y), ref lr);
            HorizontalLineData data1 = new HorizontalLineData();
            data1.lr = lr;
            data1.progress = progress;
            horizontals.Add(data1);
            lr = Instantiate(LinePrefab, horizontal.transform).GetComponent<LineRenderer>();
            SetLine(new Color(lrColor.r, lrColor.g, lrColor.b, trueProgress), new Color(lrColor.r, lrColor.g, lrColor.b, trueProgress), new Vector3((-camWidth / 2) - 1, -y), new Vector3((camWidth / 2) + 1, -y), ref lr);
            HorizontalLineData data2 = new HorizontalLineData();
            data2.lr = lr;
            data2.progress = progress;
            horizontals.Add(data2);
        }

        previousColor = color;
        ModifyColor();

        normalColor = color;
        normalHorizontalTime = horizontalMoveTime;
        normalVerticalSpeed = verticalMoveSpeed;

        EnemyManager.instance.OnEnterBossWave.AddListener(EnterBoss);
        EnemyManager.instance.OnExitBossWave.AddListener(ExitBoss);
    }

    public override void GameFixedUpdate()
    {
        for (int i = 0; i < verticals.Count; i++)
        {
            LineRenderer lr = verticals[i];
            float x = lr.GetPosition(1).x + verticalMoveSpeed * Time.deltaTime;
            if (x < xMin)
                x += 2 * xMax;
            if (x > xMax)
                x -= 2 * xMax;

            SetLine(lrColor, new Color(lrColor.r, lrColor.g, lrColor.b, 0), new Vector3(x * xShiftRatio, lr.GetPosition(0).y), new Vector3(x, lr.GetPosition(1).y), ref lr);
            verticals[i] = lr;
        }

        for (int i = 0; i < horizontals.Count; i++)
        {
            LineRenderer lr = horizontals[i].lr;
            float progress = horizontals[i].progress;
            progress += (1 / horizontalMoveTime) * Time.fixedDeltaTime;
            if (progress >= 1)
                progress -= 1;
            float trueProgress = EaseLibrary.CallEaseFunction(yEase, progress);
            float y = Mathf.Lerp(minY, maxY, trueProgress);
            y = (lr.GetPosition(0).y > 0) ? y : -y;
            SetLine(new Color(lrColor.r, lrColor.g, lrColor.b, trueProgress), new Color(lrColor.r, lrColor.g, lrColor.b, trueProgress), new Vector3((-camWidth / 2) - 1, y), new Vector3((camWidth / 2) + 1, y), ref lr);
            horizontals[i].lr = lr;
            horizontals[i].progress = progress;
        }
  
        foreach(SpriteRenderer sr in circuits)
        {
            sr.transform.localPosition += Vector3.right * verticalMoveSpeed * 0.5f * Time.deltaTime;
            if (sr.transform.localPosition.x < -12)
                sr.transform.localPosition += Vector3.right * circuitWidth * circuits.Count;
        }

        var main = ps.main;
        main.simulationSpeed = Mathf.Abs(verticalMoveSpeed);

        if(previousColor != color)
            ModifyColor();
        previousColor = color;
    }

    private void ModifyColor()
    {
        bg.color = new Color(bgColor.r, bgColor.g, bgColor.b, bg.color.a);

        foreach (SpriteRenderer sr in circuits)
            sr.color = circuitColor;

        var col = ps.colorOverLifetime;
        Gradient grad = new Gradient();
        grad.SetKeys
        (
            new GradientColorKey[]
            { new GradientColorKey(psColor, 0.0f), new GradientColorKey(psColor, 1.0f) },
            new GradientAlphaKey[]
            { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 0.9f),  new GradientAlphaKey(0.0f, 0.9f)}
        );
        col.color = grad;
    }

    private void SetLine(Color start, Color end, Vector3 startPos, Vector3 endPos, ref LineRenderer lr)
    {
        lr.SetColors(start, end);
        lr.SetPosition(0, startPos);
        lr.SetPosition(1, endPos);
    }

    public void EnterBoss()
    {
        DOTween.To(() => color, x => color = x, bossColor, enterBossTime);
        DOTween.To(() => verticalMoveSpeed, x => verticalMoveSpeed = x, bossVerticalSpeed, enterBossTime);
        DOTween.To(() => horizontalMoveTime, x => horizontalMoveTime = x, bossHorizontalTime, enterBossTime);
    }

    public void ExitBoss()
    {
        DOTween.To(() => color, x => color = x, normalColor, enterBossTime);
        DOTween.To(() => verticalMoveSpeed, x => verticalMoveSpeed = x, normalVerticalSpeed, exitBossTime);
        DOTween.To(() => horizontalMoveTime, x => horizontalMoveTime = x, normalHorizontalTime, exitBossTime);
    }
}
