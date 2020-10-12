using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMouseScroller : MonoBehaviour
{
    public float cameraMovementSpeed;
    public float minCameraX;
    public float maxCameraX;
    public float minCameraY;
    public float maxCameraY;
    
    public float scrollBorderPixels;
    

    float keyboardInputX, keyboardInputY;
    float mousePositionX, mousePositionY;
    void Update()
    {
        mousePositionX = Input.mousePosition.x;
        mousePositionY = Input.mousePosition.y;

        if(mousePositionY > Screen.height - scrollBorderPixels)
            scrollUp();
        else if(mousePositionY < 0 + scrollBorderPixels)
            scrollDown();
        if(mousePositionX > Screen.width - scrollBorderPixels)
            scrollRight();
        else if(mousePositionX < 0 + scrollBorderPixels)
            scrollLeft();

    }
    
        private void scrollLeft()
    {
        transform.position -= transform.right * Time.deltaTime * cameraMovementSpeed;
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, minCameraX, maxCameraX), transform.position.y, transform.position.z);
    }

    private void scrollRight()
    {
        transform.position += transform.right * Time.deltaTime * cameraMovementSpeed;
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, minCameraX, maxCameraX), transform.position.y, transform.position.z);
    }

    private void scrollUp()
    {
        transform.position += transform.up * Time.deltaTime * cameraMovementSpeed;
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, minCameraY, maxCameraY), transform.position.z);
    }

    private void scrollDown()
    {
        transform.position -= transform.up * Time.deltaTime * cameraMovementSpeed;
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, minCameraY, maxCameraY), transform.position.z);
    }
}
