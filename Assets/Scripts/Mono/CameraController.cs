using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    public GameObject dogOne;
    public GameObject dogTwo;

    const float MinCamHeight = 80f;
    private bool wideAxisDown = false;
    private bool wideActive = false;
    private Vector3 offset;

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        CatchWideToggle();
    }


    void LateUpdate()
    {
        MoveCam();
    }

    public void FollowDog(GameObject dog, bool exclusive = false)
    {
        dogTwo = exclusive ? null : dogOne;
        dogOne = dog;
    }

    private void MoveCam()
    {
        if (dogOne == null)
        {
            return;
        }

        float vertMinHeight = wideActive ? MinCamHeight * 4f : MinCamHeight;
        Vector3 centerPos = dogOne.transform.position;

        if (dogTwo != null)
        {
            Vector3 deltaDog = dogOne.transform.position - dogTwo.transform.position;
            centerPos = deltaDog / 2f + dogTwo.transform.position;
            vertMinHeight = Mathf.Max(vertMinHeight, Mathf.Abs(deltaDog.z * 2.1f));
            vertMinHeight = Mathf.Max(vertMinHeight, Mathf.Abs(deltaDog.x * 2.1f / cam.aspect));
        }

        offset.y = vertMinHeight;
        offset.z = -offset.y / 3f;

        centerPos.z += offset.z / 10f;

        transform.position = centerPos + offset;
        transform.LookAt(centerPos);
    }

    private void CatchWideToggle()
    {
        float wideAxis = Input.GetAxis("WideView");

        if (wideAxisDown)
        {
            if (wideAxis == 0f)
            {
                wideAxisDown = false;
            }
        }
        else
        {
            if (wideAxis > 0f)
            {
                wideActive = !wideActive;
                wideAxisDown = true;
            }
        }
    }
}
