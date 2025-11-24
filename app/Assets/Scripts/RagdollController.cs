using UnityEngine;

public class RagdollController : MonoBehaviour
{

    private Animator _animator;
    private Rigidbody[]  _rigidbodies;
    private Collider[] _colliders;

    void Awake()
    {
        _animator =  GetComponent<Animator>();
        _rigidbodies = GetComponentsInChildren<Rigidbody>();
        _colliders = GetComponentsInChildren<Collider>();
        SetRagdollEnabled(false);
    }
    void Start()
    {
        SetRagdollEnabled(true);
    }

    private void SetRagdollEnabled(bool enabled)
    {
        if (_animator != null)
        {
            _animator.enabled = !enabled;
        }

        foreach (Rigidbody rb in _rigidbodies)
        {
            rb.isKinematic = !enabled;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        foreach (Collider col in _colliders)
        {
            col.enabled = enabled;
        }
        
    }
}
