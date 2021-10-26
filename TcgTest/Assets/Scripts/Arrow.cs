using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[ExecuteInEditMode]
public class Arrow : MonoBehaviour
{
    public Vector3 Pos2 { get; set; }
    public void Draw(Vector3 pos1, Vector3 pos2)
    {
        Pos2 = pos2;
        Vector3 direction = pos2 - pos1;
        float distance = direction.magnitude;
        direction.Normalize();
        float angle = Vector3.Angle(direction, Vector3.up);
        if (pos2.x > pos1.x) angle *= -1;
        transform.position = pos1 + (distance / 2) * direction;
        transform.localScale = new Vector3(1 + (distance/15), distance / 5, transform.localScale.z);
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }
    public void Hide()
    {
        transform.localScale = new Vector3(0,0,1);
    }

}
