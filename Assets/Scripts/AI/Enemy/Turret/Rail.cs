using System.Collections;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Rail : GameBehaviour
{
    [SerializeField]
    private GameObject pathEnd;

    private float duration;
    private Vector3 from;
    private Vector3 to;

    private bool isCreating;
    private LineRenderer line;

    public override void GameStart()
    {
        this.line = GetComponent<LineRenderer>();
        this.line.positionCount = 2;
    }

    public bool PreparePath(Vector3 from, Vector3 to, float duration)
    {
        if(this.isCreating)
        {
            Debug.LogWarning("Set path value during creation, ignored.");
            return false;
        }
        this.from = from;
        this.to = to;
        this.duration = duration;
        return true;
    }

    private IEnumerator movePoint(int index)
    {
        this.isCreating = true;
        var current = this.from;
        var end = Instantiate(this.pathEnd, transform);
        yield return DOTween.To(
                () => current,
                x =>
                {
                    current = x;
                    this.line.SetPosition(index, current);
                    end.transform.position = current;
                },
                this.to,
                this.duration
            )
            .SetEase(Ease.Linear)
            .WaitForCompletion();
        this.isCreating = false;
        Destroy(end);
    }

    public IEnumerator Create()
    {
        this.line.SetPosition(0, this.from);
        this.line.SetPosition(1, this.from);
        yield return StartCoroutine(this.movePoint(1));
    }

    public IEnumerator Clear()
    {
        yield return StartCoroutine(this.movePoint(0));
        Destroy(gameObject);
    }
}