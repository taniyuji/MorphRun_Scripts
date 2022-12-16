using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UniRx;
using DG.Tweening;

public class PlayerMover : MonoBehaviour
{
    private float moveSpeed = 1.5f;

    private Vector3 hitBackAmount = new Vector3(0, 3f, 3f);

    private float beHitTime = 0.6f;

    private CollisionInformationProvider collisionInformationProvider;

    private Rigidbody _rigidbody;

    public bool isCollide { get; private set; } = false;

    private Vector3 moveVector;

    private float defaultYPosition;

    void Awake()
    {
        collisionInformationProvider = GetComponent<CollisionInformationProvider>();

        _rigidbody = GetComponent<Rigidbody>();

        moveVector = new Vector3(0, 0, -moveSpeed);

        defaultYPosition = transform.position.y;
    }

    void OnCollisionEnter(Collision collisionInfo)
    {
        if (collisionInfo.gameObject.CompareTag("Obstacle") && !isCollide)
        {
            StartCoroutine(DamagedBehavior());

            if (gameObject.CompareTag("Player"))
            {
                SoundManager.i.PlayOneShot(UnityEngine.Random.Range(3, 6), 1);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isCollide) _rigidbody.velocity = moveVector;
    }

    private IEnumerator DamagedBehavior()
    {
        isCollide = true;

        _rigidbody.velocity = Vector3.zero;

        _rigidbody.AddForce(new Vector3(0, 0, hitBackAmount.z), ForceMode.Impulse);

        yield return new WaitForSeconds(beHitTime);

        //Debug.Log("collide");
        isCollide = false;
    }

}
