using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UniRx;
using DG.Tweening;

//Playerの動きを制御するスクリプト
public class PlayerMover : MonoBehaviour
{
    private float moveSpeed = 2f;

    private Vector3 hitBackAmount = new Vector3(0, 3f, 3f);

    private float damagedTime = 0.6f;

    private Rigidbody _rigidbody;

    public bool isCollide { get; private set; } = false;

    private Vector3 moveVector;

    private float defaultYPosition;

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();

        moveVector = new Vector3(0, 0, -moveSpeed);

        defaultYPosition = transform.position.y;
    }

    void OnCollisionEnter(Collision collisionInfo)
    {
        if (collisionInfo.gameObject.CompareTag("Obstacle") && !isCollide)
        {
            StartCoroutine(DamagedBehavior());//障害物と衝突した場合の挙動

            if (gameObject.CompareTag("Player"))
            {
                SoundManager.i.PlayOneShot(UnityEngine.Random.Range(3, 6), 1);
            }
        }
    }

    void FixedUpdate()
    {
        if (!isCollide) _rigidbody.velocity = moveVector;
    }

    private IEnumerator DamagedBehavior()
    {
        isCollide = true;

        _rigidbody.velocity = Vector3.zero;

        //後ろにノックバックさせる
        _rigidbody.AddForce(new Vector3(0, 0, hitBackAmount.z), ForceMode.Impulse);

        yield return new WaitForSeconds(damagedTime);

        //Debug.Log("collide");
        isCollide = false;
    }

}
