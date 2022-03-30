using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Locomotion))]
public class Parkour : ControllerModule
{
    private Vector3 targetPos;
    
    [Header("Vaulting")]
    [SerializeField]
    private float maxVaultHeight;
    [SerializeField]
    private float vaultSpeed;
    private bool canVaultOver;
    private float vaultTimer;
    private float vaultDelay = 0.2f;
    private Vector3 vaultPeak;

    [Header("Layer Masks")]
    [SerializeField]
    private LayerMask vaultLayer;

    private void Start()
    {
        canVaultOver = false;
    }

    private void FixedUpdate()
    {
        if (controller.currentState == Controller.characterStates.active && controller.locomotion.isSprinting)
        {
            if (VaultCheck())
            {
                targetPos = VaultTarget();
            }
        }
        else if (controller.currentState == Controller.characterStates.vaulting)
        {
            Vaulting();
        }
    }

    private void Vaulting()
    {
        if (controller.currentState == Controller.characterStates.vaulting)
        {
            if (canVaultOver)
            {
                transform.position = Vector3.Lerp(transform.position, targetPos, vaultTimer * vaultSpeed);
                if (vaultTimer >= 1f)
                {
                    controller.currentState = Controller.characterStates.active;
                    vaultTimer = 0;
                    controller.rb.useGravity = true;
                    GetComponent<Collider>().enabled = true;
                    canVaultOver = false;
                }
                else
                {
                    vaultTimer += Time.deltaTime;
                }
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, vaultPeak, vaultTimer * 1f / vaultDelay);
                if (vaultTimer >= vaultDelay)
                {
                    vaultTimer = 0;
                }
                else
                {
                    vaultTimer += Time.deltaTime;
                }
                controller.rb.velocity *= 0.35f;
            }
        }
    }

    private bool VaultCheck()
    {
        RaycastHit[] hits;
        Vector3 rayTemp = transform.position + (transform.forward * 0.75f) + (transform.up * maxVaultHeight);
        hits = Physics.RaycastAll(rayTemp, -transform.up, 2f, vaultLayer);
        if (hits.Length == 1)
        {
            float distance = Vector3.Distance(transform.position, hits[0].point);
            if (distance <= 1.5f && hits[0].collider.GetComponent<ActionPoint>())
            {
                if (hits[0].collider.GetComponent<ActionPoint>().action == ActionPoint.actionTypes.vault)
                {
                    Debug.DrawLine(rayTemp, hits[0].point, Color.green);

                    float height = hits[0].point.y - transform.position.y;
                    if (height >= 0.6f)
                    {
                        vaultPeak = transform.position + new Vector3(0, hits[0].point.y, 0);
                    }
                    else if (height < 0.6f)
                    {
                        vaultPeak = transform.position + new Vector3(0, hits[0].point.y, 0) * 0.5f;
                    }

                    return true;
                }
            }
        }
        Debug.DrawRay(rayTemp, -transform.up, Color.red, 1f);
        return false;
    }

    private Vector3 VaultTarget()
    {
        RaycastHit[] hits;
        Vector3 rayTemp = transform.position + (transform.forward * 0.75f) + (transform.up * maxVaultHeight);
        hits = Physics.RaycastAll(rayTemp, -transform.up, 2f, vaultLayer);
        if (hits.Length == 1)
        {
            float distance = 0.2f;
            Vector3 lastHit = new Vector3();
            bool hasTarget = false;
            bool isShortVault = false;
            RaycastHit[] targetHits;
            while (distance < 3f)
            {
                Vector3 targetRay = transform.position + (transform.up * 2f) + (transform.forward * distance);
                targetHits = Physics.RaycastAll(targetRay, -transform.up, 2f, vaultLayer);
                if (targetHits.Length == 1)
                {
                    lastHit = targetHits[0].point;
                    if (Vector3.Distance(transform.position, lastHit) >= 1.5f)
                    {
                        Debug.DrawLine(rayTemp, targetHits[0].point, Color.yellow, 3f);
                        hasTarget = true;
                        isShortVault = false;
                    }
                    else if (Vector3.Distance(transform.position, lastHit) >= 0.2f && Vector3.Distance(transform.position, lastHit) < 1.5f)
                    {
                        hasTarget = true;
                        isShortVault = true;
                    }
                    else
                    {
                        Debug.DrawLine(rayTemp, targetHits[0].point, Color.red, 3f);
                    }
                }
                else
                {
                    if (hasTarget)
                    {
                        Vector3 flatLastHit = new Vector3(lastHit.x, 0, lastHit.z);
                        Vector3 target = flatLastHit + transform.forward;
                        Debug.DrawLine(transform.position, target, Color.white, 3f);

                        if (isShortVault)
                        {
                            vaultDelay = 0.1f;
                            StartCoroutine(VaultDelay(vaultDelay, isShortVault));
                        }
                        else
                        {
                            vaultDelay = 0.3f;
                            StartCoroutine(VaultDelay(vaultDelay, isShortVault));
                        }
                        return target;
                    }
                }
                distance += 0.1f;
            }
        }
        return transform.position;
    }

    private IEnumerator VaultDelay(float delay, bool shortVault)
    {
        controller.currentState = Controller.characterStates.vaulting;
        if (shortVault)
        {
            controller.anim.SetTrigger("ShortVault");
        }
        else
        {
            controller.anim.SetTrigger("Vault");
        }
        controller.rb.useGravity = false;
        GetComponent<Collider>().enabled = false;
        yield return new WaitForSeconds(delay);
        canVaultOver = true;
    }
}
