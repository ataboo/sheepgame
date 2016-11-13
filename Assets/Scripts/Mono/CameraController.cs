using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    public GameObject dogOne;
	public GameObject dogTwo;
    private Vector3 offset;

	private Camera cam;

    void Start()
    {
        offset = new Vector3(0, 80.0f, -20f);
		cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
		Vector3 deltaDog = dogOne.transform.position - dogTwo.transform.position;

        Vector3 centerPos = deltaDog / 2f + dogTwo.transform.position;

		float vertMinHeight = Mathf.Abs(deltaDog.z * 2.1f);
		vertMinHeight = Mathf.Max(vertMinHeight, Mathf.Abs(deltaDog.x * 2.1f / cam.aspect));
		
		Debug.Log(vertMinHeight);
		Debug.Log(deltaDog);

		offset.y = Mathf.Max(80f, vertMinHeight);
		offset.z = -offset.y / 3f;

		centerPos.z += offset.z / 10f;

        transform.position = centerPos + offset;
        transform.LookAt(centerPos);
    }
}
