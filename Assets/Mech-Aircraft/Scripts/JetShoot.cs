using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JetShoot : MonoBehaviour
{

    LineRenderer line;

    [Header("Gun Settings")]
    public float spoolTime = 0.5f;
    public float fireRate = 0.2f;
    public float curFireRate = 0.0f;
    public float dispersalRadius = 0.1f;
    public float range = 5000.0f;
    public float overheatTime = 0;
    //public float coolDownTime = 1;
    public float normalSpread;
    bool isCoolingDown = false;

    public Transform gunPos;
    public float maxOverheatTime;
    float maxCooldownTime = 5;

    public Slider gunCooldownBar;
    public Image gunCooldownBarImage;

    // Start is called before the first frame update
    void Start()
    {
        line = gunPos.GetComponent<LineRenderer>();
        maxOverheatTime = overheatTime;
        //maxCooldownTime = coolDownTime;
        gunCooldownBar.maxValue = maxOverheatTime;

        if(AudioManager.Instance != null)
        {
            AudioManager.Instance.CreateFMODInstancePoolServerRpc(AudioManager.Instance.events.combatAudioEvents.minigunShot, 30);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (line.enabled)
        {
            line.SetPosition(0, gunPos.position);
        }

        JetFiring();
        if (isCoolingDown)
        {
            overheatTime -= Time.deltaTime * 3;
            if (overheatTime <= 0)
            {
                overheatTime = 0;
                //coolDownTime = maxCooldownTime;
                isCoolingDown = false;
            }
        }
    }



    void JetFiring()
    {
        if (GetComponent<VTOLCharacterController>().isJet)
        {
            if (InputManager.VTOL.Shoot.IsPressed())
            {
                overheatTime += Time.deltaTime;
                if (spoolTime > 0)
                {
                    spoolTime -= Time.deltaTime;
                }
                else
                {
                    if (overheatTime < 5 && !isCoolingDown)
                    {
                        if (curFireRate > 0)
                        {
                            curFireRate -= Time.deltaTime;
                        }
                        else
                        {
                            curFireRate = fireRate;
                            RaycastHit hit;
                            Vector3 dir = gunPos.transform.forward;
                            dir.x += Random.Range(-normalSpread, normalSpread);
                            dir.y += Random.Range(-normalSpread, normalSpread);
                            dir.z += Random.Range(-normalSpread, normalSpread);

                            if (Physics.Raycast(gunPos.position, dir, out hit, range))
                            {
                                line.enabled = true;
                                //Debug.Log("Hit " + hit.collider.gameObject);
                                Debug.DrawRay(gunPos.position, dir * range, Color.red, 2);
                                line.SetPosition(1, hit.point);
                            }
                            else
                            {
                                line.enabled = true;
                                //Debug.Log("Miss");
                                Debug.DrawRay(gunPos.position, gunPos.transform.forward * range, Color.red, 2);
                                line.SetPosition(1, gunPos.transform.TransformDirection(Vector3.forward)+dir * range + gunPos.position);
                            }

                            AudioManager.Instance.PlayFMODOneShotServerRpc(AudioManager.Instance.events.combatAudioEvents.minigunShot, transform.position);

                        }
                    }
                    else
                    {
                        isCoolingDown = true;
                        line.enabled = false;
                    }
                }
            }
            else
            {
                isCoolingDown = true;
                line.enabled = false;
                if (spoolTime < 0.5)
                {
                    spoolTime += Time.deltaTime;
                }
            }
        }
        
    }
}
