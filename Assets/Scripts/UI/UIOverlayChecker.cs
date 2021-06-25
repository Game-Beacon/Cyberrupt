using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


[RequireComponent(typeof(CanvasGroup))]
public class UIOverlayChecker : GameBehaviour
{
    private RectTransform canvas;
    private CanvasGroup group;
    private Camera cam;
    private LayerMask checkMask;
    private bool previousOverlay = false;
    private bool isOverlay = false;
    
    private static readonly float alpha = 0.4f;
    private static readonly float fadeTime = 0.3f;

    public override void GameAwake()
    {
        cam = Camera.main;
        canvas = GetComponentInParent<Canvas>().gameObject.GetComponent<RectTransform>();
        group = GetComponent<CanvasGroup>();
        checkMask = CollisionLayer.instance.enemyMask | CollisionLayer.instance.playerMask;
    }

    public override void GameFixedUpdate()
    {
        bool findOverlay = false;
        //非常暴力而且浪費效能，but whatever, it works.
        foreach (Transform child in GetComponentsInChildren<Transform>())
        {
            if (child == transform)
                continue;

            RectTransform r = null;
            if(child.gameObject.TryGetComponent(out r))
            {
                // v[1]----------v[2]
                //   |             |
                //   |             |
                //   |             |
                // v[0]----------v[3]
                Vector3[] v = new Vector3[4];
                r.GetWorldCorners(v);
                for (int i = 0; i < 4; i++)
                    v[i] = CanvasToWorldPos(v[i]);

                Vector2 center = (v[0] + v[2]) / 2;
                float width = Mathf.Abs(v[1].x - v[2].x);
                float height = Mathf.Abs(v[0].y - v[1].y);
                Vector2 size = new Vector2(width, height);

                //Debug.DrawLine(new Vector2(center.x - width / 2, center.y - height / 2), new Vector2(center.x + width / 2, center.y - height / 2));
                //Debug.DrawLine(new Vector2(center.x + width / 2, center.y - height / 2), new Vector2(center.x + width / 2, center.y + height / 2));
                //Debug.DrawLine(new Vector2(center.x + width / 2, center.y + height / 2), new Vector2(center.x - width / 2, center.y + height / 2));
                //Debug.DrawLine(new Vector2(center.x - width / 2, center.y + height / 2), new Vector2(center.x - width / 2, center.y - height / 2));

                RaycastHit2D raycast = Physics2D.BoxCast(center, size, 0, Vector2.zero, 0, checkMask);
                if(raycast.collider != null)
                {
                    findOverlay = true;
                    break;
                }
            }
        }

        isOverlay = findOverlay;

        if (isOverlay && !previousOverlay)
        {
            group.DOKill();
            DOTween.To(() => group.alpha, x => group.alpha = x, alpha, fadeTime);
        }    
        else if (!isOverlay && previousOverlay)
        {
            group.DOKill();
            DOTween.To(() => group.alpha, x => group.alpha = x, 1, fadeTime);
        }   
        previousOverlay = isOverlay;
    }

    private Vector3 CanvasToWorldPos(Vector3 v)
    {
        v -= canvas.position;
        v /= ((canvas.sizeDelta.y * canvas.localScale.y) / (cam.orthographicSize * 2));
        return v;
    }
}
