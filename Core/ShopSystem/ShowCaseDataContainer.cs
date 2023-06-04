using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowCaseDataContainer : MonoBehaviour
{
    public List<Transform> ViewPoints;
    public List<Transform> Slots;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        foreach (var point in ViewPoints)
        {
            Gizmos.DrawSphere(point.position, 0.2f);
        }

        Gizmos.color = Color.green;
        foreach (var point in Slots)
        {
            Gizmos.DrawSphere(point.position, 0.2f);
        }
    }
}
