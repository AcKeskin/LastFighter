using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomCrosshair : MonoBehaviour
{
    public Texture crosshairTexture; 

    public float center_speed = 5f; //How fast the pointer returns to the center.
    public bool center_lock = false; //Crosshair will be locked to the center. Also affects shooting raycast (always shoots to the center of the screen)
    public bool invertY = false; 
    public float deadzoneRadius = 0.35f; //Deadzone in the center of the screen where the pointer can move without affecting the ship's movement.
    public float mouseSensivity = 20f;
    public Rect deadzoneRect; //Rect representation of the deadzone.
    public Vector2 crosshairPosition; //Position of the pointer in screen coordinates.

    private void Awake()
    {
        crosshairPosition = Input.mousePosition;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Start()
    {
        deadzoneRect = new Rect((Screen.width / 2) - (deadzoneRadius), (Screen.height / 2) - (deadzoneRadius), deadzoneRadius * 2, deadzoneRadius * 2);

    }

    // Update is called once per frame
    void Update()
    {

        if(Time.timeScale == 1f)
        {
            float x_axis = Input.GetAxis("Mouse X");
            float y_axis = Input.GetAxis("Mouse Y");

            if (invertY)
                y_axis = -y_axis;

            crosshairPosition += new Vector2(x_axis * mouseSensivity,
                                           y_axis * mouseSensivity);


            //Keep the pointer within the bounds of the screen.
            crosshairPosition.x = Mathf.Clamp(crosshairPosition.x, 0, Screen.width);
            crosshairPosition.y = Mathf.Clamp(crosshairPosition.y, 0, Screen.height);
        }

    }

    void OnGUI()
    {
        if (Time.timeScale == 1f)
        {
            //Draw the pointer texture.
            if (crosshairTexture != null && !center_lock)
                GUI.DrawTexture(new Rect(crosshairPosition.x - (crosshairTexture.width / 2), Screen.height - crosshairPosition.y - (crosshairTexture.height / 2), crosshairTexture.width, crosshairTexture.height), crosshairTexture);
            else
            {
                GUI.DrawTexture(new Rect((Screen.width / 2f) - (crosshairTexture.width / 2), (Screen.height / 2f) - (crosshairTexture.height / 2), crosshairTexture.width, crosshairTexture.height), crosshairTexture);
            }
        }
    }

}
