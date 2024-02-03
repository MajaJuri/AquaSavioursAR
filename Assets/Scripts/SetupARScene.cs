using Lean.Touch;
using MalbersAnimations;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.Interaction.Toolkit.AR;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch;

public class SetupARScene : MonoBehaviour
{
    [SerializeField]
    public String defaultARAnimal = "Squid";

    public static String ARAnimal;

    [SerializeField]
    private string pathToObjectPrefab;

    [SerializeField]
    private GameObject objectPrefab;

    [SerializeField]
    public TextMeshProUGUI textComponent;

    [SerializeField]
    GameObject blink;

    [SerializeField]
    private Camera pictureCamera;

    [SerializeField]
    private TextMeshProUGUI textMessageComponent;

    [SerializeField]
    public GameObject textPanel;

    [SerializeField]
    private GameObject xrOrigin;

    [SerializeField]
    private GameObject ARSession;

    void Awake()
    {
        LoadARAnimal();
        WarningMessage("Tap on a horizontal plane to place the " + StaticScript.ARAnimal.ToLower() + ".", 5f);
        //Text textComponent = titleText.GetComponentInChildren<Text>();
        textComponent.text = ARAnimal;
    }

    private void Update()
    {
       /* if (EnhancedTouch.Touch.activeTouches.Count > 0)
        {
            EnhancedTouch.Touch touch = EnhancedTouch.Touch.activeTouches[0];

            if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began)
            {
                Debug.Log("Touch registered");
               
                if (!ARSession.GetComponent<ARPlacementInteractable>().isActiveAndEnabled)
                {
                    WarningMessage("One " + StaticScript.ARAnimal.ToLower() + " is already around you.");
                }
                
            }
        }
        */
    }

    public void LoadARAnimal()
    {
        if (StaticScript.ARAnimal == null)
        {
            StaticScript.ARAnimal = defaultARAnimal;
        }


        ARAnimal = StaticScript.ARAnimal;
        Debug.Log("Loading AR animal = " + ARAnimal);

        Debug.Log("Loading AR animal prefab = " + pathToObjectPrefab + StaticScript.ARAnimal);

        objectPrefab = Resources.Load<GameObject>(pathToObjectPrefab + StaticScript.ARAnimal);

        ARSession.GetComponent<ARPlacementInteractable>().placementPrefab = objectPrefab;
    }

    public void TakePicture()
    {
        //pictureCamera.clearFlags = CameraClearFlags.SolidColor;
        pictureCamera.enabled = true;
        StartCoroutine(TakeScreenShot());
    }
    
    IEnumerator TakeScreenShot()
    {
        if (!ARSession.GetComponent<ARPlacementInteractable>().enabled)
        {
            yield return new WaitForEndOfFrame();

            string currentTime = System.DateTime.Now.ToString("dd-MM-yy (HH-mm-ss)");
            string imageName = "screenshot " + currentTime + ".png";

            RenderTexture screenTexture = new RenderTexture(UnityEngine.Screen.width, UnityEngine.Screen.height, 16);
            pictureCamera.targetTexture = screenTexture;
            RenderTexture.active = screenTexture;
            pictureCamera.Render();
            Texture2D renderedTexture = new Texture2D(UnityEngine.Screen.width, UnityEngine.Screen.height);
            renderedTexture.ReadPixels(new Rect(0, 0, UnityEngine.Screen.width, UnityEngine.Screen.height), 0, 0);
            RenderTexture.active = null;
            byte[] byteArray = renderedTexture.EncodeToPNG();

            Instantiate(blink, new Vector2(0f, 0f), Quaternion.identity);

            if (SaveToGallery(byteArray, imageName))
            {
                Debug.Log("A screenshot was taken and saved to the gallery.");
                WarningMessage("Image saved.", 5f);
            }
            else
            {
                WarningMessage("Oops. Something went wrong.", 5f);
            }
        }
        else
        {
            WarningMessage("Tap on a horizontal plane to place the " + StaticScript.ARAnimal.ToLower() + " and then take a picture.", 5f);
        }
    }

    bool SaveToGallery(byte[] mediaBytes, string imageName)
    {
        if (UnityEngine.Application.platform == RuntimePlatform.Android)
        {
            NativeGallery.Permission permission = NativeGallery.SaveImageToGallery(mediaBytes, "AquaSavioursAR", imageName);

            if(permission == NativeGallery.Permission.Granted)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            System.IO.File.WriteAllBytes(imageName, mediaBytes);

            return true;
        }
        else
        {
            Debug.Log("Device not supported.");
            return false;
        }
    }

    public void WarningMessage(string message, float messageActiveTime = 5f)
    {
        textPanel.SetActive(true);
        textMessageComponent.text = message;
        if (messageActiveTime > 0) {
            Invoke("ClearWarningMessage", messageActiveTime);//ocisti poruku nakon x sekundi
        }
    }

    public void ClearWarningMessage()
    {
        textMessageComponent.text = "";
        textPanel.active = false;
    }

    public void EnableScalingAndRotation()
    {
        GameObject animal = GameObject.FindGameObjectWithTag("ARAnimal");
        animal.GetComponent<LeanDragTranslate>().enabled = true;
        animal.GetComponent<LeanPinchScale>().enabled = true;
        animal.GetComponent<LeanTwistRotateAxis>().enabled = true;
    }

    public void DisableScalingAndRotation()
    {
        GameObject animal = GameObject.FindGameObjectWithTag("ARAnimal");
        animal.GetComponent<LeanDragTranslate>().enabled = false;
        animal.GetComponent<LeanPinchScale>().enabled = false;
        animal.GetComponent<LeanTwistRotateAxis>().enabled = false;
    }
}
