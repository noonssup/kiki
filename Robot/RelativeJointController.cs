using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelativeJointController : MonoBehaviour
{
   public RelativeJoint2D relJoint;

    private void Awake()
    {
        relJoint = GetComponent<RelativeJoint2D>();
        if (this.gameObject.layer == LayerMask.NameToLayer("PLAYER")) relJoint.enabled = false;
    }

    public void ConnectBodySetup(Rigidbody2D _parentRb2d)
    {
        if (this.transform.GetComponent<RelativeJoint2D>() != null)
        {
            this.transform.GetComponent<RelativeJoint2D>().enabled = false;
            this.transform.GetComponent<Rigidbody2D>().freezeRotation = false;
            this.transform.GetComponent<RelativeJoint2D>().connectedBody = _parentRb2d;
            this.transform.GetComponent<RelativeJoint2D>().autoConfigureOffset = true;
            this.transform.GetComponent<RelativeJoint2D>().linearOffset = Vector2.zero;
            this.transform.GetComponent<RelativeJoint2D>().autoConfigureOffset = false;
            this.transform.GetComponent<Rigidbody2D>().freezeRotation = true;
            this.transform.GetComponent<RelativeJoint2D>().enabled = true;
        }
    }
}
