using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class StepShuffler : MonoBehaviour
{
    private List<GameObject> pages = new List<GameObject>();
    public float animationDuration = 0.25f;
    public Vector3 exitOffset = new Vector3(0, 0.3f, 0); 
    
    [Header("Floating Settings")]
    public float targetHeight = 1f;
    public float floatStrength = 5f;

    private bool isShuffling = false;
    private XRGrabInteractable grabInteractable;
    private Rigidbody rb;

    void Start() 
    {
        rb = GetComponent<Rigidbody>();
        grabInteractable = GetComponent<XRGrabInteractable>();
        
        if (grabInteractable != null)
        {
            grabInteractable.activated.AddListener(OnStepActivated);
        }

        pages.Clear();
        foreach (Transform child in transform)
        {
            // Verificăm să fie pagină și să nu fie un AttachPoint
            if (child.name.Contains("Page")) 
            {
                pages.Add(child.gameObject);
            }
        }
        
        Debug.Log($"<color=green>[StepShuffler]</color> Inițializat pe {gameObject.name}. Pagini: {pages.Count}");
    }

    void Update()
    {
        // Debug pentru calculator
        if (grabInteractable != null && grabInteractable.isSelected)
        {
            if (UnityEngine.InputSystem.Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                ShuffleTopPage();
            }
        }
    }

    // Această metodă se ocupă de plutire când nu ținem obiectul
    void FixedUpdate()
    {
        if (grabInteractable != null && !grabInteractable.isSelected && !rb.isKinematic)
        {
            float heightError = targetHeight - transform.position.y;
            rb.AddForce(Vector3.up * heightError * floatStrength, ForceMode.Acceleration);
        }
    }

    private void OnStepActivated(ActivateEventArgs args)
    {
        ShuffleTopPage();
    }

    public void ShuffleTopPage()
    {
        if (isShuffling || pages.Count <= 1) return;
        StartCoroutine(AnimatePageTransition(pages[0]));
    }

    IEnumerator AnimatePageTransition(GameObject leavingPage)
    {
        isShuffling = true;

        // 1. Gestionare Rigidbody (Evităm conflictele cu Grab)
        // Dacă îl ținem în mână, e deja Kinematic, nu trebuie să facem nimic.
        // Dacă nu îl ținem, îl facem Kinematic doar pe durata animației.
        bool wasKinematicBefore = rb.isKinematic;
        rb.isKinematic = true; 

        Transform originalParent = transform;
        
        // Salvăm orientarea corectă față de "sus-ul" manualului
        Vector3 worldStartPos = leavingPage.transform.position;
        Vector3 upDirection = originalParent.up; 

        leavingPage.transform.SetParent(null); // Deparentare pentru mișcare liberă

        // 2. Animația
        float elapsedTime = 0;
        Vector3 targetWorldPos = worldStartPos + (upDirection * exitOffset.y);

        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsedTime / animationDuration);
            leavingPage.transform.position = Vector3.Lerp(worldStartPos, targetWorldPos, t);
            yield return null;
        }

        // 3. Resetare ierarhie și poziție
        leavingPage.transform.SetParent(originalParent);
        leavingPage.transform.SetAsFirstSibling(); // Trimite la fundul teancului
        
        leavingPage.transform.localPosition = Vector3.zero;
        leavingPage.transform.localRotation = Quaternion.identity;

        // 4. REACTIVARE FIZICĂ (Logic Corectă)
        // Dacă utilizatorul a dat drumul manualului în timpul animației, reactivăm fizica.
        // Dacă utilizatorul ÎNCĂ ȚINE manualul, îl lăsăm Kinematic (altfel XR Grab se strică).
        if (grabInteractable != null && !grabInteractable.isSelected)
        {
            rb.isKinematic = false;
            rb.WakeUp();
        }

        // 5. Update Listă
        pages.RemoveAt(0);
        pages.Add(leavingPage);

        isShuffling = false;
    }

    private void OnDestroy()
    {
        if (grabInteractable != null)
            grabInteractable.activated.RemoveListener(OnStepActivated);
    }
}