using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(Animator))]
public class HandIKDriver : MonoBehaviour
{
    public XRGrabInteractable grab;   // The cube
    public Transform handTarget;      // The cube's transform
    public AvatarIKGoal ikGoal = AvatarIKGoal.RightHand;

    private Animator _animator;

    void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    void OnAnimatorIK(int layerIndex)
    {
        if (grab.isSelected && handTarget != null)
        {
            // Enable IK for the hand
            _animator.SetIKPositionWeight(ikGoal, 1f);
            _animator.SetIKRotationWeight(ikGoal, 1f);

            // Move the hand toward the cube
            _animator.SetIKPosition(ikGoal, handTarget.position);
            _animator.SetIKRotation(ikGoal, handTarget.rotation);
        }
        else
        {
            // Disable IK when not grabbed
            _animator.SetIKPositionWeight(ikGoal, 0f);
            _animator.SetIKRotationWeight(ikGoal, 0f);
        }
    }
}