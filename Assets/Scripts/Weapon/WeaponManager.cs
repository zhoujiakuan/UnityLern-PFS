using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [Header("武器库")]
    public List<Firearms> allWeapons;
    [Header("主武器")]
    public Firearms MainWeapon;
    [Header("副武器")]
    public Firearms SecondlyWeapon;
    [Header("当前武器")]
    [SerializeField] private Firearms CurrentWeapon;
    [Header("拾取检测")]
    [SerializeField] private float rayDistance;
    [SerializeField] private LayerMask layerMask;

    PlayerController playerController;
    IEnumerator waitingHolster;
    Camera pickUpCam;

    private void Start()
    {
        pickUpCam = GameObject.FindWithTag("FPCam").GetComponent<Camera>();
        if (SecondlyWeapon)
        {
            SecondlyWeapon.gameObject.SetActive(false);
        }
        playerController = FindObjectOfType<PlayerController>();
        if (MainWeapon)
        {
            //CurrentWeapon = MainWeapon;
            if (CurrentWeapon)
            {
                playerController.SetUpAnimator(CurrentWeapon.GunAnim);
            }
        }
    }

    private void Update()
    {
        //按E拾取武器噢
        PickCheck();

        if (CurrentWeapon == null) return;

        SwapWeapon();

        //射击
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0))
        {
            //正在切枪的时候不能射击
            if (waitingHolster != null) return;
            CurrentWeapon.DoAttack();
        }

        //装子弹逻辑
        if (Input.GetMouseButtonDown(0))
        {
            CurrentWeapon.DoReload(0);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            CurrentWeapon.DoReload(1);
        }

        //瞄准逻辑
        if (Input.GetMouseButtonDown(1))
        {
            //正在切枪的时候不能瞄准
            if (waitingHolster != null) return;
            CurrentWeapon.Aim(true);
        }
        if (Input.GetMouseButtonUp(1))
        {
            //取消瞄准
            CurrentWeapon.Aim(false);
        }
    }

    /// <summary>
    /// 切换武器
    /// </summary>
    private void SwapWeapon()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && waitingHolster == null)
        {
            //如果主武器存在 并且当前武器不等于主武器的情况下执行
            if (MainWeapon && CurrentWeapon != MainWeapon)
            {
                //正在瞄准的时候切枪 要先取消瞄准
                CurrentWeapon.Aim(false);
                CurrentWeapon.GunAnim.SetTrigger("Holster");
                StartWaitingHolster(MainWeapon);
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && waitingHolster == null)
        {
            //如果副武器存在 并且当前武器不等于副武器的情况下执行
            if (SecondlyWeapon && CurrentWeapon != SecondlyWeapon)
            {
                //正在瞄准的时候切枪 要先取消瞄准
                CurrentWeapon.Aim(false);
                CurrentWeapon.GunAnim.SetTrigger("Holster");
                StartWaitingHolster(SecondlyWeapon);
            }
        }
    }

    /// <summary>
    /// 等待收枪动画完毕
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitingHolster(Firearms targetWeapon)
    {
        while (true)
        {
            AnimatorStateInfo animatorStateInfo = CurrentWeapon.GunAnim.GetCurrentAnimatorStateInfo(0);
            if(animatorStateInfo.normalizedTime >= 0.9f)
            {
                CurrentWeapon.isReloading = false;
                CurrentWeapon.gameObject.SetActive(false);
                targetWeapon.gameObject.SetActive(true);
                CurrentWeapon = targetWeapon;
                playerController.SetUpAnimator(CurrentWeapon.GunAnim);
                waitingHolster = null;
                yield break;
            }
            yield return null;
        }
    }

    /// <summary>
    /// 启动收枪动画的携程
    /// </summary>
    /// <param name="targetWeapon"></param>
    private void StartWaitingHolster(Firearms targetWeapon)
    {
        if(waitingHolster == null)
        {
            waitingHolster = WaitingHolster(targetWeapon);
        }
        StartCoroutine(WaitingHolster(targetWeapon));
    }

    /// <summary>
    /// 拾取武器的射线检测
    /// </summary>
    private void PickCheck()
    {
        RaycastHit hitinfo;
        if(Physics.Raycast(pickUpCam.transform.position,pickUpCam.transform.forward,out hitinfo, rayDistance, layerMask))
        {
            //尝试获取一下目标有没有baseitem组件
            if(hitinfo.collider.TryGetComponent(out BaseItem baseItem))
            {
                if(baseItem is FirearmsItem firearmsItem)
                {
                    foreach (var item in allWeapons)
                    {
                        //对比一下检测到的武器的名字
                        if (firearmsItem.WeaponName.CompareTo(item.name) == 0)
                        {
                            if (Input.GetKeyDown(KeyCode.E))
                            {
                                if (firearmsItem.CurrentFirearmsType.Equals(FirearmsItem.FirearmsType.AssaultRifle))
                                {
                                    MainWeapon = item;
                                }
                                if (firearmsItem.CurrentFirearmsType.Equals(FirearmsItem.FirearmsType.HandGun))
                                {
                                    SecondlyWeapon = item;
                                }
                                SetUpWeapon(item);
                            }
                        }
                    }
                }
            }
        }
    }

    private void SetUpWeapon(Firearms weapon)
    {
        if (CurrentWeapon)
        {
            //如果要捡的武器和手上的一样就不返回
            if (weapon.name.CompareTo(CurrentWeapon.name) == 0) return;

            CurrentWeapon.isReloading = false;
            CurrentWeapon.Aim(false);
            CurrentWeapon.gameObject.SetActive(false);
        }
        weapon.gameObject.SetActive(true);
        CurrentWeapon = weapon;
        playerController.SetUpAnimator(weapon.GunAnim);
    }
}
