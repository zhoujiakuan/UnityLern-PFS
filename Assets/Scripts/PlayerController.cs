using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private Animator anim;
    private CharacterController cc;
    private const float Gravity = -9.81f;
    [Header("基础控制")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float spritingSpeed;
    [SerializeField] private float jumpHeight;
    [SerializeField] private float fallSpeed;//落地速度
    [SerializeField] private int jumpTimes = 2;
    private float currentSpeed;
    public Transform cam;
    private float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;

    private float gravity = -9.81f;
    private Vector3 velocity;

    [Header("地面检测")]
    public Transform groundCheck1;
    public Transform groundCheck2;
    public float checkRange = 0.1f;
    public LayerMask groundMask;
    private bool isGround;


    private void Start()
    {
        cam = Camera.main.transform;
        anim = GetComponent<Animator>();
        cc = GetComponent<CharacterController>();

    }

    private void Update()
    {
        isGround = Physics.CheckSphere(groundCheck1.position,checkRange,groundMask) || Physics.CheckSphere(groundCheck2.position, checkRange, groundMask);

        if(isGround && velocity.y < 0)
        {
            jumpTimes = 2;
            velocity.y = -1;
            anim.SetBool("Jump",false);
        }

        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(x,0,z).normalized;
        currentSpeed = Input.GetKey(KeyCode.LeftShift) ? spritingSpeed : moveSpeed;
        //移动
        if (direction.magnitude > 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;//获得旋转角度

            //平滑旋转角色的朝向
            float Angle = Mathf.SmoothDampAngle(transform.eulerAngles.y,targetAngle,ref turnSmoothVelocity,turnSmoothTime);
            transform.rotation = Quaternion.Euler(0,Angle,0);

            Vector3 moveDir = Quaternion.Euler(0,targetAngle,0) * Vector3.forward;
            cc.Move(moveDir.normalized * currentSpeed * Time.deltaTime);
        }

        if(Input.GetButtonDown("Jump") && isGround)
        {
            jumpTimes--;
            Jump();
        }
        if(Input.GetButtonDown("Jump") && jumpTimes > 0 && !isGround)
        {
            jumpTimes--;
            Jump();
        }


        velocity.y += gravity * Time.deltaTime * fallSpeed;
        cc.Move(velocity * Time.deltaTime);

        anim.SetFloat("Speed", direction.magnitude * currentSpeed);
    }


    private void Jump()
    {
        anim.SetBool("Jump", true);
        velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }


}
