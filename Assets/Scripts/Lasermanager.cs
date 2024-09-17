using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lasermanager : MonoBehaviour
{
    static public Lasermanager instance;

    public GameObject LinePrefab;
    List<Lasergun> lasers = new List<Lasergun>();
    List<GameObject> lines = new List<GameObject>();

    float maxStepDistance = 20;

    public void AddLaser(Lasergun laser) {  lasers.Add(laser); }

    public void RemoveLaser(Lasergun laser) { lasers.Remove(laser); }

    //для удаления кучи клонов линии, то есть при отражении было 2 луча
    void RemoveOldLines(int linesCount)
    {
        if (linesCount < lines.Count)
        {
            Destroy(lines[lines.Count - 1]);
            lines.RemoveAt(lines.Count - 1);
            RemoveOldLines(linesCount);
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        int linesCount = 0;
        //определение стартовой позиции и направления
        foreach(Lasergun laser in lasers)
        {
            linesCount += CalcLaserLine(laser.transform.position + laser.transform.forward * 0.6f, laser.transform.forward, linesCount);
        }
        RemoveOldLines(linesCount);
    }

    //добавление луча, 
    int CalcLaserLine(Vector3 startPosition, Vector3 direction, int index)
    {
        int result = 1;
        RaycastHit hit;
        Ray ray = new Ray(startPosition, direction);
        bool intersect = Physics.Raycast(ray, out hit, maxStepDistance);

        Vector3 hitPosision = hit.point;
        if (!intersect) { hitPosision = startPosition + direction * maxStepDistance; }

        Vector3 intersectPoint;
        int intersectIndex;
        if (checkLaserIntersect(startPosition, hitPosision, index, out intersectPoint, out intersectIndex)) //проверка пересечения лучей
        {
            DrawLine(startPosition, intersectPoint, index);
            LineRenderer firstLine = lines[index].GetComponent<LineRenderer>();
            LineRenderer secondLine = lines[intersectIndex].GetComponent<LineRenderer>();
            DrawLine(secondLine.GetPosition(0), intersectPoint, intersectIndex);
            Vector3 pos = Vector3.Normalize(secondLine.GetPosition(1)- secondLine.GetPosition(0)) + Vector3.Normalize(firstLine.GetPosition(1) - firstLine.GetPosition(0));
            result += CalcLaserLine(intersectPoint, pos, index + result);
        }
        else if (intersect) //проверка пересечения объектов
        {
            DrawLine(startPosition, hitPosision, index);
            result += CalcLaserLine(hitPosision, Vector3.Reflect(direction, hit.normal), index + result);
        } else
        {
            DrawLine(startPosition, hitPosision, index);
        }
        return result;
    }

    bool checkLaserIntersect(Vector3 startPosition, Vector3 finishPosision, int index, out Vector3 intersectPoint, out int intersectIndex)
    {
        intersectPoint = Vector3.zero;
        bool isIntersect = false;
        intersectIndex = 0;
        for (int i = 0; i<index; i++)
        {
            LineRenderer line = lines[i].GetComponent<LineRenderer>();
            isIntersect = GetIntersectPointCoords(startPosition, finishPosision, line.GetPosition(0), line.GetPosition(1), out intersectPoint);
            if (isIntersect)
            {
                return true;
            }
        }
        return false;
    }

    bool GetIntersectPointCoords(Vector3 A1, Vector3 A2, Vector3 B1, Vector3 B2, out Vector3 intersectPoint)
    {
        intersectPoint = Vector3.zero;

        float a1 = A1.z - A2.z;
        float b1 = A2.x - A1.x;
        float c1 = A1.x * A2.z - A2.x * A1.z;

        float a2 = B1.z - B2.z;
        float b2 = B2.x - B1.x;
        float c2 = B1.x * B2.z - B2.x * B1.z;

        float det = a1 * b2 - a2 * b1;
        if (det == 0)
        {
            return false;
        }

        intersectPoint = new Vector3((b1 * c2 - b2 * c1) / det, A1.y, (a2 * c1 - a1 * c2) / det);
        return pointBetweenAB(A1, A2, intersectPoint) && pointBetweenAB(B1, B2, intersectPoint);
    }

    bool pointBetweenAB(Vector3 A, Vector3 B, Vector3 C)
    {
        return Vector3.Dot((B - C).normalized, (A - C).normalized) < 0f;
    }

    //отображение линии
    void DrawLine(Vector3 startPosition, Vector3 finishPosition, int index)
    {
        LineRenderer line = null;
        if (index < lines.Count)
        {
            line = lines[index].GetComponent<LineRenderer>();
        } else
        {
            GameObject go = Instantiate(LinePrefab, Vector3.zero, Quaternion.identity);
            line = go.GetComponent<LineRenderer>();
            lines.Add(go);
        }
        line.SetPosition(0, startPosition);
        line.SetPosition(1, finishPosition);
    }
}
