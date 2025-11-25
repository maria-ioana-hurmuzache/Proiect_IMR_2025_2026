using UnityEngine;

public class HandIKController : MonoBehaviour
{
    private Animator _animator;
    public bool ikActive; // Turn IK control on/off
    public Transform rightHandTarget; // An empty GameObject to define the hand's goal position/rotation

    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    // Called by the Animator component
    private void OnAnimatorIK(int layerIndex)
    {
        if (_animator)
        {
            if (ikActive)
            {
                // Set the position and rotation of the right hand
                _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1f); // 1f = full IK control
                _animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1f);

                // Set the target goal
                if (rightHandTarget != null)
                {
                    _animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandTarget.position);
                    _animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandTarget.rotation);
                }
            }
            else
            {
                // Restore full animation control (turn off IK)
                _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0f);
                _animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0f);
            }
        }
    }
}