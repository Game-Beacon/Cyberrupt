using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class PlayerWeaponText : GameBehaviour
{
    [SerializeField]
    private TextMeshPro weaponText;
    [SerializeField]
    private float showTime;
    [SerializeField]
    private float fadeTime;
    [SerializeField]
    private Vector2 gap;
    [SerializeField]
    private WeaponController controller;

    //判定玩家是否太靠近邊界用的
    private LayerMask checkMask; 

    private bool isActive = false;
    private Task task = null;

    public override void GameAwake()
    {
        weaponText.text = "";
        checkMask = CollisionLayer.instance.screenMask;
        controller.OnWeaponChange.AddListener(ShowWeapon);
    }

    public override void GameUpdate()
    {
        if(isActive)
            CalculateTextPosition();
    }

    private void ShowWeapon(Weapon weapon)
    {
        isActive = true;
        weaponText.text = weapon.data.weaponName;
        CalculateTextPosition();

        weaponText.alpha = 1;
        if (task != null && task.Running)
            task.Stop();
        weaponText.DOKill();
        task = new Task(FadeText());
    }

    //根據玩家所處的位置更新
    private void CalculateTextPosition()
    {
        float width = weaponText.preferredWidth;
        float height = weaponText.preferredHeight;

        RaycastHit2D up = Physics2D.Raycast(transform.parent.position, Vector2.up, gap.y + height, checkMask);
        RaycastHit2D down = Physics2D.Raycast(transform.parent.position, Vector2.down, gap.y + height, checkMask);
        RaycastHit2D left = Physics2D.Raycast(transform.parent.position, Vector2.left, gap.x + width, checkMask);
        RaycastHit2D right = Physics2D.Raycast(transform.parent.position, Vector2.right, gap.x + width, checkMask);

        Vector2 delta = Vector2.zero;

        if (up.collider != null)
            delta += Vector2.down * (gap.y + (height / 2));
        if (down.collider != null)
            delta += Vector2.up * (gap.y + (height / 2));
        if (left.collider != null)
            delta += Vector2.right * (gap.x + (width / 2));
        if (right.collider != null)
            delta += Vector2.left * (gap.x + (width / 2));

        if(delta == Vector2.zero)
            delta += Vector2.down * (gap.y + (height / 2));

        weaponText.rectTransform.localPosition = delta;
    }

    IEnumerator FadeText()
    {
        yield return new WaitForSeconds(showTime);
        weaponText.DOFade(0, fadeTime);
        yield return new WaitForSeconds(fadeTime);
        isActive = false;
        weaponText.text = "";
    }
}
