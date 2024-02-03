using MalbersAnimations.Controller;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch;

[RequireComponent(typeof(ARRaycastManager), typeof(ARPlaneManager))]
public class ARObjectPlacement : MonoBehaviour
{
    [SerializeField]
    private GameObject objectPrefab;
    [SerializeField]
    private float distanceFromFloor = 0.5f;
    [SerializeField]
    private float distanceFromCamera = 3f;
    [SerializeField]
    private string pathToObjectPrefab;
    [SerializeField]
    private GameObject gameManager;

    private ARRaycastManager arRaycastManager;
    private ARPlaneManager arPlaneManager;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();

    public bool objectInScene = false;
    private bool touchEnabled;

    void Awake()
    {
        arRaycastManager = GetComponent<ARRaycastManager>();
        arPlaneManager = GetComponent<ARPlaneManager>();

        gameManager.GetComponent<SetupARScene>().WarningMessage("Tap on a horizontal plane to place the " + StaticScript.ARAnimal.ToLower() + ".");

        LoadARAnimal();

        objectInScene = false;
        touchEnabled = true;
    }

    private void OnEnable()
    {
        EnhancedTouch.TouchSimulation.Enable();
        EnhancedTouch.EnhancedTouchSupport.Enable();
    }

    private void OnDisable()
    {
        EnhancedTouch.TouchSimulation.Disable();
        EnhancedTouch.EnhancedTouchSupport.Disable();
    }

    void Update()
    {
        if (EnhancedTouch.Touch.activeTouches.Count > 0 && touchEnabled)
        {
            EnhancedTouch.Touch touch = EnhancedTouch.Touch.activeTouches[0];

            if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began)
            {
                Debug.Log("Touch registered");
                gameManager.GetComponent<SetupARScene>().ClearWarningMessage();
                PlaceObject(touch);
            }
        }
    }

    void PlaceObject(EnhancedTouch.Touch touch)
    {
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        Vector2 touchPosition = touch.screenPosition;

        if (arRaycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            if (!objectInScene)
            {
                Pose hitPose = hits[0].pose;

                Vector3 newPosition = hitPose.position;

                newPosition += Camera.main.transform.forward * distanceFromCamera; // Vector3.forward myb

                newPosition += Vector3.up * distanceFromFloor;

                Instantiate(objectPrefab, newPosition, Quaternion.Euler(0f, 210f, 0f), null);

                objectInScene = true;
                touchEnabled = false;
            }
            else
            {
                if (!gameManager.GetComponent<SetupARScene>().textPanel.active)
                {
                    gameManager.GetComponent<SetupARScene>().WarningMessage("One " + StaticScript.ARAnimal.ToLower() + " is already around you.");
                }
            }
        }
        else
        {
            gameManager.GetComponent<SetupARScene>().WarningMessage("Select a position ON a horizontal plane.");
        }
    }

    public void LoadARAnimal()
    {
        Debug.Log("Loading AR animal prefab = " + pathToObjectPrefab + StaticScript.ARAnimal);

        objectPrefab = Resources.Load<GameObject>(pathToObjectPrefab + StaticScript.ARAnimal);
    }

}
