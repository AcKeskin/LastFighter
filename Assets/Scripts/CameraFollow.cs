using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

	public Transform target; 
	public SpaceShipControl control; 

	public float followDistance = 15.0f; //How far behind the camera will follow the targeter.
    public float camElevation = 8.0f; //How high the camera will rise above the targeter's Z axis.
    public float followTightness = 10.0f; //How closely the camera will follow the target. Higher values are snappier, lower results in a more lazy follow.

    public float rotTightness = 10.0f; 
    public float thrustShake = 2f; //How much the camera will shake when engines are runnin'
    public float yawMultiplier = 0.005f; //Curbs the extremes of input. This should be a really small number. Might need to be tweaked, but do it as a last resort.

    public bool shakeAmount = true; //The camera will shake when afterburners are active.

	void FixedUpdate()
	{

		if(target != null)
        {
			Vector3 newPosition = target.TransformPoint(control.yaw * yawMultiplier, camElevation, -followDistance);

			//Get the difference between the current location and the target's current location.
			Vector3 positionDifference = target.position - transform.position;
			//Move the camera towards the new position.
			transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * followTightness * Mathf.Lerp(0.1f,0.5f, (control.maxSpeed / control.currentSpeed)));
			
			Quaternion newRotation;
			if (Input.GetAxis("Thrust") > 0 && shakeAmount || (control.currentSpeed / control.maxSpeed) > 0.9f)
			{
				//Shake the camera while looking towards the targeter.
				newRotation = Quaternion.LookRotation(positionDifference + new Vector3(
					Random.Range(-thrustShake, thrustShake),
					Random.Range(-thrustShake, thrustShake),
					Random.Range(-thrustShake, thrustShake)),
					target.up);

			}
			else
			{
				//Look towards the targeter
				newRotation = Quaternion.LookRotation(positionDifference, target.up);

			}

			transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * rotTightness);
		}

	}
}
