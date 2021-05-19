using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WeaponController : GameBehaviour
{
    [SerializeField]
    private Transform muzzle;
    [SerializeField]
    private GameObject bomb;
    [SerializeField]
    private int _bombCount;
    [SerializeField]
    private WeaponData baseWeapon;

    private Weapon _currentWeapon;
    public Weapon currentWeapon { get { return _currentWeapon; } }
    private List<Weapon> weapons = new List<Weapon>();

    private float _timer;
    public float timer { get { return _timer; } }
    public int bombCount { get { return _bombCount; } }

    private bool keyDown, keyUp;

    public GameEvent OnKeyDown { get; } = new GameEvent();
    public GameEvent OnKey { get; } = new GameEvent();
    public GameEvent OnKeyUp { get; } = new GameEvent();
    public ObjectEvent OnWeaponChange { get; } = new ObjectEvent();
    public IntEvent OnBombCountChange { get; } = new IntEvent();

    public override void GameAwake()
    {
        Weapon newWeapon = new Weapon(this, baseWeapon, -1, muzzle);
        newWeapon.OnSelected();
        _currentWeapon = newWeapon;
        
        weapons.Add(newWeapon);
    }

    public void UpdateController()
    {
        _timer += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Mouse0))
            keyDown = true;
        if (Input.GetKey(KeyCode.Mouse0))
            OnKey.Invoke();
        if (Input.GetKeyUp(KeyCode.Mouse0))
            keyUp = true;

        if (keyDown)
            OnKeyDown.Invoke();
        if(keyUp)
            OnKeyUp.Invoke();

        if (_currentWeapon.ammoCount == 0)
            RemoveWeapon(_currentWeapon);

        if (Input.GetKeyDown(KeyCode.Q))
            SwitchWeapon();

        if (Input.GetKeyDown(KeyCode.R) && _bombCount > 0)
        {
            Instantiate(bomb, transform.position, Quaternion.identity);
            _bombCount--;
            OnBombCountChange.Invoke(_bombCount);
        }  
    }

    private void SwitchWeapon()
    {
        int index = weapons.IndexOf(_currentWeapon);
        index = (index + 1) % weapons.Count;

        _currentWeapon.OnDeselected();
        _currentWeapon = weapons[index];
        _currentWeapon.OnSelected();
        OnWeaponChange.Invoke(_currentWeapon);
        Debug.Log("Switch Weapon! " + _currentWeapon.data.weaponName + " : " + _currentWeapon.ammoCount);
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
