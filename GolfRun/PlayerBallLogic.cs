using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerBallLogic : MonoBehaviour
{
    public float pow = 10;
    public Vector2 minPow, maxPow;
    public float speedGate = 0.01f;
    public bool roundOver = false;
    public float roundEndTimer = 5.0f;
    public TextMeshProUGUI textMesh;

    bool shotCancel = false;

    private int _hitCount = 0;

    
    Camera cam;
    public Rigidbody2D rigB;
    Vector2 forceApplied;

    Vector3 startP, endP;

    LineView lv;

    MouseFollow mouseWeight;


    private void Start()
    {
        cam = Camera.main;
        lv = GetComponent<LineView>();
        mouseWeight = GameObject.FindGameObjectWithTag("MouseCursorFollow").GetComponent<MouseFollow>();
        textMesh = GameObject.FindGameObjectWithTag("ScoreCounter").gameObject.GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) 
        {
            Application.Quit();
        }

        if (Input.GetKeyDown(KeyCode.R)) 
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if (rigB.velocity.magnitude <= speedGate && roundOver != true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                startP = cam.ScreenToWorldPoint(Input.mousePosition);
                startP.z = 0;
                shotCancel = false;
            }

            if (Input.GetMouseButton(0))
            {
                if (!shotCancel)
                {
                    Vector3 currP = cam.ScreenToWorldPoint(Input.mousePosition);
                    currP.z = 0;
                    mouseWeight.addVec = startP - currP;
                    Vector3 clampedCurrP = new Vector3(Mathf.Clamp(startP.x - currP.x, minPow.x, maxPow.x), Mathf.Clamp(startP.y - currP.y, minPow.y, maxPow.y), 0);

                    float magHol = clampedCurrP.magnitude;
                    if (magHol > maxPow.x)
                    {
                        clampedCurrP.Normalize();
                        clampedCurrP *= maxPow.x;
                    }
                    lv.RenderLine(transform.position, transform.position - clampedCurrP);

                    if (Input.GetMouseButtonDown(1))
                    {
                        endP = cam.ScreenToWorldPoint(Input.mousePosition);
                        endP.z = 0;
                        mouseWeight.addVec = Vector3.zero;
                        lv.EndLine();
                        shotCancel = true;
                    }
                }
            }


            if (Input.GetMouseButtonUp(0))
            {
                if (shotCancel == false)
                {
                    endP = cam.ScreenToWorldPoint(Input.mousePosition);
                    endP.z = 0;
                    mouseWeight.addVec = Vector3.zero;
                    lv.EndLine();
                    forceApplied = new Vector2(Mathf.Clamp(startP.x - endP.x, minPow.x, maxPow.x), Mathf.Clamp(startP.y - endP.y, minPow.y, maxPow.y));

                    float magHol = forceApplied.magnitude;
                    if (magHol > maxPow.x)
                    {
                        forceApplied.Normalize();
                        forceApplied *= maxPow.x;
                    }
                    rigB.AddForce(forceApplied * pow, ForceMode2D.Impulse);
                    _hitCount++;
                    textMesh.text = _hitCount.ToString();
                }
            }
            else
            {
                rigB.velocity -= rigB.velocity.normalized * 0.5f * Time.deltaTime; ;
            }
        }
        else if (rigB.velocity.magnitude > speedGate)
        {
            shotCancel = true;
        }
        else if (roundOver == true)
        {
            if ((roundEndTimer -= Time.deltaTime) <= 0)
            {
                if (SceneManager.GetActiveScene().name == "TestingLevel")
                {
                    SceneManager.LoadScene("TestingLevel2");
                }
                else if (SceneManager.GetActiveScene().name == "TestingLevel2")
                {
                    SceneManager.LoadScene("TestingLevel3");
                }
                else if (SceneManager.GetActiveScene().name == "TestingLevel3")
                {
                    SceneManager.LoadScene("TestingLevel4");
                }
                else if (SceneManager.GetActiveScene().name == "TestingLevel4")
                {
                    SceneManager.LoadScene("TestingLevel");
                }
            }    

        }    
    }
}
