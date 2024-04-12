using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(EdgeCollider2D))]

public class FloorHandler : MonoBehaviour
{
    // Start is called before the first frame update
    public LineRenderer LineRenderer;
    public LineRenderer goalHeight;
    private EdgeCollider2D collider;
    private MeshRenderer meshR;
    private MeshFilter meshF;
    private CircleCollider2D ballColl;
    private Rigidbody2D rb;
    public int pointsEachSide;
    private GameObject ball;
    private GameObject sinBgMesh;
    private GameObject bg;
    private GameObject mainCam;
    private TMP_Text ScoreLabel;
    private TMP_Text heightShower;
    private Camera cam;
    private int x = 10;
    public float shift = 0;
    public float frequency = 0.4f;
    private float[] randFreqs = { 0.5f, 0.3f, 0.7f };
    public float height = 3f;
    private float Heightthreshold = 0.1f;
    private int velocityReduction = 500;
    private Mesh mesh;
    void Start()
    {
        LineRenderer = GetComponent<LineRenderer>();
        goalHeight = GameObject.Find("GoalHeight").GetComponent<LineRenderer>();
        collider = GetComponent<EdgeCollider2D>();
        ball = GameObject.Find("Ball");
        mainCam = GameObject.Find("MainCam");
        sinBgMesh = GameObject.Find("SinWaveBgMesh");
        ScoreLabel = GameObject.Find("ScoreLabel").GetComponent<TMP_Text>();
        heightShower = GameObject.Find("HeightShower").GetComponent<TMP_Text>();
        bg = GameObject.Find("bigBg");
        cam = mainCam.GetComponent<Camera>();
        ballColl = ball.GetComponent<CircleCollider2D>();
        rb = ball.GetComponent<Rigidbody2D>();
        meshR = sinBgMesh.GetComponent<MeshRenderer>();
        meshF = sinBgMesh.GetComponent<MeshFilter>();
        initScreen();
    }

    void initScreen()
    {
        mesh = new Mesh();
        meshF.mesh = mesh;
        meshF.mesh.name = "SinBgMesh";
        ScoreLabel.text = "Score: 0";
        Draw();
    }

    private Vector2 leftBnd = new Vector2(0, 0);
    private Vector2 rightBnd = new Vector2(1, 0);
    public float maxVelocity = 0.5f;

    private void Draw()
    {
        ball.transform.position = new Vector3(0, ball.transform.position.y, 0);
        if (rb.velocity.x > 0)
        {
            shift -= Mathf.Min(rb.velocity.x / velocityReduction, maxVelocity);

        }
        else
        {
            shift -= Mathf.Max(rb.velocity.x / velocityReduction, -maxVelocity);

        }
        if(shift > 500)
        {
            shift = shift % ((2 * Mathf.PI) / frequency);
        }
        
        float leftXBound = x - cam.ViewportToWorldPoint(leftBnd).x;
        float rightXBound = x - cam.ViewportToWorldPoint(rightBnd).x;

        goalHeight.transform.position = new Vector3(leftXBound-x, 10, 0);
        goalHeight.transform.localScale = new Vector3(1, 1, rightXBound - leftXBound);
        
        pointsEachSide = (int)Mathf.Round(cam.orthographicSize * 3.5f);

        bool isAboveHeightThreshold = false;
        bool atPeak = true;
        float lastHeight = Mathf.Sin(-shift);
        LineRenderer.positionCount = pointsEachSide * 2;
        for (int l = 0; l < pointsEachSide; l++)
        {
            float progress = (float)l / (pointsEachSide - 1);
            float xL = Mathf.Lerp(leftXBound, x, progress);
            float yL = specialSin(xL-shift);
            yL *= height;
            

            // left positions first
            LineRenderer.SetPosition(l, new Vector3(xL, yL, 0));
            //Debug.Log("Point #" + l + " x y coords: " + xL + "," + yL);

        }
        for (int r = 0; r < pointsEachSide; r++)
        {
            
            float progress = (float)r / (pointsEachSide - 1);
            float xR = Mathf.Lerp(x, rightXBound, progress);
            float yR = specialSin(xR-shift);
            yR *= height;
            //isAboveHeightThreshold = yR > height - Heightthreshold;
            //if (isAboveHeightThreshold && atPeak)
            //{
            //    frequency = getRandFreq();
            //    atPeak = false;
            //}

            ////reset atpeak
            //atPeak = yR < 0;
            // right positions next
            LineRenderer.SetPosition(r + pointsEachSide, new Vector3(xR, yR, 0));
            // Debug.Log("Point #" + (r+pointsEachSide) + " x y coords: " + xR + "," + yR);


        }
        LineRenderer.transform.localRotation = Quaternion.identity;
        updateColliderAndRenderer();
    }
    private bool isTapInProgress;

    float specialSin(float x)
    {
        int pattern =(int) x % 4;
        switch (pattern)
        {
            case 0:
                return Mathf.Sin(frequency * x);
            case 1:
                return Mathf.Sin(frequency * x);
            case 2:
                return Mathf.Sin(frequency * x);
            case 3:
                return Mathf.Sin(frequency * x);
        }
        // shoud never reach this case
        return Mathf.Sin(frequency*x);
    }

    // Update is called once per frame
    private int numHighJumps = 0;
    private bool highAir = false;

    private Color32 orange = new Color32(255, 74, 28,255);
    private Color32 red  = new Color32(221, 45, 74,255);

    void Update()
    {
        float xForce = collider.IsTouchingLayers() ? 0.2f : 0.0005f;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            rb.AddForce(Vector2.left * xForce, ForceMode2D.Impulse);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            rb.AddForce(Vector2.right * xForce, ForceMode2D.Impulse);

        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            rb.AddForce(Vector2.left * xForce, ForceMode2D.Impulse);

        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            rb.AddForce(Vector2.right * xForce, ForceMode2D.Impulse);

        }
        if (!collider.IsTouchingLayers())
        {
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))
            {
                rb.AddForce(Vector2.down * 0.15f, ForceMode2D.Impulse);

            }
        }

        if (ball.transform.position.y > 10 && !highAir)
        {
            numHighJumps++;
            ScoreLabel.text = "Score: " + numHighJumps;
            highAir = true;
        }
        if (ball.transform.position.y < 10)
        {
            highAir = false;
        }




        // Check for tap release

        float ballY = ball.transform.position.y;
        cam.orthographicSize = ballY + 9;
        float heightC = cam.orthographicSize - 3.05f - height;
        mainCam.transform.position = new Vector3(0, heightC, -10);
        if(ballY > 10)
        {
            goalHeight.enabled = false;
        }
        else
        {
            goalHeight.enabled = true;
        }

        if(ballY > 10)
        {
            heightShower.enabled = true;
            // above goalheight
            if(rb.velocity.y > 0)
            {
                heightShower.fontSize = 36 + (ballY / 2);
                if(ballY > 70)
                {
                    heightShower.color = orange;
                }
                else if(ballY > 35)
                {
                    heightShower.color = red;
                }
                else
                {
                    heightShower.color = Color.white;
                }
                heightShower.text = ballY.ToString("F2");
            }
        }
        else
        {
            heightShower.enabled = false;
        }


        Draw();


    }

    private int bottom = -6;

    void updateColliderAndRenderer()
    {
        int pointCount = LineRenderer.positionCount;
        //collider stuff
        List<Vector2> edges = new List<Vector2>();

        // renderer stuff
        List<int> triangles = new List<int>();
        List<Vector3> vertices = new List<Vector3>();

        for (int i = 0; i < 2*pointCount; i +=2)
        {
            // collider stuff
            Vector3 pos = LineRenderer.GetPosition(i/2);
            Vector3 posLowered = new Vector3(pos.x, bottom, pos.z);
           // Vector3 nextPos = LineRenderer.GetPosition(i + 1);
           // Vector3 nextLowered = new Vector3(nextPos.x, bottom, nextPos.z);
            edges.Add(new Vector2(pos.x, pos.y));

            // mesh renderer stuff
            vertices.Add(pos);
           // vertices.Add(nextPos);
            vertices.Add(posLowered);
           // vertices.Add(nextLowered);
            if (i != 0)
            {
                triangles.Add(i - 2);
                triangles.Add(i - 1);
                triangles.Add(i);

                triangles.Add(i - 1);
                triangles.Add(i + 1);
                triangles.Add(i);
            }
           
           
           // Debug.Log("i: " + i + " pos x y: " + pos.x + " " + pos.y +
                     // " nextPos x y" + nextPos.x + " " + nextPos.y +
                      //" poslowered x y" + posLowered.x + " " + posLowered.y +
                      //" nextLowered x y" + nextLowered.x + " " + nextLowered.y);

        }

        collider.SetPoints(edges);

        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
    }


    


}
