using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class DriveBoneWhenGrabbed : MonoBehaviour
{
    public Transform bone;
    public XRGrabInteractable grab;

    FollowBone _follow;

    void Awake()
    {
        _follow = GetComponent<FollowBone>();
    }

    void LateUpdate()
    {
        if (grab.isSelected)
        {
            if (_follow) _follow.enabled = false;

            bone.position = transform.position;
            bone.rotation = transform.rotation;
        }
        else
        {
            if (_follow) _follow.enabled = true;
        }
    }
}