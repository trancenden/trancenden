using UnityEngine;

public class AmmoPatterns : MonoBehaviour, IFireable
{
    [Tooltip("Populate the array with the child ammo gameObjects")]
    [SerializeField] private Ammo[] ammos;

    private float ammoRange; //the range of each ammo
    private float ammoSpeed;
    private Vector3 fireDirectionVector;
    private float fireDirectionAngle;
    private AmmoDetailsSO ammoDetails;
    private float ammoChargerTime;

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public void InitializeAmmo(AmmoDetailsSO ammoDetails, float aimAngle, float weaponAimAngle, float ammoSpeed, Vector3 weaponAimDirectionVector, bool isTurret, bool overrideAmmoMovement = false)
    {
        this.ammoDetails = ammoDetails;
        this.ammoSpeed = ammoSpeed;

        SetFireDirection(ammoDetails, aimAngle, weaponAimAngle, weaponAimDirectionVector, isTurret);

        ammoRange = ammoDetails.ammoRange;

        gameObject.SetActive(true);

        foreach (Ammo ammo in ammos)
        {
            ammo.InitializeAmmo(ammoDetails, aimAngle, weaponAimAngle, ammoSpeed, weaponAimDirectionVector, isTurret, true);
        }

        if (ammoDetails.ammoChargeTime > 0f)
        {
            ammoChargerTime = ammoDetails.ammoChargeTime;
        }
        else
        {
            ammoChargerTime = 0f;
        }
    }
    private void Update()
    {
        //Ammo charge effect
        if (ammoChargerTime > 0f)
        {
            ammoChargerTime -= Time.deltaTime;
            return;
        }

        //Calculate distance vector to move ammo
        Vector3 distanceVector = ammoSpeed * Time.deltaTime * fireDirectionVector;
        transform.position += distanceVector;

        transform.Rotate(new Vector3(0f, 0f, ammoDetails.ammoRotationSpeed * Time.deltaTime));

        //Disable after max range reached
        ammoRange -= distanceVector.magnitude;

        if (ammoRange < 0f)
        {
            DisableAmmo();
        }
    }
    /// <summary>
    /// Set ammo fire direction and angle based on the input angle and direction adjusted by the random spread
    /// </summary>
    private void SetFireDirection(AmmoDetailsSO ammoDetails, float aimAngle, float weaponAimAngle, Vector3 weaponAimDirectionVector, bool isTurret)
    {
        //Ammo'nun random açýsý
        float randomSpread = Random.Range(ammoDetails.ammoSpreadMin, ammoDetails.ammoSpreadMax);

        //Get a random spread toggle of 1 or -1
        int spreadToggle = Random.Range(0, 2) * 2 - 1;

        if (weaponAimDirectionVector.magnitude < Settings.useAimAngleDistance)
        {
            fireDirectionAngle = aimAngle;
        }
        else
        {
            fireDirectionAngle = weaponAimAngle;
        }

        //Adjust ammo fire angle by random spread
        fireDirectionAngle += spreadToggle * randomSpread;

        //Set ammo rotation
        //transform.eulerAngles = new Vector3(0f, 0f, fireDirectionAngle); //sildik çünkü 60.satýrda rotation var. update içinde

        //Set ammo fire direction
        fireDirectionVector = !isTurret ? weaponAimDirectionVector : HelperUtilities.GetDirectionVectorFromAngle(fireDirectionAngle);
    }
    private void DisableAmmo()
    {
        gameObject.SetActive(false);
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(ammos), ammos);
    }
#endif
    #endregion
}
