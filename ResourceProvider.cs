using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

//シーン上にひとつしかないものの参照を提供するスクリプト
public class ResourceProvider : MonoBehaviour
{
    public static ResourceProvider i;

    [SerializeField]
    private CinemachineVirtualCamera _cineCam;

    public CinemachineVirtualCamera cineCam
    {
        get { return _cineCam; }
    }

    [SerializeField]
    private ShapeInputController _inputController;

    public ShapeInputController inputController
    {
        get { return _inputController; }
    }

    [SerializeField]
    private DrawPhysicsLine _drawer;

    public DrawPhysicsLine drawer
    {
        get { return _drawer; }
    }

    [SerializeField]
    private ShapeAnimation _shapeAnimation;

    public ShapeAnimation shapeAnimation
    {
        get { return _shapeAnimation; }
    }

    [SerializeField]
    private PlayerMover _playerMover;

    public PlayerMover playerMover
    {
        get { return _playerMover; }
    }

    [SerializeField]
    private SpriteRenderer _ok;

    public SpriteRenderer ok
    {
        get { return _ok; }
    }

    void Awake()
    {
        i = this;
    }
}
