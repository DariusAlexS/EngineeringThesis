using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraKeyScroller : MonoBehaviour
{
    public Camera cam;
    public float cameraMovementSpeed;
    public float minCameraX;
    public float maxCameraX;
    public float minCameraY;
    public float maxCameraY;
    private float keyboardInputX, keyboardInputY;
    void Update()
    {
        keyboardInputX = Input.GetAxis("Horizontal");
        keyboardInputY = Input.GetAxis("Vertical");
        
        if(keyboardInputX!=0.0)
            moveHorizontal();
        if(keyboardInputY!=0.0)
            moveVertical();
    }

    public void moveHorizontal()
    {
        
        cam.transform.position += cam.transform.right * keyboardInputX * Time.deltaTime * cameraMovementSpeed;
        cam.transform.position = new Vector3(Mathf.Clamp(cam.transform.position.x, minCameraX, maxCameraX), cam.transform.position.y, cam.transform.position.z);
    }

    public void moveVertical()
    {
        cam.transform.position += transform.up * keyboardInputY * Time.deltaTime * cameraMovementSpeed;
        cam.transform.position = new Vector3(cam.transform.position.x, Mathf.Clamp(cam.transform.position.y, minCameraY, maxCameraY), cam.transform.position.z);
    }
}
