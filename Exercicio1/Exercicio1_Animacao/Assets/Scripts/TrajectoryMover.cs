using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

[RequireComponent(typeof(LineRenderer))]
public class TrajectoryMover : MonoBehaviour
{
    [Header("Objeto a ser animado")]
    public Transform movingObject;

    [Header("Pontos de Controle")]
    [Tooltip("Adicione pontos na regra de 1 + 3n (ex: 4, 7, 10, 13 pontos)")]
    public List<Transform> controlPoints;

    [Header("Configurações de Animação")]
    public float speed = 0.5f;
    private float t = 0f;
    private bool movingForward = true;

    [Header("Modo de Interpolação")]
    public bool useBezier = true;

    [Header("Configurações da Linha (Visual)")]
    public int lineResolution = 30;
    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));

        UpdateLineVisuals();
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            useBezier = !useBezier;
            UpdateLineVisuals();
            Debug.Log("Modo de interpolação: " + (useBezier ? "Bézier Composta" : "Linear"));
        }

        if (controlPoints.Count < 4 || movingObject == null) return;

        if (movingForward)
        {
            t += Time.deltaTime * speed;
            if (t > 1f) { t = 1f; movingForward = false; }
        }
        else
        {
            t -= Time.deltaTime * speed;
            if (t < 0f) { t = 0f; movingForward = true; }
        }

        movingObject.position = GetTrajectoryPoint(t);

    }

    Vector3 GetTrajectoryPoint(float globalTime)
    {
        if (!useBezier)
        {
            return Vector3.Lerp(controlPoints[0].position, controlPoints[controlPoints.Count - 1].position, globalTime);
        }

        int curveCount = (controlPoints.Count - 1) / 3;
        float scaledT = globalTime * curveCount;
        int currentCurve = Mathf.FloorToInt(scaledT);

        if (currentCurve >= curveCount) currentCurve = curveCount - 1;

        float localT = scaledT - currentCurve;
        int nodeIndex = currentCurve * 3;

        return CalculateBezierPoint(localT,
            controlPoints[nodeIndex].position,
            controlPoints[nodeIndex + 1].position,
            controlPoints[nodeIndex + 2].position,
            controlPoints[nodeIndex + 3].position);
    }

    Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        Vector3 p = uuu * p0;
        p += 3 * uu * t * p1;
        p += 3 * u * tt * p2;
        p += ttt * p3;

        return p;
    }

    void UpdateLineVisuals()
    {
        if (controlPoints == null || controlPoints.Count < 4 || lineRenderer == null) return;

        if (!useBezier)
        {
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, controlPoints[0].position);
            lineRenderer.SetPosition(1, controlPoints[controlPoints.Count - 1].position);
            lineRenderer.startColor = Color.blue;
            lineRenderer.endColor = Color.blue;
        }
        else
        {
            int curveCount = (controlPoints.Count - 1) / 3;
            int totalPoints = (curveCount * lineResolution) + 1;
            lineRenderer.positionCount = totalPoints;

            lineRenderer.startColor = Color.red;
            lineRenderer.endColor = Color.red;

            for (int i = 0; i < totalPoints; i++)
            {
                float drawT = i / (float)(totalPoints - 1);
                lineRenderer.SetPosition(i, GetTrajectoryPoint(drawT));
            }
        }
    }

    void OnDrawGizmos()
    {
        if (controlPoints == null || controlPoints.Count < 4) return;

        Gizmos.color = Color.green;
        int curveCount = (controlPoints.Count - 1) / 3;

        for (int i = 0; i < curveCount; i++)
        {
            int nodeIndex = i * 3;
            if (nodeIndex + 3 >= controlPoints.Count) break;
            if (controlPoints[nodeIndex] == null || controlPoints[nodeIndex + 1] == null ||
                controlPoints[nodeIndex + 2] == null || controlPoints[nodeIndex + 3] == null) continue;

            Gizmos.DrawLine(controlPoints[nodeIndex].position, controlPoints[nodeIndex + 1].position);
            Gizmos.DrawLine(controlPoints[nodeIndex + 1].position, controlPoints[nodeIndex + 2].position);
            Gizmos.DrawLine(controlPoints[nodeIndex + 2].position, controlPoints[nodeIndex + 3].position);
        }
    }
}