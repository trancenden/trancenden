//if you apply this script to a game object, that game object follows the player.

using UnityEngine;

public class FollowPlayerShip : MonoBehaviour
{
    public GameObject player;
    public float rotationSpeed = 2.5f;
    public float followSpeed;
    public bool spaceShip;
    public void Update()
    {
        if (gameObject.activeInHierarchy)
        {
            MoveTowards(player.transform.position);
            RotateTowards(player.transform.position);
        }
    }

    private void MoveTowards(Vector2 targetPosition)
    {
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, followSpeed * Time.deltaTime);
    }

    private void RotateTowards(Vector2 targetPosition)
    {
        float angle = 0f;
        var offset = 90f;
        Vector2 direction = targetPosition - (Vector2)transform.position;
        direction.Normalize();
        if (spaceShip)
        {
            angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        }
        else
        {
            angle = Mathf.Atan2(-direction.y, -direction.x) * Mathf.Rad2Deg;
        }
        Quaternion rotation = Quaternion.AngleAxis(angle + offset, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
    }
}
