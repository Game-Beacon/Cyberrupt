using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WeaponController : GameBehaviour
{
#if UNITY_EDITOR
    [SerializeField]
    private bool _oneShotKill;
    public bool oneShotKill { get { return _oneShotKill; } }
#endif

    [SerializeField]
    private Transform muzzle;
    [SerializeField]
    private GameObject bomb;
    [SerializeField]
    private int _bombCount;
    public int bombCount { get { return _bombCount; } }
    [SerializeField]
    private WeaponData baseWeapon;

    private Weapon _currentWeapon;
    public Weapon currentWeapon { get { return _currentWeapon; } }
    
    private List<Weapon> weapons = new List<Weapon>();

    private bool keyDown, keyUp;

    [HideInInspector]
    public float timeScale = 1;

    public FloatEvent OnKeyDown { get; } = new FloatEvent();
    public FloatEvent OnKey { get; } = new FloatEvent();
    public FloatEvent OnKeyUp { get; } = new FloatEvent();
    public ObjectEvent<Weapon> OnWeaponChange { get; } = new ObjectEvent<Weapon>();
    public IntEvent OnBombCountChange { get; } = new IntEvent();

    public UltVector2Event OnShoot = new UltVector2Event();
    public GameEvent OnChargeStart = new GameEvent();
    public GameEvent OnChargeStop = new GameEvent();

    public override void GameAwake()
    {
        Weapon newWeapon = new Weapon(this, baseWeapon, -1, muzzle);
        newWeapon.OnSelected();
        _currentWeapon = newWeapon;
        
        weapons.Add(newWeapon);
        update = false;
    }

    public void UpdateController()
    {
        float delta = Time.deltaTime * timeScale;
        currentWeapon.UpdateTime(delta);

        if (Input.GetKeyDown(KeyCode.Mouse0))
            keyDown = true;
        if (Input.GetKey(KeyCode.Mouse0))
            OnKey.Invoke(delta);
        if (Input.GetKeyUp(KeyCode.Mouse0))
            keyUp = true;

        if (keyDown)
            OnKeyDown.Invoke(delta);
        if (keyUp)
            OnKeyUp.Invoke(delta);

        if (_currentWeapon.ammoCount == 0)
            RemoveWeapon(_currentWeapon);

        if (Input.mouseScrollDelta.y != 0)
            SwitchWeapon(Input.mouseScrollDelta.y);

        if (Input.GetKeyDown(KeyCode.Q) && _bombCount > 0)
        {
            Instantiate(bomb, transform.position, Quaternion.identity);
            _bombCount--;
            OnBombCountChange.Invoke(_bombCount);
        }  
    }

    private void SwitchWeapon(float delta)
    {
        if (weapons.Count < 2)
            return;

        int index = weapons.IndexOf(_currentWeapon);
        if (delta > 0)
            index = (index + 1) % weapons.Count;
        else
            index = (index + weapons.Count - 1) % weapons.Count;

        keyDown = false;
        keyUp = false;

        _currentWeapon.OnDeselected();
        _currentWeapon = weapons[index];
        _currentWeapon.OnSelected();
        OnWeaponChange.Invoke(_currentWeapon);
    }

    public void AddBomb(int count)
    {
        _bombCount += count;
        OnBombCountChange.Invoke(_bombCount);
    }

    public void AddWeapon(WeaponData data, int ammo)
    {
        foreach (Weapon weapon in weapons)
        {
            if (weapon.data == data)
            {
                weapon.AddAmmo(ammo);
                return;
            }
        }

        keyDown = false;
        keyUp = false;

        Weapon newWeapon = new Weapon(this, data, ammo, muzzle);
        weapons.Add(newWeapon);
        _currentWeapon.OnDeselected();
        _currentWeapon = newWeapon;
        _currentWeapon.OnSelected();
        OnWeaponChange.Invoke(_currentWeapon);
    }

    private void RemoveWeapon(Weapon weapon)
    {
        if (!weapons.Contains(weapon))
            return;
        if (weapon == _currentWeapon)    
        {
            int index = weapons.IndexOf(_currentWeapon);
            index = (index + 1) % weapons.Count;

            keyDown = false;
            keyUp = false;

            _currentWeapon.OnDeselected();
            _currentWeapon = weapons[index];
            _currentWeapon.OnSelected();
            OnWeaponChange.Invoke(_currentWeapon);
        }
        weapons.Remove(weapon);
    }

    public void CancelKeyDown()
    {
        keyDown = false;
    }

    public void CancelKeyUp()
    {
        keyUp = false;
    }
}
