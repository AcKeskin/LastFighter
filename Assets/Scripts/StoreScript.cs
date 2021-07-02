using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreScript : MonoBehaviour
{
    public bool canShop = false;
    public bool hasLaser = false;
    public bool hasRocket = false;


    [SerializeField]
    private SpaceShipControl sc;

    [SerializeField]
    private GameObject cashError;

    private Text t_ShopAvailable;
    [SerializeField]
    private GameObject shopMenu;
    [SerializeField]
    private GameObject laserMenu;
    [SerializeField]
    private GameObject rocketMenu;
    [SerializeField]
    private GameObject turretMenu;

    public int lrof_cost = 800;
    public int ldmg_cost = 600;
    public int rrof_cost = 200;
    public int rdmg_cost = 500;
    public int trof_cost = 50;
    public int tdmg_cost = 100;

    private int bulletCost = 5;
    private int rocketCost = 200;
    private int laserCost = 500;


    public Image[] laserROF; // = new Image[5];
    public Image[] laserDMG; // = new Image[3];
    public Image[] rocketROF; // = new Image[5];
    public Image[] rocketDMG; // = new Image[3];
    public Image[] turretROF; // = new Image[3];
    public Image[] turretDMG; // = new Image[3];

    public GameObject BuyLaser;
    public GameObject BuyRocket;


    private void Start()
    {
        canShop = false;
        shopMenu.SetActive(false);
        t_ShopAvailable = GameObject.Find("t_ShopAvailable").GetComponent<Text>();
        t_ShopAvailable.enabled = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M) && canShop)
        {
            TriggerShop();
        }


    }

    private void OnTriggerEnter(Collider other) //Show "Can use shop" text
    {
        canShop = true;
        t_ShopAvailable.enabled = true;
    }
    private void OnTriggerExit(Collider other) //Hide "Can use shop" text
    {
        canShop = false;
        t_ShopAvailable.enabled = false;
    }

    public void TriggerShop()
    {
        if (!shopMenu.activeInHierarchy)
        {
            sc.canShoot = false;
            shopMenu.SetActive(true);
            Time.timeScale = 0f; //Stop game
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            sc.canShoot = true;
            Cursor.visible = false;
            shopMenu.SetActive(false);
            Time.timeScale = 1f;  //resume game
        }
    }

    public void BuyLasers()
    {
        if (sc.cash >= 4500)
        {
            sc.AmmoCounts[2] += 10;
            sc.CashUpdate(-4500);
            BuyLaser.SetActive(false);
            laserMenu.SetActive(true);
            hasLaser = true;
        }
        else
            StartCoroutine(ShowCashError());
    }
    public void BuyRockets()
    {
        if (sc.cash >= 2500)
        {
            sc.AmmoCounts[1] += 10;
            sc.CashUpdate(-2500);
            hasRocket = true;
            BuyRocket.SetActive(false);
            rocketMenu.SetActive(true);
        }
        else
            StartCoroutine(ShowCashError());
    }

    public void LaserROFUpgrade()
    {
        if (sc.cash >= lrof_cost)
        {
            lrof_cost =(int)(lrof_cost * 1.1f);
            var button = GameObject.Find("b_LIncreaseROF").GetComponent<Button>();
            button.GetComponentInChildren<Text>().text = lrof_cost + " $";
            laserROF[sc.laserROFCount++].color = Color.green;
            sc.WeaponCooldowns[2] *= 1.0f - sc.laserROFCount * 0.1f;
            print("new cooldown is " + sc.WeaponCooldowns[2]);
            if (sc.laserROFCount >= laserROF.Length)
            {             
                button.interactable = false;
                button.GetComponentInChildren<Text>().text = "Upgraded to MAX";
            }
        }
        else
        {
            StartCoroutine(ShowCashError());
        }
    }
    public void LaserDMGUpgrade()
    {
        if (sc.cash >= ldmg_cost)
        {
            ldmg_cost = (int)(ldmg_cost * (1 + (0.1f * sc.laserDMGCount)));
            var button = GameObject.Find("b_LIncreaseDMG").GetComponent<Button>();
            button.GetComponentInChildren<Text>().text = ldmg_cost + " $";
            sc.bulletDMGs[2] *= 1.0f + sc.laserDMGCount * 0.1f;
            print("New dmg is " + sc.bulletDMGs[2]);
            laserDMG[sc.laserDMGCount++].color = Color.green;
            if (sc.laserDMGCount >= laserDMG.Length)
            {
                button.GetComponentInChildren<Text>().text = "Upgraded to MAX";
                button.interactable = false;
            }
        }
        else
        {
            StartCoroutine(ShowCashError());
        }

    }


    public void RocketROFUpgrade()
    {
        if (sc.cash >= rrof_cost)
        {
            rrof_cost = (int)((1+(0.1f*sc.rocketROFCount)) * rrof_cost);
            var button = GameObject.Find("b_RIncreaseROF").GetComponent<Button>();
            button.GetComponentInChildren<Text>().text = rrof_cost + " $";
            rocketROF[sc.rocketROFCount++].color = Color.green;
            sc.WeaponCooldowns[1] *= 1.0f - sc.rocketROFCount * 0.1f;
            print("new cooldown is " + sc.WeaponCooldowns[1]);
            if (sc.rocketROFCount >= rocketROF.Length)
            {
                button.interactable = false;
                button.GetComponentInChildren<Text>().text = "Upgraded to MAX";
            }
        }
        else
        {
            StartCoroutine(ShowCashError());
        }
    }
    public void RocketDMGUpgrade()
    {
        if (sc.cash >= rdmg_cost)
        {
            rdmg_cost = (int)(1.1f * rdmg_cost);
            var button = GameObject.Find("b_RIncreaseDMG").GetComponent<Button>();
            button.GetComponentInChildren<Text>().text = rdmg_cost + " $";
            rocketDMG[sc.rocketDMGCount++].color = Color.green;
            sc.bulletDMGs[1] *= 1.0f + sc.rocketDMGCount * 0.1f;
            print("New dmg is " + sc.bulletDMGs[1]);
            if (sc.rocketDMGCount >= laserDMG.Length)
            {
                button.interactable = false;
                button.GetComponentInChildren<Text>().text = "Upgraded to MAX";
            }
        }
        else
        {
            StartCoroutine(ShowCashError());
        }
    }


    public void TurretROFUpgrade()
    {
        if (sc.cash >= trof_cost)
        {
            trof_cost = (int)(1.1f*trof_cost);
            turretROF[sc.turretROFCount++].color = Color.green;
            var button = GameObject.Find("b_TIncreaseROF").GetComponent<Button>();
            button.GetComponentInChildren<Text>().text = trof_cost + " $";
            sc.WeaponCooldowns[0] *= 1.0f - sc.turretROFCount * 0.1f;
            print("new cooldown is " + sc.WeaponCooldowns[0]);
            if (sc.turretROFCount >= turretROF.Length)
            {
                button.interactable = false;
                button.GetComponentInChildren<Text>().text = "Upgraded to MAX";
            }
        }
        else
        {
            StartCoroutine(ShowCashError());
        }
    }
    public void TurretDMGUpgrade()
    {
        if (sc.cash >= tdmg_cost)
        {
            tdmg_cost = (int)(1.1f * tdmg_cost);
            var button = GameObject.Find("b_TIncreaseDMG").GetComponent<Button>();
            button.GetComponentInChildren<Text>().text = tdmg_cost + " $";
            turretDMG[sc.turretDMGCount++].color = Color.green;
            sc.bulletDMGs[0] *= 1.0f + sc.turretDMGCount * 0.1f;
            print("New dmg is " + sc.bulletDMGs[0]);
            if (sc.turretDMGCount >= turretDMG.Length)
            {
                button.interactable = false;
                button.GetComponentInChildren<Text>().text = "Upgraded to MAX";
            }
        }
        else
        {
            StartCoroutine(ShowCashError());
        }
    }

    public void BuyBullet(int cost)
    {
        if(sc.cash >= cost)
        {
            sc.CashUpdate(-cost);
            sc.AmmoCounts[0] += (cost / bulletCost);
            sc.UpdateAmmo();
            print(cost + "/" + bulletCost + " = " + (cost / bulletCost));
        }
        else
        {
            StartCoroutine(ShowCashError());
        }

    }
    public void BuyRocketAmmo(int cost)
    {
        if(sc.cash >= cost)
        {
            sc.CashUpdate(-cost);
            sc.AmmoCounts[1] += (cost / rocketCost);
            sc.UpdateAmmo();
            print(cost + "/" + rocketCost + " = " + (cost / rocketCost));
        }
        else
        {
            StartCoroutine(ShowCashError());
        }

    }
    public void BuyLaserAmmo(int cost)
    {
        if(sc.cash >= cost)
        {
            sc.CashUpdate(-cost);
            sc.AmmoCounts[2] += (cost / laserCost);
            sc.UpdateAmmo();
            print(cost + "/" + laserCost + " = " + (cost / laserCost));
        }
        else
        {
            StartCoroutine(ShowCashError());
        }

    }


    public IEnumerator ShowCashError()
    {
        cashError.SetActive(true);
        yield return new WaitForSeconds(7.5f);
        if (cashError.activeInHierarchy)
        {
            cashError.SetActive(false);
        }
    }
    public void WillEarnCash()
    {
        cashError.SetActive(false);
    }
}
