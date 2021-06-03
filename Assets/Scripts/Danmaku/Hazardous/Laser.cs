using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Laser : GameBehaviour
{
    [SerializeField]
    private LineRenderer lr;
    [SerializeField]
    private Transform laserStart;
    [SerializeField]
    private Transform laserEnd;
    
    [SerializeField]
    private float laserWidth;
    [SerializeField]
    private float desiredLaserLength;

    private float radius { get { return laserWidth / 2f; } }
    private Vector2 direction 
    {
        get
        {
            float angle = transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
            return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        }
    }
    private LayerMask bombMask
    {
        get
        {
            return CollisionLayer.instance.bombMask;
        }
    }
    private LayerMask playerMask
    {
        get
        {
            return CollisionLayer.instance.playerMask;
        }
    }

    public override void GameAwake()
    {
        SetLaserRender(Vector3.zero);
    }

    public override void GameUpdate()
    {
        //Check the acutal length of the laser (the laser can be blocked by bomb)
        RaycastHit2D raycast = Physics2D.CircleCast(transform.position, radius / 2f, direction, desiredLaserLength, bombMask);
        float laserLength = (raycast.collider == null) ? desiredLaserLength : raycast.distance;
        SetLaserRender(direction * laserLength);

        //Check whether the laser collides with the player
        raycast = Physics2D.CircleCast(transform.position, radius / 2f, direction, laserLength, playerMask);
        if(raycast.collider != null)
            raycast.collider.gameObject.GetComponent<Player>().OnHit();
    }

    private void SetLaserRender(Vector3 endDelta)
    {
        float width = laserWidth * (1 + 0.05f * Mathf.Sin(Time.time * Mathf.PI * 20));

        lr.startWidth = width;
        lr.endWidth = width;
        lr.positionCount = 2;
        lr.SetPosition(0, transform.position);
        lr.SetPosition(1, transform.position + endDelta);
        laserStart.position = transform.position;
        laserStart.localScale = new Vector3(width / 2, width / 2, width / 2);
        laserEnd.position = transform.position + endDelta;
        laserEnd.localScale = new Vector3(width / 2, width / 2, width / 2);
    }

    /// <summary>
    /// 設定雷射的屬性，如果要調整雷射的角度請直接修改rotation
    /// </summary>
    /// <param name="width">雷射的寬度</param>
    /// <param name="length">雷射的最大長度</param>
    public void SetLaserProperty(float width, float length)
    {
        laserWidth = width;
        desiredLaserLength = length;
    }

    public void Kill(float killTime)
    {
        update = false;
        StartCoroutine(KillAction(killTime));
    }

    private IEnumerator KillAction(float killTime)
    {
        DOTween.To(() => lr.startWidth, x => lr.startWidth = x, 0, killTime);
        DOTween.To(() => lr.endWidth, x => lr.endWidth = x, 0, killTime);
        laserStart.DOScale(Vector3.zero, killTime);
        laserEnd.DOScale(Vector3.zero, killTime);
        yield return new WaitForSeconds(killTime);
        KillBehaviour(true);
    }
}
