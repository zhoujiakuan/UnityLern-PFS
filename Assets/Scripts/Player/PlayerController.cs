using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private Animator anim;
    private CharacterController cc;
    public Vector3 velocity;//charactercontroller的velocity
    [Header("角色模型")]
    public Transform characterTransform;
    [Header("基础控制")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float spritingSpeed;
    [SerializeField] private float crouchSpeed;
    [SerializeField] private float jumpHeight;
    [SerializeField] private float fallSpeed;//落地速度
    [SerializeField] private float crouchHeight;//下蹲后的高度
    private float originHeight;//原始高度
    private bool isCrouch;
    [Header("脚底射线检测")]
    [SerializeField] private float rayDis;
    public Transform rayTransform;
    private Vector3 moveDir;
    [Header("当前移速")]
    [SerializeField]private float currentSpeed;

    private float gravity = 9.81f;


    private void Start()
    {
        cc = GetComponent<CharacterController>();
        originHeight = cc.height;
    }

    private void Update()
    {
        rayTransform.localPosition = new Vector3(rayTransform.localPosition.x,-cc.height/2-0.05f,rayTransform.localPosition.z);

        if (isGrounded())
        {
            //获取用户输入
            float x = Input.GetAxisRaw("Horizontal");
            float z = Input.GetAxisRaw("Vertical");
            //移动的方向
            moveDir = (transform.right * x + transform.forward * z).normalized;
            //冲刺判定
            if (isCrouch)
            {
                currentSpeed = crouchSpeed;
            }
            else
            {
                currentSpeed = Input.GetKey(KeyCode.LeftShift) ? spritingSpeed : moveSpeed;
            }
            //跳跃
            if (Input.GetButtonDown("Jump"))
            {
                Jump();
            }
        }
        else
        {
            //不在地面上时开始模拟重力下降
            moveDir.y -= gravity * Time.deltaTime * fallSpeed;
        }

        //下蹲 不放在isGround()情况里是让在空中时也可以下蹲
        Crouch();

        //charactercontroller不自带重力的移动，自带重力用simplemove
        cc.Move(moveDir * currentSpeed * Time.deltaTime);



        velocity = cc.velocity;
        velocity.y = 0;
        if (anim)
        {
            anim.SetFloat("velocity", velocity.magnitude,0.25f,Time.deltaTime);
        }
    }

    public bool isGrounded()
    {
        Debug.DrawLine(rayTransform.position, new Vector3(rayTransform.position.x, rayTransform.position.y - rayDis, rayTransform.position.z), Color.red, 0.1f);
        return Physics.Raycast(rayTransform.position, Vector3.down, rayDis);
    }

    private void Jump()
    {
        //anim.SetTrigger("Jump");
        moveDir.y = jumpHeight;
    }

    private void Crouch()
    {

        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.LeftControl))
        {
            isCrouch = true;
        }
        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            isCrouch = false;
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            isCrouch = !isCrouch;
        }

        if (isCrouch)
        {
            cc.height = crouchHeight;
        }
        else
        {
            cc.height = originHeight;
        }
    }

    public void SetUpAnimator(Animator animator)
    {
        anim = animator;
    }
}
