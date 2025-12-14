using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class HandGrabDriver : MonoBehaviour
{
    public Transform bone;                   // The character hand bone
    public XRGrabInteractable grab;          // The cube's grab interactable

    public bool followBoneWhenNotGrabbed = true;

    void LateUpdate()
    {
        if (grab.isSelected)
        {
            // Cube drives the hand
            if (bone != null)
            {
                bone.position = transform.position;
                bone.rotation = transform.rotation;
            }

            // Make the cube kinematic while grabbed
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null && !rb.isKinematic)
                rb.isKinematic = true;
        }
        else
        {
            // Cube follows the hand when not grabbed
            if (followBoneWhenNotGrabbed && bone != null)
            {
                transform.position = bone.position;
                transform.rotation = bone.rotation;
            }

            // Restore cube Rigidbody to non-kinematic if you want physics when released
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null && rb.isKinematic)
                rb.isKinematic = false;
        }
    }
}