using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
    public float bulletSpeed;
    Rigidbody rb;
    TrailRenderer trailRenderer;
    Collider coll;

    private void Awake()
    {
        Invoke("DestoryBullet", 10f);
        rb = GetComponent<Rigidbody>();
        trailRenderer = GetComponent<TrailRenderer>();
        coll = GetComponent<Collider>();
    }
    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.P))
        {
            Time.timeScale = 0;
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            Time.timeScale = 1;
        }
        
    }

    void DestoryBullet()
    {
        ObjectPoolManager.Instance.DeSpawn(gameObject);
        trailRenderer.Clear();
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("coll:"+collision.collider.name);
        rb.velocity = Vector3.zero;
        trailRenderer.Clear();
    }


    private void OnEnable()
    {
        rb.AddForce(transform.forward * bulletSpeed, ForceMode.Impulse);
        Invoke("DestoryBullet", 10f);
    }

    private void OnDisable()
    {
        rb.velocity = Vector3.zero;
    }
}
