using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;

public class BezierDraw : MonoBehaviour
{
    public Transform[] points;
    public int resolution = 20;

    private LineRenderer lineRenderer;
    private Spline spline;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = resolution + 1;

        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;

        spline = new Spline();
        spline.Add(new BezierKnot());
        spline.Add(new BezierKnot());

    }

    // Update is called once per frame
    void Update()
    {
        UpdateSplineKnot();
        DrawSpline();
    }
    void UpdateSplineKnot()
    {
        //P0
        Debug.Log("P0");
        //var knot1 = new BezierKnot(points[0].position);
        var knot1 = new BezierKnot();
        knot1.Position = (float3)points[0].position;
        knot1.TangentOut = (float3)(points[1].position - points[0].position); //Vector huong ra
        spline[0] = knot1;

        //P4
        Debug.Log("P4");
        //var knot2 = new BezierKnot(points[3].position);
        var knot2 = new BezierKnot();
        knot2.Position = (float3)points[3].position;
        knot2.TangentIn = (float3)(points[2].position - points[3].position);
        spline[1] = knot2;
    }
    void DrawSpline()
    {
        Debug.Log("Draw Spline");
        for (int i = 0; i <= resolution; i++)
        {
            float t = i / (float)resolution;
            Vector3 pos = (Vector3)SplineUtility.EvaluatePosition(spline, t);
            lineRenderer.SetPosition(i, pos);
        }
    }
}
