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

    private Weapon currentWeapon;
    private List<Weapon> weapons = new List<Weapon>();

    private float _timer;
    public float timer { get { return _timer; } }
    public int bombCount { get { return _bombCount; } }

    public GameEvent OnKeyDown { get; } = new GameEvent();
    public GameEvent OnKey { get; } = new GameEvent();
    public GameEvent OnKeyUp { get; } = new GameEvent();
    public IntEvent OnBombCountChange { get; } = new IntEvent();

    public override void GameAwake()
    {
        Weapon newWeapon = new Weapon(this, baseWeapon, -1, muzzle);
        newWeapon.OnSelected();
        currentWeapon = newWeapon;
        
        weapons.Add(newWeapon);
    }

    public void UpdateController()
    {
        _timer += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Mouse0))
            OnKeyDown.Invoke();
        if (Input.GetKey(KeyCode.Mouse0))
            OnKey.Invoke();
        if (Input.GetKeyUp(KeyCode.Mouse0))
            OnKeyUp.Invoke();

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
        Debug.Log("Switch Weapon!");

        int index = weapons.IndexOf(currentWeapon);
        index = (index + 1) % weapons.Count;

        currentWeapon.OnDeselected();
        currentWeapon = weapons[index];
        currentWeapon.OnSelected();
    }
}
