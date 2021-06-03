using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenBound : GameBehaviour
{
    public static ScreenBound instance { get; private set; }

    [SerializeField]
    private Bound _screen;

    public float GetWorldScreenMinX { get { return _screen.parent.position.x - (_screen.xSize / 2); } }
    public float GetWorldScreenMaxX { get { return _screen.parent.position.x + (_screen.xSize / 2); } }
    public float GetWorldScreenMinY { get { return _screen.parent.position.y - (_screen.ySize / 2); } }
    public float GetWorldScreenMaxY { get { return _screen.parent.position.y + (_screen.ySize / 2); } }

    public override void GameAwake()
    {
        if (instance == null)
            instance = this;
        else
        {
            KillBehaviour(true);
            return;
        }
    }

    public bool InScreen(Vector2 position)
    {
        return !_screen.OverBound(position);
    }

    public Vector2 GetRandomPointInScreen(float edge)
    {
        float maxX = _screen.parent.position.x + (_screen.xSize / 2) - edge;
        float minX = _screen.parent.position.x - (_screen.xSize / 2) + edge;
        float maxY = _screen.parent.position.y + (_screen.ySize / 2) - edge;
        float minY = _screen.parent.position.y - (_screen.ySize / 2) + edge;

        return new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));
    }

    public Vector2 GetRandomPointOnScreenEdge()
    {
        float maxX = _screen.parent.position.x + (_screen.xSize / 2);
        float minX = _screen.parent.position.x - (_screen.xSize / 2);
        float maxY = _screen.parent.position.y + (_screen.ySize / 2);
        float minY = _screen.parent.position.y - (_screen.ySize / 2);

        float lerp = Random.Range(0f, 1f);
        int rand = Random.Range(0, 4);

        if (rand == 0)
            return new Vector2(minX, Mathf.Lerp(minY, maxY, lerp));
        else if (rand == 1)
            return new Vector2(maxX, Mathf.Lerp(minY, maxY, lerp));
        else if (rand == 2)
            return new Vector2(Mathf.Lerp(minX, maxX, lerp), minY);
        else
            return new Vector2(Mathf.Lerp(minX, maxX, lerp), maxY);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3(_screen.xSize, _screen.ySize));
    }
}
