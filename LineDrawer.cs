using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineDrawer : MonoBehaviour
{
    [SerializeField]
    private GameObject hand;

    [SerializeField]
    private float fixXAmount;

    [SerializeField]
    private float fixYAmount;

    [SerializeField]
    private float zPos;

    [SerializeField]
    private PlayerMover playerMover;

    private LineRenderer lineRenderer;

    private int positionCount;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.useWorldSpace = true;

        positionCount = 0;
    }

    void Start()
    {
        Time.timeScale = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S) && Time.timeScale < 1)
        {
            Time.timeScale = 1;
        }
        
        if(playerMover.isCollide) return;

        if (Input.GetMouseButton(0))
        {
            var mouseInput = Input.mousePosition;

            hand.SetActive(true);

            //Debug.Log(mouseInput);

            if (773 < mouseInput.x)
            {
                mouseInput.x = 773;
            }
            else if (mouseInput.x < 330)
            {
                mouseInput.x = 330;
            }

            if (310 < mouseInput.y)
            {
                mouseInput.y = 310;
            }
            else if (mouseInput.y < 72)
            {
                mouseInput.y = 72;
            }

            mouseInput = new Vector3(mouseInput.x, mouseInput.y, 1);

            mouseInput = Camera.main.ScreenToViewportPoint(mouseInput);

            var xDirection = float.Parse(mouseInput.x.ToString("f2")) - 0.5f;

            var fixXPos = xDirection * fixXAmount;

            var yDirection = float.Parse(mouseInput.y.ToString("f2"));

            var fixYPos = yDirection * fixYAmount;

            //Debug.Log("mouseInput.x - 0.5f = " + yDirection);

            //Debug.Log(fixYPos);

            transform.position = new Vector3(mouseInput.x + fixXPos, mouseInput.y + 49.5f + fixYPos, zPos);

            positionCount++;
            lineRenderer.positionCount = positionCount;
            lineRenderer.SetPosition(positionCount - 1, transform.position);

            lineRenderer.enabled = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            positionCount = 0;

            lineRenderer.enabled = false;

            hand.SetActive(false);

            SoundManager.i.PlayOneShot(1, 1);
        }
    }
}
