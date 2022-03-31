using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [Header("主武器")]
    public Firearms MainWeapon;
    [Header("副武器")]
    public Firearms SecondlyWeapon;
    [Header("当前武器")]
    [SerializeField] private Firearms CurrentWeapon;

    PlayerController playerController;
    IEnumerator waitingHolster;

    private void Start()
    {
        if (MainWeapon)
        {
            CurrentWeapon = MainWeapon;
            playerController.SetUpAnimator(CurrentWeapon.GunAnim);
        }
        SecondlyWeapon.gameObject.SetActive(false);
        playerController = FindObjectOfType<PlayerController>();
    }

    private void Update()
    {
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
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if(CurrentWeapon != MainWeapon && waitingHolster == null)
            {
                //正在瞄准的时候切枪 要先取消瞄准
                CurrentWeapon.Aim(false);
                CurrentWeapon.GunAnim.SetTrigger("Holster");
                StartWaitingHolster(MainWeapon);
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && waitingHolster == null)
        {
            if (CurrentWeapon != SecondlyWeapon)
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
}
