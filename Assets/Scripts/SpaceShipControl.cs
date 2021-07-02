using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpaceShipControl : MDestroyable
{
    public enum BulletType
    {
        Bullet,
        Rocket,
        Laser
    }

    private Color lastColorOfCrosshair;

    private ParticleSystem.MainModule exhaust;
    private Rigidbody rb;
    private CustomCrosshair cross;

    private Text cashText;
    private Text ammoText;
    private GameObject currentWeaponIcon;

    private Slider thrust;

    public GameObject RocketGO;
    public GameObject LaserGO;
    public GameObject ExplosionGO;
    public GameObject BulletHitGO;
    public GameObject ShipModel;

    [SerializeField]
    private Image ShipDMGIndicator;

    public float maxHealth = 200f; // will be used while showing percentage of dmg taken

    [SerializeField]
    private Texture[] WeaponIcons;
    public int[] AmmoCounts;
    public float[] WeaponCooldowns; //Holds time in seconds for weapon to fire again for each bullet type
    public float[] bulletDMGs;

    public BulletType currentBullet;

    public int cash;

    //holds purchased upgrades' count
    public int laserROFCount = 0;
    public int laserDMGCount = 0;
    public int rocketROFCount = 0;
    public int rocketDMGCount = 0;
    public int turretROFCount = 0;
    public int turretDMGCount = 0;

    public float currentSpeed = 0f;
    public float maxSpeed = 100f;
    public float turningSpeed = 0f;
    //public float baseSpeed = 20f; //Primary flight speed
    public float motorSpeed = 45f; //Speed when the button for positive thrust is being held down
    public float thrustSpeed = 15f; //How quickly motors/brakes will reach their maximum effect
    public float rollSpeedModifier = 7; //Multiplier for roll speed. Base roll is determined by turn speed
    public float breakSpeed = 4f; //Speed when the button for negative thrust is being held down
    public float screen_clamp = 500; //Maximum pixels that cross can move from center, when exceeded crosshair will be treated as this many pixels away from center
    public float roll, yaw, pitch; //Inputs for roll, yaw, and pitch, taken from Unity's input system.
    public float pitchYawStrength = 0.5f; //Pitch/Yaw Multiplier

    public bool canShoot = true;

    Vector2 mousePos; //Pointer position from CustomPointer
    Transform leftBarrel, rightBarrel,rocketSpawnPoint, laserSpawnPoint; // Positions of weapons barrels
    [SerializeField]
    private ParticleSystem muzzleL;
    [SerializeField]
    private ParticleSystem muzzleR;

    float deadzone = 0; //Deadzone radius from custom crosshair
    float distFromVertical; //Distance in pixels from the vertical center of the screen.
    float distFromHorizontal; //Distance in pixel from the horizontal center of the screen.
    void Start()
    {
        health = maxHealth;
        bulletDMGs = new float[3] { 5,50,85};
        currentBullet = BulletType.Bullet;
        AmmoCounts = new int[3] { 300, 0, 0}; // Only turret has ammo at the beginning
        cashText = GameObject.Find("t_Cash").GetComponent<Text>();
        cash = 0;
        WeaponCooldowns = new float[3] { 1f, 5f, 10f };
        currentWeaponIcon = GameObject.Find("WeaponIcon");
        currentWeaponIcon.GetComponent<RawImage>().texture = WeaponIcons[(int)currentBullet];
        ammoText = GameObject.Find("BulletCount").GetComponent<Text>();
        ammoText.text = AmmoCounts[(int)currentBullet] +"";
        CashUpdate(99500);
        lastColorOfCrosshair = Color.white;
        ShipModel = GameObject.Find("SpaceShip");
        exhaust = GetComponentInChildren<ParticleSystem>().main;
        leftBarrel = GameObject.Find("Barrel_L").transform;
        rightBarrel = GameObject.Find("Barrel_R").transform;
        muzzleL = GameObject.Find("Muzzle_L").GetComponent<ParticleSystem>();
        muzzleR = GameObject.Find("Muzzle_R").GetComponent<ParticleSystem>();
        muzzleL.Stop();
        muzzleR.Stop();
        rocketSpawnPoint = GameObject.Find("RocketSpawn").transform;
        laserSpawnPoint = GameObject.Find("LaserSpawn").transform;
        cross = Camera.main.GetComponent<CustomCrosshair>();
        mousePos = new Vector2(0, 0);
        deadzone = cross.deadzoneRadius;
        rb = GetComponent<Rigidbody>();
        thrust = GameObject.Find("Thrust").GetComponent<Slider>();
        thrust.maxValue = maxSpeed;
    }

    private void FixedUpdate()
    {
        UpdateCursor();
        roll = (Input.GetAxis("Roll") * rollSpeedModifier);
        pitch = Mathf.Clamp(distFromVertical, -screen_clamp - deadzone, screen_clamp + deadzone) * pitchYawStrength;
        yaw = Mathf.Clamp(distFromHorizontal, -screen_clamp - deadzone, screen_clamp + deadzone) * pitchYawStrength;

        if (Input.GetAxis("Thrust") > 0)
        {
                currentSpeed += motorSpeed* thrustSpeed * Time.deltaTime;
        }
        else if (Input.GetAxis("Thrust") < 0)
        {
            currentSpeed -= breakSpeed * thrustSpeed * Time.deltaTime;
        }

        currentSpeed = Mathf.Clamp(currentSpeed,0,maxSpeed);

       rb.AddRelativeTorque(
            (pitch * turningSpeed * Time.deltaTime),
            (yaw * turningSpeed * Time.deltaTime),
            (roll * turningSpeed * (rollSpeedModifier / 2) * Time.deltaTime));
        rb.velocity = transform.forward * currentSpeed;
        UpdateSlider();
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            AmmoCounts[(int)currentBullet] += 10;
            UpdateAmmo();
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentBullet = BulletType.Bullet;
            currentWeaponIcon.GetComponent<RawImage>().texture = WeaponIcons[(int)currentBullet];
            ammoText.text = AmmoCounts[(int)currentBullet] + "";

            Time.timeScale = 0.1f;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            currentBullet = BulletType.Rocket;
            currentWeaponIcon.GetComponent<RawImage>().texture = WeaponIcons[(int)currentBullet];
            ammoText.text = AmmoCounts[(int)currentBullet] + "";
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            currentBullet = BulletType.Laser;
            currentWeaponIcon.GetComponent<RawImage>().texture = WeaponIcons[(int)currentBullet];
            ammoText.text = AmmoCounts[(int)currentBullet] + "";
        }

        if (Input.GetMouseButton(0)) 
        { 
            if (canShoot)
            {
                switch (currentBullet)
                {
                    case BulletType.Rocket:
                        if(AmmoCounts[1] > 0)
                        StartCoroutine(FireRocket());
                        break;
                    case BulletType.Bullet:
                        if (AmmoCounts[0] > 0)
                            StartCoroutine(FireBullet());
                        break;
                    case BulletType.Laser:
                        if (AmmoCounts[2] > 0)
                            StartCoroutine(FireLaser());
                        break;
                }
                ammoText.text = AmmoCounts[(int)currentBullet] + "";
            }

        }

        if (currentSpeed > 0f)
        {
            exhaust.startLifetimeMultiplier = currentSpeed / motorSpeed; 
        }
        else
        {
            exhaust.startLifetimeMultiplier = 0 ;
        }
    }

    public void CashUpdate(int amount)
    {
        cash += amount;
        cashText.text = cash + " $";
    }
    public override void TakeDamage(float dmg)
    {
        health -= dmg;
        print(health + "/" +maxHealth);
        ShipDMGIndicator.fillAmount = (health / maxHealth);
        if (health <= 0)
        {
            Explode();
        }
    }

    void UpdateBanking()
    {

        //Load rotation information.
        Quaternion newRotation = transform.rotation;
        Vector3 newEulerAngles = newRotation.eulerAngles;

        //Basically, we're just making it bank a little in the direction that it's turning.
        newEulerAngles.z += Mathf.Clamp((-yaw * turningSpeed * Time.deltaTime) * 2, -3, 3);
        newRotation.eulerAngles = newEulerAngles;

        //Apply the rotation to the gameobject that contains the model.
        ShipModel.transform.rotation = Quaternion.Slerp(ShipModel.transform.rotation, newRotation, 2 * Time.deltaTime);

    }


    void UpdateCursor()
    {
        mousePos = cross.crosshairPosition;

        float distV = Vector2.Distance(mousePos, new Vector2(mousePos.x, Screen.height / 2));
        float distH = Vector2.Distance(mousePos, new Vector2(Screen.width / 2, mousePos.y));

        if (Mathf.Abs(distV) < deadzone)
            distV = 0;
        else
            distV -= deadzone;

        if (Mathf.Abs(distH) < deadzone)
            distH = 0;
        else
            distH -= deadzone;


        distFromVertical = Mathf.Clamp(distV, 0, (Screen.height));
        distFromHorizontal = Mathf.Clamp(distH, 0, (Screen.width));

        if (mousePos.x < Screen.width / 2 && distFromHorizontal != 0)
        {
            distFromHorizontal *= -1;
        }
        if (mousePos.y >= Screen.height / 2 && distFromVertical != 0)
        {
            distFromVertical *= -1;
        }
    }

    void UpdateSlider()
    {
        thrust.value = currentSpeed;
    }

    public IEnumerator  FireRocket()
    {
        ChangeIconColor(Color.red);
        canShoot = false;
        Ray shotRay;
        if (!cross.center_lock)
            shotRay = Camera.main.ScreenPointToRay(cross.crosshairPosition);
        else
            shotRay = Camera.main.ScreenPointToRay(new Vector2(Screen.width / 2f, Screen.height / 2f));
        RaycastHit hit;
        GameObject fired;
        fired = Instantiate(RocketGO) as GameObject;
        fired.transform.position = rocketSpawnPoint.position;
        fired.transform.GetChild(0).gameObject.tag = "Rocket";
        if (Physics.Raycast(shotRay, out hit))
        {
            fired.transform.LookAt(hit.point);
        }
        else
        {
            fired.transform.LookAt(shotRay.origin + shotRay.direction * 500);
        }
        fired.transform.SetParent(this.transform);
        yield return new WaitForSeconds(WeaponCooldowns[1]);
        ChangeIconColor(Color.white);
        canShoot = true;
        AmmoCounts[1] -= 1;
    }
    public IEnumerator  FireLaser()
{
        ChangeIconColor(Color.red);
        canShoot = false;
        Ray shotRay;
        if (!cross.center_lock)
            shotRay = Camera.main.ScreenPointToRay(cross.crosshairPosition);
        else
            shotRay = Camera.main.ScreenPointToRay(new Vector2(Screen.width / 2f, Screen.height / 2f));
        RaycastHit hit;
        GameObject fired;
        fired = Instantiate(LaserGO) as GameObject;
        fired.transform.localScale *= 0.5f;
        fired.transform.position = laserSpawnPoint.position;
        fired.gameObject.tag = "Laser";
        fired.transform.SetParent(this.transform);
        if (Physics.Raycast(shotRay, out hit))
        {
            fired.transform.LookAt(hit.point);
        }
        else
        {
            fired.transform.LookAt(shotRay.origin + shotRay.direction * 500);
        }
        GameObject.Destroy(fired, 3f);
        yield return new WaitForSeconds(WeaponCooldowns[2]);
        ChangeIconColor(Color.white);
        canShoot = true;
        AmmoCounts[2] -= 1;
    }
    public IEnumerator FireBullet()
    {
        ChangeIconColor(Color.red);
        if (!muzzleL.isPlaying && !muzzleR.isPlaying)
        {
            muzzleL.Play();
            muzzleR.Play();
        }
        canShoot = false;
        Ray shotRay;
        if (!cross.center_lock)
            shotRay = Camera.main.ScreenPointToRay(cross.crosshairPosition);
        else
            shotRay = Camera.main.ScreenPointToRay(new Vector2(Screen.width / 2f, Screen.height / 2f));
        RaycastHit hit;
        if (Physics.Raycast(shotRay, out hit))
        {
            var bulletHit = Instantiate(BulletHitGO);
            bulletHit.transform.position = hit.point;
            Destroy(bulletHit, 5f);
            Rigidbody r = hit.rigidbody;
            if(r != null)
            {
                r.AddForce(-hit.normal * 3f) ;
            }
            var f = hit.collider.gameObject.GetComponent<MDestroyable>();
            if(f != null)
            {
                f.TakeDamage(bulletDMGs[0]*2);
            }

        }
        AmmoCounts[0] -= 2;
        yield return new WaitForSeconds(WeaponCooldowns[0]);
        ChangeIconColor(Color.white);
        if (muzzleL.isPlaying && muzzleR.isPlaying)
        {
            muzzleR.Stop();
            muzzleL.Stop();
        }
        canShoot = true;
    }
    public void ChangeIconColor(Color c)
    {
        currentWeaponIcon.GetComponent<RawImage>().color = c;
    }

    public void UpdateAmmo()
    {
        currentWeaponIcon.GetComponent<RawImage>().texture = WeaponIcons[(int)currentBullet];
        ammoText.text = AmmoCounts[(int)currentBullet] + "";
    }



    public override void Explode()
    {
        foreach (Transform child in ShipModel.transform)
        {
            var handler = child.GetComponent<ChildCollisionHandler>();
            if (handler != null)
            {
                handler.enabled = false;
                child.GetComponent<MeshCollider>().isTrigger = false;
            }
        }
        transform.DetachChildren();
        if (cross != null && !cross.enabled)
        {
            cross.enabled = false;
        }
        foreach (Transform child in ShipModel.transform)
        {
            if (child.gameObject.GetComponent<MeshCollider>() != null)
            {
                child.gameObject.AddComponent<Rigidbody>().useGravity = false;
            }
            else
            {
                Destroy(child.gameObject);
            }
        }
        Time.timeScale = 0.25f;
        var explode = Instantiate(ExplosionGO);
        explode.transform.position = transform.position;
        explode.transform.localScale *= 5;
        Collider[] toBlow = Physics.OverlapSphere(transform.position, 20);
        foreach (Collider c in toBlow)
        {
            Rigidbody r = c.GetComponent<Rigidbody>();
            if (r != null)
            {
                r.AddExplosionForce(50f, transform.position, 20);
            }
        }
        Destroy(explode, 2.5f);
    }

}
