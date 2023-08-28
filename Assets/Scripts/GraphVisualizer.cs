using Cysharp.Threading.Tasks;
using Shapes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Polyline))]
public class GraphVisualizer : MonoBehaviour
{
    private Polyline polyline;
    private float drawSpeed = 3.2f;
    private Vector3[] targetPoints;
    private Vector3 startingPosition;
    private Vector3 currentPosition;

    private void Initialize()
    {
        polyline = GetComponent<Polyline>();

        startingPosition = targetPoints[0];
        currentPosition = startingPosition;
        polyline.points.Clear();
        polyline.AddPoint(startingPosition);
        polyline.AddPoint(startingPosition);
    }

    public async UniTask StartDrawing(params Vector3[] pos)
    {
        targetPoints = new Vector3[pos.Length];
        for (int i = 0; i < targetPoints.Length; i++)
        {
            targetPoints[i] = pos[i];
        }

        Initialize();
        await DrawLine(targetPoints);
    }

    private void DrawDefaultPoints()
    {
        targetPoints = new Vector3[polyline.points.Count];
        for (int i = 0; i < polyline.points.Count; i++)
        {
            targetPoints[i] = polyline.points[i].point;
        }
    }

    private async UniTask DrawLine(params Vector3[] pos)
    {
        int currentIdx = 1;

        while (true)
        {
            float dist = GetDistance(currentPosition, pos[currentIdx]);

            if (dist <= 0.02)
            {
                currentIdx++;
                if (currentIdx >= pos.Length)
                    break;
                polyline.AddPoint(pos[currentIdx]);
            }
            var nextPosition = drawSpeed * (pos[currentIdx] - currentPosition).normalized * Time.deltaTime + currentPosition;
            polyline.SetPointPosition(currentIdx, nextPosition);

            currentPosition = nextPosition;

            await UniTask.Yield();
        }
        Debug.Log("Finished");

        return;
    }

    private float GetDistance(Vector3 pos1, Vector3 pos2)
    {
        return Vector3.Distance(pos1, pos2);
    }
}
