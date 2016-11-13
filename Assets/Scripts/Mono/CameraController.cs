using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    public GameObject dogOne;
	public GameObject dogTwo;
    private Vector3 offset;

    void Start()
    {
        offset = new Vector3(0, 100.0f, -30f);
    }

    void LateUpdate()
    {
        Vector3 centerPos = (dogOne.transform.position - dogTwo.transform.position) / 2f + dogTwo.transform.position;
        transform.position = centerPos + offset;
        transform.LookAt(centerPos);
    }
}
