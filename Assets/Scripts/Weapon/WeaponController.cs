using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WeaponController : GameBehaviour
{
    [SerializeField]
    private Transform muzzle;
    [SerializeField]
    private WeaponData baseWeapon;

    private Weapon currentWeapon;
    private List<Weapon> weapons = new List<Weapon>();

    private float _timer;
    public float timer { get { return _timer; } }

    private UnityEvent _OnKeyDown = new UnityEvent(), _OnKey = new UnityEvent(), _OnKeyUp = new UnityEvent();
    public UnityEvent OnKeyDown { get { return _OnKeyDown; } }
    public UnityEvent OnKey { get { return _OnKey; } }
    public UnityEvent OnKeyUp { get { return _OnKeyUp; } }

    public override void GameAwake()
    {
        Weapon newWeapon = new Weapon(this, baseWeapon, -1, muzzle);
        newWeapon.OnSelected();
        currentWeapon = newWeapon;
        
        weapons.Add(newWeapon);
    }

    public override void GameUpdate()
    {
        _timer += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Mouse0))
            _OnKeyDown.Invoke();
        if (Input.GetKey(KeyCode.Mouse0))
            _OnKey.Invoke();
        if (Input.GetKeyUp(KeyCode.Mouse0))
            _OnKeyUp.Invoke();

        if (Input.GetKeyDown(KeyCode.Q))
            SwitchWeapon();
    }

    private void SwitchWeapon()
    {
        Debug.Log("Switch Weapon!");

        int index = weapons.IndexOf(currentWeapon);
        index = (index + 1) % weapons.Count;

        currentWeapon.OnDeselected();
        currentWeapon = weapons[index];
        currentWeapon.OnSelected();
    }
}
