using UnityEngine;

public class FollowCamera : MonoBehaviour
{
   [SerializeField] GameObject FollowTarget;

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = FollowTarget.transform.position + new Vector3(0,0,-10);
    }
}
