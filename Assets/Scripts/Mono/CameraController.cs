using UnityEngine;
using UnityEngine.Networking;

public class CameraController : NetworkToggleable
{
    public GameObject dogOne;
    public GameObject dogTwo;

    const float MinCamHeight = 80f;
    private bool wideAxisDown = false;
    private bool wideActive = false;
    private Vector3 offset;

    private Camera cam;
	private EntitySpawner spawner;

    public override void BothAwake()
    {
		cam = GetComponent<Camera>();

		GameObject levelManager = GameObject.FindGameObjectWithTag ("LevelManager");

		spawner = levelManager.GetComponent<EntitySpawner> ();
    }

	public override void BothStart() 
	{
		if (dogOne == null) {
			FindLocalPlayers ();
		}
	}

	public override void BothUpdate()
    {
        CatchWideToggle();

		if (dogOne == null) {
			FindLocalPlayers ();
		}
    }


    void LateUpdate()
    {
        MoveCam();
    }

	private void FindLocalPlayers() {
		bool dogOneSet = false;
		foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player")) {
			if (!player.GetComponent<NetworkIdentity> ().isLocalPlayer) {
				continue;
			}
				
			PlayerControl control = player.GetComponent<PlayerControl> ();
			spawner.RespawnDog (control);

			if (!dogOneSet) {
				dogOne = player;
				control.SetControls (PlayerControl.DogControl.DogOne);
				dogOneSet = true;
			} else {
				dogTwo = player;
				control.SetControls (PlayerControl.DogControl.DogTwo);
				return;
			}
		}
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
