using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeManager : MonoBehaviour
{
    public enum MoveDirection { Left, Right, Up, Down };
    public enum RotationDirection { Clockwise, CounterClockwise };

    public delegate void MoveDelegate(MoveDirection direction);
    public MoveDelegate moveElement;

    public delegate void RotateDelegate(RotationDirection direction);
    public RotateDelegate rotateEvent;

    private List<Vector2> points = new List<Vector2>();
    private List<Vector2> drawPoints = new List<Vector2>();

    public Material lineMaterial;

    private const float POINT_STEP = 10;
    private const float POINT_SIZE = 5;
    private const float MIN_ANGLE_SUM = 200;

    private Vector2 curvCenter;
    private Vector2 curvRadius;
    private float curvAngleSum;

    public static SwipeManager instance;

    private void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 pos = Input.mousePosition;
        Vector2 drawPos = pos;
        drawPos.y = Screen.height - drawPos.y;
        //start
        if (Input.GetMouseButtonDown(0))
        {
            points.Clear();
            drawPoints.Clear();
            points.Add(pos);
            drawPoints.Add(drawPos);
        }
        //move
        if(Input.GetMouseButton(0)) {
            if (Vector2.Distance(points[points.Count-1], pos) > POINT_STEP)
            {
                points.Add(pos);
                drawPoints.Add(drawPos);
            }
        }
        //finish
        if (Input.GetMouseButtonUp(0))
        {
            DetectSwipe();
        }
    }

    private void DetectSwipe()
    {
        CalcStats();

        if(curvAngleSum > MIN_ANGLE_SUM)
        {
            //Rotation
            float sum = 0;
            for (int i = 0; i < points.Count - 1; i++)
            {
                sum += (points[i+1].x - points[i].x) * (points[i+1].y + points[i].y);
            }
            sum += (points[0].x - points[points.Count - 1].x) * (points[0].y + points[points.Count - 1].y);

            RotationDirection rotDirection = sum > 0 ? RotationDirection.Clockwise : RotationDirection.CounterClockwise;
            rotateEvent?.Invoke(rotDirection);
        } else
        {
            //move
            Vector2 swipeDelta = points[points.Count - 1] - points[0];
            if (Mathf.Abs(swipeDelta.x) < 5 && Mathf.Abs(swipeDelta.y) < 5)
                return;

            moveElement?.Invoke(Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y) ?
                                swipeDelta.x < 0 ? MoveDirection.Left : MoveDirection.Right :
                                swipeDelta.y < 0 ? MoveDirection.Down : MoveDirection.Up);
        }
        
    }

    private void CalcStats()
    {
        if (points.Count < 2) return;

        //Calc Center
        curvCenter = Vector2.zero;
        foreach(Vector2 p in points)
        {
            curvCenter += p;
        }
        curvCenter /= points.Count;

        //calc min max radius
        Vector2 radius;
        float length;
        curvRadius.x = 1000000;
        curvRadius.y = 0;
        for (int i = 0; i < points.Count; i++) {
            radius = points[i] - curvCenter;
            length = radius.magnitude;
            curvRadius.x = length < curvRadius.x ? length : curvRadius.x;
            curvRadius.y = length > curvRadius.y ? length : curvRadius.y;
        }

        //calc angles
        curvAngleSum = 0;
        for (int i = 1; i < points.Count; i++)
        {
            curvAngleSum += Vector2.Angle(points[i-1] - curvCenter, points[i] - curvCenter);
        }
    }

    private void OnGUI()
    {
        if (Event.current.type != EventType.Repaint) return;

        lineMaterial.SetPass(0);

        GL.PushMatrix();
        DrawPoints();
        GL.PopMatrix();

        GUI.Label(new Rect(10, 10, 500, 25), "Center: " + curvCenter);
        GUI.Label(new Rect(10, 30, 500, 25), "Radius: " + curvRadius.x + "..." + curvRadius.y);
        GUI.Label(new Rect(10, 50, 500, 25), "Angle: " + curvAngleSum);
    }

    private void DrawPoints()
    {
        if (drawPoints.Count < 2) return;

        curvCenter = drawPoints[0];

        for (int i=1; i < drawPoints.Count; i++)
        {
            DrawLine(drawPoints[i - 1], drawPoints[i]);
            DrawPoint(drawPoints[i-1]);
            curvCenter += drawPoints[i];
        }
        curvCenter /= points.Count;
        DrawPoint(curvCenter);
    }

    private void DrawPoint(Vector2 p)
    {
        GL.Begin(GL.TRIANGLES);
        GL.Vertex(p + new Vector2(-POINT_SIZE, POINT_SIZE / 2));
        GL.Vertex(p + new Vector2(POINT_SIZE, POINT_SIZE / 2));
        GL.Vertex(p + new Vector2(0, -POINT_SIZE));
        GL.End();
    }

    private void DrawLine(Vector2 p1, Vector2 p2)
    {
        GL.Begin(GL.LINES);
        GL.Vertex(p1);
        GL.Vertex(p2);
        GL.End();
    }
}
