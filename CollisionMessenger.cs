using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UniRx;

public class CollisionMessenger : MonoBehaviour
{
    public enum names
    {
        player,
        enemy,
    }

    [SerializeField]
    private names myName;

    private Subject<names> _isCollide = new Subject<names>();

    public IObservable<names> isCollide
    {
        get {return  _isCollide; }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collisionInfo)
    {
        Debug.Log(myName + "messenger collide");

        if(collisionInfo.gameObject.CompareTag("Obstacle"))
        {
            _isCollide.OnNext(myName); 
        }
    }
}
