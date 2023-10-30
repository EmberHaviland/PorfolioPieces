using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MouseFollow : MonoBehaviour
{
    Camera cam;
    public CinemachineVirtualCamera virtCam;
    public Vector3 addVec = Vector3.zero;
    public float minZoom = 2.5f;
    public float maxZoom = 20f;
    private float ZoomSpeed = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Vector3 tempVec = cam.ScreenToWorldPoint(Input.mousePosition) + addVec * 2;
            // = GameObject.FindGameObjectWithTag("Player").transform.position + addVec;
            transform.position = tempVec;
        }
        else
        {
            transform.position = cam.ScreenToWorldPoint(Input.mousePosition);
        }

        virtCam.m_Lens.OrthographicSize = Mathf.Clamp((virtCam.m_Lens.OrthographicSize - (Input.mouseScrollDelta.y * ZoomSpeed)), minZoom, maxZoom);
    }
}
