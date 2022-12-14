using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeInputController : MonoBehaviour
{

    [SerializeField]
    private Transform drawPointerTransform;

    [SerializeField]
    private float addXPos;

    [SerializeField]
    private float addYPos;

    public Vector3 getPointerPosition
    {
        get { return drawPointerTransform.position; }
    }

    public Vector3 getPointerDownPosition { get; private set; }

    private float fixXAmount = 2.88f;

    private int mouseX0Pos = 530;

    private int mouseY0Pos = 200;

    private DrawPhysicsLine drawer;

    private PlayerMover playerMover;


    void Awake()
    {
        drawer = GetComponent<DrawPhysicsLine>();

        playerMover = GetComponent<PlayerMover>();
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

        mouseInput = new Vector3(mouseInput.x, mouseInput.y, 1);

        var fixedMouseInput = Camera.main.ScreenToWorldPoint(mouseInput);

        float fixX = fixXAmount + (mouseX0Pos - mouseInput.x) / 400;

        float fixY = (mouseInput.y - mouseY0Pos) / 400;

        //Debug.Log(fixX);

        drawPointerTransform.position = new Vector3(fixedMouseInput.x, fixedMouseInput.y, drawPointerTransform.position.z);

        drawPointerTransform.localPosition = new Vector3(drawPointerTransform.localPosition.x + fixX + addXPos,
                                                         drawPointerTransform.localPosition.y + fixY + addYPos,
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
