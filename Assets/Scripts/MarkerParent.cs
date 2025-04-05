using UnityEngine;

public class MarkerParent : MonoBehaviour {
    private AIMinigameCensorable[] allCensorableObjects;

    private void Awake() {
        allCensorableObjects = FindObjectsOfType<AIMinigameCensorable>();

        foreach (AIMinigameCensorable censorable in allCensorableObjects) {
            censorable.markerParent = censorable.markerParent != null ? censorable.markerParent : gameObject;
        }
    }
}
