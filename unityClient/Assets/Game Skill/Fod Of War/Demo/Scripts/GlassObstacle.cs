using FogOfWar;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlassObstacle : FieldObstacle
{
    private Vector3 lf;
    private Vector3 lb;
    private Vector3 rf;
    private Vector3 rb;
    public override bool IsPointInObstacle(Vector3 point)
    {
        CalculateBoundPoints();
        // 是否在前后面之间
        bool inFB = Mathf.Sign(Vector3.Dot(point - lb, transform.forward)) * Mathf.Sign(Vector3.Dot(point - lf, transform.forward)) < 0;
        // 是否在左右面之间
        bool inLR = Mathf.Sign(Vector3.Dot(point - lf, transform.right)) * Mathf.Sign(Vector3.Dot(point - rf, transform.right)) < 0;
        // 是否在上下面之间(没必要)
        return inFB && inLR;
    }

    private void CalculateBoundPoints()
    {
        Vector3 center = transform.position;
        Vector3 size = transform.localScale;
        lf = center + transform.forward * size.z * 0.5f - transform.right * transform.localScale.x * 0.5f;
        lb = center - transform.forward * size.z * 0.5f - transform.right * transform.localScale.x * 0.5f;
        rf = center + transform.forward * size.z * 0.5f + transform.right * transform.localScale.x * 0.5f;
        rb = center - transform.forward * size.z * 0.5f + transform.right * transform.localScale.x * 0.5f;
    }

    private void OnDrawGizmos()
    {
        CalculateBoundPoints();
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(lf, 0.2f);
        Gizmos.DrawSphere(lb, 0.2f);
        Gizmos.DrawSphere(rf, 0.2f);
        Gizmos.DrawSphere(rb, 0.2f);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && other.transform.GetComponent <PlayerCtrl>().isSelf==true)
        {
            fieldEye.Instance.UpdateGraidDete(GlassID);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player" && other.transform.GetComponent<PlayerCtrl>().isSelf == true)
        {
            fieldEye.Instance.UpdateGraidAdd(GlassID);
        }
    }

}
