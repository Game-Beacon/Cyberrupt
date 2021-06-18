using System;
using DG.Tweening;
using UnityEngine;

public class Bot_Death : MonoBehaviour
{
    [SerializeField]
    private float minScale;
    [SerializeField]
    private float maxScale;

    void Start()
    {
        var self = transform;
        var sp2d = GetComponent<SpriteRenderer>();
        DOTween.Sequence()
            .Append(
                self.DOScale(Vector3.one * this.maxScale, 0.5f)
                .SetEase(Ease.OutCubic)
            )
            .Append(
                self.DOScale(Vector3.one * this.minScale, 0.1f)
                .SetEase(Ease.OutCubic)
            )
            // Do fade out at the same time
            .Join(sp2d.DOColor(new Color(1, 1, 1, 0), 0.1f))
            .OnComplete(() => Destroy(gameObject));
    }
}