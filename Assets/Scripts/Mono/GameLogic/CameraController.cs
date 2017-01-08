using UnityEngine;
using UnityEngine.Networking;

public class CameraController : MonoBehaviour
{
    public GameObject dogOne;
    public GameObject dogTwo;

    const float MinCamHeight = 30f;
    private bool wideAxisDown = false;
    private bool wideActive = false;
    private Vector3 offset;

    private Camera cam;

    public void Start()
    {
		cam = GetComponent<Camera>();
    }

	public void Update()
    {
        CatchWideToggle();
    }


    void LateUpdate()
    {
        MoveCam();
    }

    private void MoveCam()
    {
		GameObject firstDog = dogOne == null ? dogTwo : dogOne;
		GameObject secondDog = dogOne == null ? null : dogTwo;

		if (firstDog == null)
        {
            return;
        }

        float vertMinHeight = wideActive ? MinCamHeight * 4f : MinCamHeight;
		Vector3 centerPos = firstDog.transform.position;

		if (secondDog != null)
        {
			Vector3 deltaDog = firstDog.transform.position - secondDog.transform.position;
            centerPos = deltaDog / 2f + dogTwo.transform.position;
            vertMinHeight = Mathf.Max(vertMinHeight, Mathf.Abs(deltaDog.z * 3f));
            vertMinHeight = Mathf.Max(vertMinHeight, Mathf.Abs(deltaDog.x * 3f / cam.aspect));
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

	public PlayerControl.DogControl RegisterDog(PlayerControl playerControl) {
		if (dogOne == null) {
			dogOne = playerControl.gameObject;
			return PlayerControl.DogControl.DogOne;
		} else if (dogTwo == null) {
			dogTwo = playerControl.gameObject;
			return PlayerControl.DogControl.DogTwo;
		}

		throw new UnityException ("CameraController cannot follow more than 2 dogs.");
	}

	public void UnregisterDog(PlayerControl playerControl) {
		if (playerControl.dogControl == PlayerControl.DogControl.DogOne) {
			dogOne = null;
		} else if (playerControl.dogControl == PlayerControl.DogControl.DogTwo){
			dogTwo = null;
		}
	}
}
