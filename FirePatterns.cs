//Some of the fire patterns such as spiral and circle etc.

using UnityEngine;
using static Assets.Scripts.Tools;

public class FirePatterns : MonoBehaviour
{
    public ProjectileType projectileType;
    public GameObject bulletPreFab;
    public float minRotation;
    public float maxRotation;
    public int numberOfBullets;
    public bool isRandom;

    public float cooldown;
    public float bulletSpeed;
    public Vector2 bulletVelocity;
    float timer;
    float[] rotations;
    float angle = 0f;

    private void Start()
    {
        timer = cooldown;
        rotations = new float[numberOfBullets];
        if (!isRandom)
        {
            DistributedRotations();
        }
    }

    private void Update()
    {
        if (timer <= 0)
        {
            SpawnBullets();
            timer = cooldown;
        }
        timer -= Time.deltaTime;
    }

    public float[] RandomRotations()
    {
        for (int i = 0; i < numberOfBullets; i++)
        {
            rotations[i] = Random.Range(minRotation, maxRotation);
        }
        return rotations;
    }

    public float[] DistributedRotations()
    {
        for (int i = 0; i < numberOfBullets; i++)
        {
            var fraction = i / ((float)numberOfBullets - 1);
            var difference = maxRotation - minRotation;
            var fractionOfDifference = fraction * difference;
            rotations[i] = fractionOfDifference + minRotation;
        }
        return rotations;
    }

    public GameObject[] SpawnBullets()
    {
        if (isRandom)
        {
            RandomRotations();
        }

        if (!bulletPreFab.GetComponent<ProjectileDirection>().enabled)
        {
            bulletPreFab.GetComponent<ProjectileDirection>().enabled = true;
        }

        GameObject[] spawnedBullets = new GameObject[numberOfBullets];
        if (projectileType == ProjectileType.Circle)
        {
            if (bulletPreFab.GetComponent<ProjectileDirection>().enabled)
            {
                bulletPreFab.GetComponent<ProjectileDirection>().enabled = false;
            }
            float angleStep = 360f / numberOfBullets;
            float radius = 5f;
            Vector2 startPoint = transform.position;
            for (int i = 0; i < numberOfBullets; i++)
            {
                float projectileDirXPos = startPoint.x + Mathf.Sin((angle * Mathf.PI) / 180) * radius;
                float projectileDirYPos = startPoint.y + Mathf.Cos((angle * Mathf.PI) / 180) * radius;

                Vector2 projectileVector = new Vector2(projectileDirXPos, projectileDirYPos);
                Vector2 projectileMoveDir = (projectileVector - startPoint).normalized * bulletSpeed;

                spawnedBullets[i] = Instantiate(bulletPreFab, startPoint, Quaternion.identity);
                spawnedBullets[i].GetComponent<Rigidbody2D>().velocity = new Vector2(projectileMoveDir.x, projectileMoveDir.y);
                Destroy(spawnedBullets[i], 3f);
                angle += angleStep;
            }
        }        
        else if (projectileType == ProjectileType.Spiral)
        {
            float bulDirX = transform.position.x + Mathf.Sin((angle * Mathf.PI) / 180f);
            float bulDirY = transform.position.y + Mathf.Cos((angle * Mathf.PI) / 180f);

            Vector3 bulMoveDirection = new Vector3(bulDirX, bulDirY, 0f);
            Vector3 bulDir = (bulMoveDirection - transform.position).normalized;

            spawnedBullets[0] = Instantiate(bulletPreFab, transform);
            var b = spawnedBullets[0].GetComponent<ProjectileDirection>();
            b.isSpiral = true;
            b.spiralMoveSpeed = bulletSpeed;
            b.transform.position = transform.position;
            b.transform.rotation = transform.rotation;
            b.SetMoveDirection(bulDir);
            angle += 10f;
        }
       
        return spawnedBullets;
    }
}
