using UnityEngine;

public class DelayedFollow : MonoBehaviour
{
    [SerializeField]
    private float delayAmount = 0.2f;

    private float followDelay;

    private Vector3 targetFollowPosition;

    private void Start()
    {
        followDelay = transform.GetSiblingIndex() * delayAmount;
    }

    void Update()
    {
        targetFollowPosition.x = Mathf.Lerp(targetFollowPosition.x, transform.parent.position.x, Time.deltaTime / followDelay);

        targetFollowPosition.y = transform.parent.position.y + (transform.GetSiblingIndex() + 1);

        transform.position = Vector3.Lerp(transform.position, targetFollowPosition, Time.deltaTime * PlayerController.Player.Speed);

    }
}
