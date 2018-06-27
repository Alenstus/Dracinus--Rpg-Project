using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorAffordance : MonoBehaviour {

    [SerializeField] Texture2D walkCursor = null;
    [SerializeField] Texture2D errorCursor = null;
    [SerializeField] Texture2D combatCursor = null;
    [SerializeField] Vector2 cursorHotspot = new Vector2(96, 96); 


    CameraRaycaster cameraRaycaster;

    // Use this for initialization
    void Start() {
        cameraRaycaster = GetComponent<CameraRaycaster>();
        cameraRaycaster.layerChangeObservers += OnDelegateCalled; //Registering
    }

    // 
    void OnDelegateCalled() {
        print("CursorAffordance delegate reporting for duty!");
        switch (cameraRaycaster.layerHit)
        {
            case Layer.Walkable:
                Cursor.SetCursor(walkCursor, cursorHotspot, CursorMode.Auto);
                break;
            case Layer.Enemy:
                Cursor.SetCursor(combatCursor, cursorHotspot, CursorMode.Auto);
                break;
            default:
                Debug.LogError("Don't know what cursor to show"); 
                Cursor.SetCursor(errorCursor, cursorHotspot, CursorMode.Auto);
                return;
        
        }
    }
}
