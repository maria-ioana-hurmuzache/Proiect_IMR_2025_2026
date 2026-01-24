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
    
    // Salvăm poziția X și Y inițială a primei pagini pentru a păstra alinierea
    private Vector2 initialXY;
    private Vector3 containerInitialLocalPos;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        grabInteractable = GetComponent<XRGrabInteractable>();
        
        if(rb) rb.isKinematic = true;
    }

    private void OnEnable()
    {
        grabInteractable.selectEntered.AddListener(OnGrabbed);
        grabInteractable.selectExited.AddListener(OnReleased);
        if (shuffleAction != null) shuffleAction.action.Enable();
    }

    private void OnDisable()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrabbed);
        grabInteractable.selectExited.RemoveListener(OnReleased);
        if (shuffleAction != null) shuffleAction.action.Disable();
    }

    private void Start()
    {
        // Memorăm exact unde ai pus tu PagesContainer în editor față de Step-ul respectiv
        if (pagesContainer != null)
        {
            containerInitialLocalPos = pagesContainer.localPosition;
        }
        
        RefreshPagesListOnly();
        ApplyDepthOffsets();
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

        Vector3 startPos = page.localPosition;
        // Ridicăm pagina pe axa Y locală, păstrând X-ul ei original
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
        
        // Calculăm noua poziție Z (la fundul teancului)
        float targetZ = -(pages.Count - 1) * pageDepthOffset;
        Vector3 finalPos = new Vector3(initialXY.x, initialXY.y, targetZ);
        Vector3 dropPeak = new Vector3(initialXY.x, initialXY.y + exitHeight, targetZ);

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

    private void ApplyDepthOffsets()
    {
        for (int i = 0; i < pages.Count; i++)
        {
            // Luăm poziția locală așa cum ai setat-o tu manual în Editor
            Vector3 currentLocalPos = pages[i].localPosition;

            // Calculăm adâncimea dorită
            float targetZ = -i * pageDepthOffset;

            // Păstrăm X și Y intacte (cele setate de tine), schimbăm doar Z
            pages[i].localPosition = new Vector3(currentLocalPos.x, currentLocalPos.y, targetZ);
            
            // Resetăm rotația să fie aliniată cu containerul
            pages[i].localRotation = Quaternion.identity;
        }
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