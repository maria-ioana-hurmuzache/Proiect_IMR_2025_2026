using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class StepShuffler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform pagesContainer;

    [Header("Shuffle Animation")]
    [SerializeField] private float animationDuration = 0.25f;
    [SerializeField] private float exitHeight = 0.15f; 

    [Header("Input")]
    [SerializeField] private InputActionReference shuffleAction;
    [SerializeField] private float pageDepthOffset = 0.0005f;

    private readonly List<Transform> pages = new();
    private XRGrabInteractable grabInteractable;
    private Rigidbody rb;

    private bool isShuffling = false;
    private bool isHeld = false;
    private Vector2 fixedPageXY;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        grabInteractable = GetComponent<XRGrabInteractable>();
        if(rb) rb.isKinematic = true;
        
        Debug.Log($"[StepShuffler] Awake: Obiectul '{gameObject.name}' este la poziția World {transform.position}");
    }

    private void OnEnable()
    {
        grabInteractable.selectEntered.AddListener(OnGrabbed);
        grabInteractable.selectExited.AddListener(OnReleased);
        if (shuffleAction != null) shuffleAction.action.Enable();
    }

    // Modificăm Start să fie mai robust
private IEnumerator Start()
{
    if (pagesContainer == null) yield break;

    // Așteptăm un moment pentru stabilitate
    yield return new WaitForEndOfFrame();

    RefreshPagesListOnly();
    
    // FORȚĂM originea. Nu mai citim din pagini, 
    // deoarece am resetat paginile la (0,0,0) în Editor.
    fixedPageXY = Vector2.zero; 

    ApplyDepthOffsets();
    
    Debug.Log($"[StepShuffler] Paginile au fost aliniate la centrul local al {pagesContainer.name}");
}
private void ApplyDepthOffsets()
{
    for (int i = 0; i < pages.Count; i++)
    {
        float targetZ = -i * pageDepthOffset;
        // Folosim LocalPosition pentru a rămâne lipite de "Step 1" oriunde s-ar mișca el
        pages[i].localPosition = new Vector3(fixedPageXY.x, fixedPageXY.y, targetZ);
        pages[i].localRotation = Quaternion.identity;
        
        // Asigură-te că paginile nu au alt Rigidbody!
        Rigidbody rbChild = pages[i].GetComponent<Rigidbody>();
        if(rbChild) Destroy(rbChild); 
    }
}
    private void Update()
    {
        if (!isHeld || isShuffling || pages.Count <= 1) return;

        bool inputDetected = (shuffleAction != null && shuffleAction.action.WasPressedThisFrame()) ||
                             (Keyboard.current != null && Keyboard.current.rightArrowKey.wasPressedThisFrame);

        if (inputDetected) StartCoroutine(AnimateShuffleFull(pages[0]));
    }

    private void OnGrabbed(SelectEnterEventArgs args) => isHeld = true;
    private void OnReleased(SelectExitEventArgs args) { isHeld = false; if(rb) rb.isKinematic = true; }

    private IEnumerator AnimateShuffleFull(Transform page)
    {
        isShuffling = true;
        Debug.Log($"[StepShuffler] Încep shuffle pentru pagina: {page.name}");

        Vector3 startPos = page.localPosition;
        Vector3 peakPos = new Vector3(startPos.x, startPos.y + exitHeight, startPos.z);
        
        float elapsed = 0f;
        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            page.localPosition = Vector3.Lerp(startPos, peakPos, Mathf.SmoothStep(0, 1, elapsed / animationDuration));
            yield return null;
        }

        page.SetAsLastSibling();
        RefreshPagesListOnly();
        
        float targetZ = -(pages.Count - 1) * pageDepthOffset;
        Vector3 finalPos = new Vector3(fixedPageXY.x, fixedPageXY.y, targetZ);
        Vector3 dropPeak = new Vector3(fixedPageXY.x, fixedPageXY.y + exitHeight, targetZ);

        page.localPosition = dropPeak;
        page.localRotation = Quaternion.identity;

        elapsed = 0f;
        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            page.localPosition = Vector3.Lerp(dropPeak, finalPos, Mathf.SmoothStep(0, 1, elapsed / animationDuration));
            yield return null;
        }

        isShuffling = false;
        ApplyDepthOffsets();
    }

    private void RefreshPagesListOnly()
    {
        pages.Clear();
        foreach (Transform child in pagesContainer)
        {
            if (child.CompareTag("Page")) pages.Add(child);
        }
    }
}