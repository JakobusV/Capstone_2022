using Assets.Scripts.PlayerControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class PlayerPartManager : MonoBehaviour
{
    public bool DEBUG = true;

    private GameObject GameUI;
    private HealthSliderControl HealthSlider;
    private GameObject pickupPrompt;
    private GameObject PauseMenu;
    private GameObject DEBUG_UI;
    private GameObject player;
    private CapsuleCollider player_collider;
    private int action; //0=idle,1=attack,2=charge,3=sprint
    private int que_action;
    private bool isBusy;
    private bool isAttacking;
    private bool isCharging;
    private bool isGrappling;
    private bool isAttached;
    private bool isCancelRightClick;
    private bool isMovingByGrapple;
    private Vector3 grapple_direction;
    public List<IPickup> pickupQue = new List<IPickup>();
    private PlayerAttack player_attack;

    public void Start()
    {
        GameUI = GameObject.Find("GameUI");
        HealthSlider = GameObject.Find("HealthSlider").GetComponent<HealthSliderControl>();
        pickupPrompt = GameObject.Find("PickupPrompt");
        PauseMenu = GameObject.Find("PauseMenu");
        PauseMenu.SetActive(false);
        player = GameObject.Find("Player");
        player_collider = player.GetComponentInChildren<CapsuleCollider>();
        player_attack = gameObject.GetComponentInChildren<PlayerAttack>();

        SetupPreferences();
        SetupStatus();
    }

    public void TakeDamage(float Damage)
    {
        float HealthLeftover = HealthSlider.GetValue() - Damage;

        // Check if dead?
        if (HealthLeftover > 0)
        {
            // Update UI
            HealthSlider.SetValue(HealthLeftover);
        } else
        {
            // Die

        }
    }

    private void FixedUpdate()
    {
        GrappleCollision();
    }

    private void GrappleCollision()
    {
        if (isGrappling && isAttached)
        {
            Collider attachedObjCol = gameObject.GetComponentInChildren<PlayerGrapple>().hit.collider;

            if (attachedObjCol != null && player_collider.bounds.Intersects(attachedObjCol.bounds))
            {
                // Try to do reflection but could throw because of missing grapple_direction
                try
                {
                    Ray ray = new Ray(player.transform.position, grapple_direction);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit, 2))
                    {
                        // Get reflection direciton
                        Vector3 reflection_direction = Vector3.Reflect(grapple_direction, hit.normal);

                        // Cancel Grapple
                        CancelGrapple();
                        action = 0;

                        // Cancel velocity and apply force
                        Rigidbody rb = player.GetComponent<Rigidbody>();
                        rb.velocity = new Vector3(0f, 0f, 0f);
                        rb.AddForce(reflection_direction * 16, ForceMode.Impulse);
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                }
            }
        }
    }

    void Update()
    {
        TryPickup();

        IsPlayerPerforming();

        GrapplePhysics();

        ActionQueManagement();

        UpdateAction();

        PerformAction();
        gameObject.GetComponentInChildren<PlayerMovement>().SetSpeed(action);

        if (DEBUG)
        {
            Debug.Log("action:" + action +
                "\tque_action:" + que_action +
                "\tisAttacking:" + isAttacking +
                "\tisCharging:" + isCharging +
                "\tisGrappling:" + isGrappling +
                "\tisAttached:" + isAttached);
        }
    }

    private void TryPickup()
    {
        if (pickupQue.Count > 0)
        {
            pickupPrompt.SetActive(true);

            if (Input.GetKeyDown(KeyCode.E))
            {
                pickupQue[0].Pickup();
                pickupQue.RemoveAt(0);
            }
        } 
        else
        {
            pickupPrompt.SetActive(false);
        }
    }

    private void GrapplePhysics()
    {
        if (isGrappling && isAttached)
        {
            gameObject.GetComponentInChildren<PlayerMovement>().isGrappling = true;
            player.GetComponent<Rigidbody>().useGravity = false;
            Vector3 attachedPoint = (Vector3)gameObject.GetComponentInChildren<PlayerGrapple>().attachedPoint;
            player.GetComponentInChildren<CapsuleCollider>().isTrigger = true;
            if (!isMovingByGrapple)
            {
                isMovingByGrapple = true;

                // Destination - Origin
                Vector3 AB = (attachedPoint - player.transform.position);
                grapple_direction = AB.normalized;

                Rigidbody rb = player.GetComponent<Rigidbody>();
                rb.velocity = new Vector3(0f, 0f, 0f);
                rb.AddForce(grapple_direction * 32, ForceMode.Impulse);
                //rb.velocity = AB.normalized * AB.magnitude;//, ForceMode.Impulse);
                //rb.velocity = AB;
            }
            //player.transform.position = Vector3.Lerp(player.transform.position, (Vector3)attachedPoint, Time.deltaTime);
        }
        else
        {
            gameObject.GetComponentInChildren<PlayerMovement>().isGrappling = false;
            player.GetComponent<Rigidbody>().useGravity = true;
            player.GetComponentInChildren<CapsuleCollider>().isTrigger = false;
            gameObject.GetComponentInChildren<PlayerGrapple>().attachedPoint = null;
            isMovingByGrapple = false;
        }
    }

    public void CancelGrapple()
    {
        //gameObject.GetComponentInChildren<PlayerGrapple>().attachedPoint = null;
        gameObject.GetComponentInChildren<PlayerGrapple>().isGrappling = false;
    }

    private void PerformAction()
    {
        switch (action)
        {
            case 0:
                gameObject.GetComponentInChildren<PlayerMovement>().Walk();
                break;
            case 1:
                gameObject.GetComponentInChildren<PlayerGrapple>().isCharging = false;
                player_attack.TryAttack();
                gameObject.GetComponentInChildren<PlayerMovement>().Attack();
                break;
            case 2:
                gameObject.GetComponentInChildren<PlayerGrapple>().TryCharge();
                gameObject.GetComponentInChildren<PlayerMovement>().Charge();
                break;
            case 3:
                gameObject.GetComponentInChildren<PlayerMovement>().Sprint();
                break;
            case 4:
                gameObject.GetComponentInChildren<PlayerGrapple>().isCharging = false;
                gameObject.GetComponentInChildren<PlayerGrapple>().TryGrapple();
                gameObject.GetComponentInChildren<PlayerMovement>().Stop();
                break;
        }
    }

    private void UpdateAction()
    {
        switch (que_action)
        {
            case 0:
                if (!isBusy)
                {
                    action = 0;
                }
                break;
            case 1:
                if (!isBusy || isCharging || action == 3)
                {
                    action = 1;
                }
                break;
            case 2:
                if ((!isBusy || action == 3) && !isCancelRightClick)
                {
                    action = 2;
                } else if (action == 4)
                {
                    isCancelRightClick = true;
                    CancelGrapple();
                    action = 0;
                }
                break;
            case 3:
                if (!isBusy)
                {
                    action = 3;
                }
                break;
            case 4:
                if (isCharging)
                {
                    action = 4;
                } else if (!isAttached)
                {
                    CancelGrapple();
                    action = 0;
                }
                break;
            default:
                Debug.LogError("que_action switch in PlayerPartManager.UpdateAction() defaulted. que_action:" + que_action);
                break;
        }
    }

    private void ActionQueManagement()
    {
        if (isCancelRightClick && Input.GetMouseButtonUp(1))
        {
            isCancelRightClick = false;
        }

        if (Input.GetKeyDown(KeyCode.Space) && isAttached)
        {
            CancelGrapple();
            action = 0;

            gameObject.GetComponentInChildren<PlayerMovement>().Jump();
        }

        if (Input.GetMouseButton(0))
        {
            que_action = 1;
        }
        else if (Input.GetMouseButtonDown(1))
        {
            que_action = 2;
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            que_action = 3;
        }
        else if (isGrappling)
        {
            que_action = 4;
        }
        else
        {
            que_action = 0;
        }
    }

    private void IsPlayerPerforming()
    {
        // This can be repetitive but it's easier on the brain in the long run and two extra bools isn't hurting a thing

        // Attack
        isAttacking = player_attack.isAttacking;

        // Charging
        isCharging = gameObject.GetComponentInChildren<PlayerGrapple>().isCharging;

        // Grappling
        isGrappling = gameObject.GetComponentInChildren<PlayerGrapple>().isGrappling;

        isAttached = gameObject.GetComponentInChildren<PlayerGrapple>().attachedPoint != null;

        if (isAttacking || isCharging || isGrappling)
        {
            isBusy = true;
        } else
        {
            isBusy = false;
        }
    }

    public void Resume()
    {
        PauseMenu.SetActive(false);
        GameUI.SetActive(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void Pause()
    {
        PauseMenu.SetActive(true);
        GameUI.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void SetupPreferences()
    {
        foreach (FieldInfo field in typeof(PlayerPreferences).GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            // Get name of field
            string name = field.Name;

            // Get value rom field
            var value = field.GetValue(null);

            // Switch to option appropriate for applying the value to the correct part
            switch (name)
            {
                case "Vertical_Sensitivity":
                    gameObject.GetComponentInChildren<CinemachineFreeLook>().m_YAxis.m_MaxSpeed = (float)value;
                    break;
                case "Horizontal_Sensitivity":
                    gameObject.GetComponentInChildren<CinemachineFreeLook>().m_XAxis.m_MaxSpeed = (float)value * 100;
                    break;
                case "Sound":
                    break;
            }
        }
    }

    public void SetupStatus()
    {
        foreach (FieldInfo field in typeof(PlayerStatus).GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            // Get name of field
            string name = field.Name;

            // Get value from field
            var value = field.GetValue(null);

            // Get player transform for xyz cases
            Transform tf = GameObject.Find("Player").transform;

            // Switch to option appropriate for applying the value to the correct part
            switch (name)
            {
                case "Health":
                    HealthSlider.SetValue((float)value);
                    break;
                case "Move_Speed":
                    gameObject.GetComponentInChildren<PlayerMovement>().moveSpeed = (float)value;
                    break;
                case "Jump_Force":
                    gameObject.GetComponentInChildren<PlayerMovement>().jumpForce = (float)value;
                    break;
                case "Position_X":
                    tf.position = new Vector3((float)value, tf.position.y, tf.position.z);
                    break;
                case "Position_Y":
                    tf.position = new Vector3(tf.position.x, (float)value, tf.position.z);
                    break;
                case "Position_Z":
                    tf.position = new Vector3(tf.position.x, tf.position.y, (float)value);
                    break;
                case "Weapon":
                    if (!string.IsNullOrEmpty((string)value))
                    {
                        WeaponPickup((string)value);
                    }
                    break;
                default:
                    //Debug.Log("IGNORED: " + name + "=" + value);
                    break;
            }
        }
    }

    public void UpdateStatus()
    {
        foreach (FieldInfo field in typeof(PlayerStatus).GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            // Get name of field
            string name = field.Name;

            // Get player transform for xyz cases
            Transform tf = GameObject.Find("Player").transform;

            // Switch to option appropriate for applying the value from the correct part
            switch (name)
            {
                case "Health":
                    field.SetValue(null, HealthSlider.GetValue());
                    break;
                case "Move_Speed":
                    field.SetValue(null, gameObject.GetComponentInChildren<PlayerMovement>().moveSpeed);
                    break;
                case "Jump_Force":
                    field.SetValue(null, gameObject.GetComponentInChildren<PlayerMovement>().jumpForce);
                    break;
                case "Position_X":
                    field.SetValue(null, tf.position.x);
                    break;
                case "Position_Y":
                    field.SetValue(null, tf.position.y);
                    break;
                case "Position_Z":
                    field.SetValue(null, tf.position.z);
                    break;
                case "Weapon":
                    if (string.IsNullOrEmpty(player_attack.weapon.Name))
                    {
                        field.SetValue(null, "");
                    }
                    else
                    {
                        field.SetValue(null, player_attack.weapon.Name);
                    }
                    break;
                default:
                    Debug.Log("IGNORED: " + name);
                    break;
            }
        }
    }

    public void ReturnToMenu()
    {
        GameObject manager = GameObject.Find("Manager");

        if (manager != null)
        {
            manager.GetComponent<ManagerControl>().ReturnToMainMenu();
        }
    }

    public void ResumeFromButton()
    {
        GameObject manager = GameObject.Find("Manager");

        if (manager != null)
        {
            manager.GetComponent<ManagerControl>().Resume();
        } else
        {
            Resume();
        }
    }

    public void WeaponPickup(string obj)
    {
        if (player_attack.weapon != null)
        {
            // Create pickup prefab
            GameObject old_weapon = Instantiate(Resources.Load("Prefabs/Pickups/" + player_attack.weapon.ModelName + "_Pickup") as GameObject);
            old_weapon.transform.position = player_attack.dropPosition.position;
        }

        try
        {
            // New weapon
            IWeapon new_weapon = null;

            // Get type from string
            Type t = Type.GetType(obj);

            // Get class from type
            new_weapon = (IWeapon)Activator.CreateInstance(t);

            // Set weapon as class
            player_attack.weapon = new_weapon;
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
    }
}
