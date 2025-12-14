using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StepShuffler : MonoBehaviour
{
    // Lista de referințe către paginile copil (Page 1, Page 2, etc.)
    private List<GameObject> pages = new List<GameObject>();

    // Setări de animație (Setează aceste valori în Inspector!)
    public float animationDuration = 0.3f;
    
    // Deplasarea paginii la ieșire (1000f pe Y ar trebui să fie suficient pentru a ieși din cadru)
    public Vector3 exitOffset = new Vector3(0, 1000f, 0); 
    
    // Indexul paginii vizibile curente în pachet
    private int currentPageIndex = 0;
    private bool isShuffling = false;

    void Awake()
    {
        // 1. Colectează toate GameObjects-urile copil (paginile)
        foreach (Transform child in transform)
        {
            pages.Add(child.gameObject);
        }

        if (pages.Count == 0)
        {
            Debug.LogError($"Step '{gameObject.name}' nu conține pagini copil.");
            return;
        }

        // 2. Configurează starea inițială
        for (int i = 0; i < pages.Count; i++)
        {
            // Doar prima pagină activă la început
            pages[i].SetActive(i == 0); 
            
            // Setăm ordinea în ierarhie. Obiectele cu index mai mic sunt afișate deasupra.
            pages[i].transform.SetSiblingIndex(i); 
        }
        
        Debug.Log($"Step {gameObject.name} inițializat cu {pages.Count} pagini.");
    }
    
    /// <summary>
    /// Metodă publică apelată de evenimentul Select Entered/Activated al XR Simple Interactable.
    /// </summary>
    public void OnPageClicked()
    {
        // Ajută la debugging-ul XR. Dacă vezi acest mesaj, interacțiunea XR funcționează!
        Debug.Log($"[XR-Hit] Click detectat pe {gameObject.name}. Index curent: {currentPageIndex}");
        
        if (isShuffling)
        {
            return;
        }
        
        // Verifică dacă am ajuns la ultima pagină
        if (currentPageIndex == pages.Count - 1)
        {
            // Dacă este ultima pagină, ascundem întregul Step.
            HideEntireStep();
            return;
        }

        // Treci la următoarea pagină
        ShuffleNextPage();
    }


    private void ShuffleNextPage()
    {
        GameObject leavingPage = pages[currentPageIndex];
        
        int nextPageIndex = currentPageIndex + 1;
        
        // Verificare suplimentară împotriva erorilor de index
        if (nextPageIndex >= pages.Count)
        {
            HideEntireStep();
            return;
        }

        GameObject comingPage = pages[nextPageIndex];

        // Animația începe
        StartCoroutine(AnimatePageTransition(leavingPage, comingPage));
    }
    
    // Corutina care gestionează mișcarea
    IEnumerator AnimatePageTransition(GameObject leavingPage, GameObject comingPage)
    {
        isShuffling = true;
        
        // 1. Pregătirea paginii următoare
        comingPage.SetActive(true);
        comingPage.transform.SetAsLastSibling(); // Mută-l în spatele ierarhiei pentru a fi cel mai de jos (dar activ)
        
        // 2. Animația de ieșire a paginii curente (de sus)
        float startTime = Time.time;
        Vector3 startPos = leavingPage.transform.localPosition;

        while (Time.time < startTime + animationDuration)
        {
            float t = (Time.time - startTime) / animationDuration;
            // Mută pagina vizibil în direcția exitOffset
            leavingPage.transform.localPosition = Vector3.Lerp(startPos, startPos + exitOffset, t);
            yield return null;
        }
        
        // 3. Finalizarea tranziției
        leavingPage.transform.localPosition = startPos; // Resetează poziția (pentru reutilizare)
        leavingPage.SetActive(false); // Dezactivează pagina care a plecat
        
        // 4. Actualizează indexul
        currentPageIndex++;
        
        isShuffling = false;
    }
    
    public void HideEntireStep()
    {
        gameObject.SetActive(false);
        Debug.Log($"Pachetul '{gameObject.name}' complet terminat și ascuns.");
    }
}