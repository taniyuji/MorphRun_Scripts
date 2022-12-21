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

    private Vector3 moveVector;

    private float defaultYPosition;

    private Subject<Unit> _isGoal = new Subject<Unit>();

    public IObservable<Unit> isGoal
    {
        get { return _isGoal; }
    }

    public enum PlayerState
    {
        Running,
        IsCollide,
        IsGoal,
    }

    public PlayerState state { get; private set; } = PlayerState.Running;

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();

        moveVector = new Vector3(0, 0, -moveSpeed);

        defaultYPosition = transform.position.y;
    }

    void OnCollisionEnter(Collision collisionInfo)
    {
        if (collisionInfo.gameObject.CompareTag("Obstacle") && state != PlayerState.IsCollide)
        {
            StartCoroutine(DamagedBehavior());//障害物と衝突した場合の挙動

            if (gameObject.CompareTag("Player"))
            {
                SoundManager.i.PlayOneShot(UnityEngine.Random.Range(3, 6), 1);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Goal"))
        {
            _isGoal.OnNext(Unit.Default);

            state = PlayerState.IsGoal;
        }
    }

    void FixedUpdate()
    {
        if (state == PlayerState.Running) _rigidbody.velocity = moveVector;
        else if (state == PlayerState.IsGoal && moveVector.z < 0)
        {
            moveVector.z += 0.01f;

            _rigidbody.velocity = moveVector;
        }
    }

    private IEnumerator DamagedBehavior()
    {
        state = PlayerState.IsCollide;

        _rigidbody.velocity = Vector3.zero;

        //後ろにノックバックさせる
        _rigidbody.AddForce(new Vector3(0, 0, hitBackAmount.z), ForceMode.Impulse);

        yield return new WaitForSeconds(damagedTime);

        //Debug.Log("collide");
        state = PlayerState.Running;
    }

}
