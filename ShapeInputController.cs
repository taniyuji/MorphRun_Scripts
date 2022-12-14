using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ShapeInputController : MonoBehaviour
{

    [SerializeField]
    private Transform drawPointerTransform;

    public Vector3 getPointerPosition
    {
        get { return drawPointerTransform.position; }
    }

    public Vector3 getPointerDownPosition { get; private set; }

    private DrawPhysicsLine drawer;

    private PlayerMover playerMover;

    private CinemachineTransposer transposer;


    void Awake()
    {
        drawer = GetComponent<DrawPhysicsLine>();

        playerMover = GetComponent<PlayerMover>();

        transposer = ResourceProvider.i.cineCam.GetCinemachineComponent<CinemachineTransposer>();
    }

    // Update is called once per frame
    void Update()
    {
        GetDrawVector();
    }

    private void GetDrawVector()
    {
        if (!Input.GetMouseButtonDown(0) && !Input.GetMouseButton(0)) return;

        if (playerMover.isCollide) return;

        var mouseInput = Input.mousePosition;

        //Debug.Log(mouseInput);

        mouseInput = new Vector3(mouseInput.x, mouseInput.y, transposer.m_FollowOffset.z);

        var fixedMouseInput = Camera.main.ScreenToWorldPoint(mouseInput);

        //Debug.Log(fixX);

        drawPointerTransform.position = new Vector3(fixedMouseInput.x, fixedMouseInput.y, drawPointerTransform.position.z);

        drawPointerTransform.localPosition = new Vector3(drawPointerTransform.localPosition.x,
                                                         drawPointerTransform.localPosition.y,
                                                         drawPointerTransform.localPosition.z);

        if (Input.GetMouseButtonDown(0))
        {
            drawer.PenDown(drawPointerTransform.localPosition);

            getPointerDownPosition = drawPointerTransform.localPosition;
        }

        if (Input.GetMouseButton(0))
        {
            drawer.PenMove(drawPointerTransform.localPosition);
        }

    }
}
