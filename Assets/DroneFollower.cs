using System.Collections.Generic;
using UnityEngine;

public class DroneFollower : MonoBehaviour
{
    [Header("Target")]
    public Transform followAnchor;

    [Header("Sampling")]
    private int numDirections = 32;
    private float coneAngle = 60f;

    [Header("Prediction")]
    private float lookAheadBase = 2f;
    private float lookAheadSpeedFactor = 1.5f;
    private float clearanceRadius = 0.3f;

    [Header("Motion Limits")]
    private float maxSpeed = 1f;
    private float maxAccel = 6f;
    private float maxTurnRate = 90f;

    [Header("Weights")]
    private float wSafe = 2f;
    private float wFollow = 1.5f;
    private float wLoS = 1f;
    private float wDyn = 0.8f;

    [Header("Layers")]
    public LayerMask obstacleMask;

    // State
    Vector3 currentVelocity;
    Vector3 lastDirection;
    float currentSpeed;
    float currentClearance;

    // Smoothing
    private float speedSmoothing = 6f;
    private float dirSmoothing = 8f;

    // Fallback
    float fallbackTimer = 0f;
    bool inFallback = false;

    Renderer rend;
    MaterialPropertyBlock mpb;

    void Start()
    {
        rend = GetComponentInChildren<Renderer>();
        mpb = new MaterialPropertyBlock();
        lastDirection = (followAnchor.position - transform.position).normalized;
        if (lastDirection.sqrMagnitude < 0.001f) lastDirection = transform.forward;
        currentClearance = clearanceRadius;
    }

    float ComputeScore(Vector3 dir, Vector3 targetPos)
    {
        float score = 0f;

        Vector3 predictedPos = transform.position + dir * lookAheadBase;

        float followDist = Vector3.Distance(predictedPos, targetPos);
        score += followDist * wFollow;

        if (Physics.Raycast(transform.position, targetPos - transform.position,
            out RaycastHit hit, Vector3.Distance(transform.position, targetPos), obstacleMask))
        {
            score += wLoS * 1.5f;
        }

        float angle = Vector3.Angle(lastDirection, dir);
        score += (angle / 180f) * wDyn;

        if (Physics.SphereCast(transform.position, clearanceRadius, dir,
            out RaycastHit nearHit, lookAheadBase * 0.5f, obstacleMask))
        {
            score += wSafe * 2f;
        }

        return score;
    }

    bool ComputeBestDirection(Vector3 targetPos, out Vector3 bestDir)
    {
        bestDir = Vector3.zero;
        float bestScore = float.MaxValue;

        Vector3 toTarget = (targetPos - transform.position).normalized;

        for (int i = 0; i < numDirections; i++)
        {
            Vector3 dir = SampleDirectionInCone(toTarget);

            if (!IsDirectionFree(dir))
                continue;

            float score = ComputeScore(dir, targetPos);

            if (score < bestScore)
            {
                bestScore = score;
                bestDir = dir;
            }
        }

        return bestScore < float.MaxValue;
    }

    Vector3 SampleDirectionInCone(Vector3 forward)
    {
        float angle = coneAngle * Mathf.Deg2Rad;

        float u = Random.value;
        float v = Random.value;

        float theta = 2f * Mathf.PI * u;
        float phi = Mathf.Acos(1f - v * (1f - Mathf.Cos(angle)));

        Vector3 localDir = new Vector3(
            Mathf.Sin(phi) * Mathf.Cos(theta),
            Mathf.Sin(phi) * Mathf.Sin(theta),
            Mathf.Cos(phi)
        );

        return Quaternion.LookRotation(forward) * localDir;
    }

    bool IsDirectionFree(Vector3 dir)
    {
        float L = lookAheadBase + lookAheadSpeedFactor * currentSpeed;

        if (Physics.SphereCast(transform.position, clearanceRadius, dir,
            out RaycastHit hit, L, obstacleMask))
        {
            return false;
        }

        return true;
    }

    Vector3 GetSafeAnchorPos()
    {
        Vector3 p = followAnchor.position;

        // Si l’ancre est trop proche d’un obstacle, on la projette au plus proche point extérieur
        Collider[] hits = Physics.OverlapSphere(p, clearanceRadius, obstacleMask, QueryTriggerInteraction.Ignore);

        if (hits.Length == 0)
            return p;

        // On pousse hors des colliders détectés
        for (int i = 0; i < hits.Length; i++)
        {
            Vector3 closest = hits[i].ClosestPoint(p); // point sur la surface (ou p si déjà dehors)
            Vector3 push = p - closest;

            if (push.sqrMagnitude < 1e-6f)
            {
                // Cas rare : p est vraiment dedans et ClosestPoint renvoie p
                // On pousse dans une direction stable (loin du drone)
                push = (p - transform.position).normalized;
            }
            else
            {
                push.Normalize();
            }

            p = closest + push * (clearanceRadius + 0.02f); // petit padding
        }

        return p;
    }

    void ApplyMovement(Vector3 dir)
    {
        Vector3 desiredVel = dir * currentSpeed;
        currentVelocity = Vector3.Lerp(currentVelocity, desiredVel, Time.deltaTime * speedSmoothing);

        Vector3 delta = currentVelocity * Time.deltaTime;
        float dist = delta.magnitude;

        if (dist > 1e-5f)
        {
            Vector3 moveDir = delta / dist;

            // Cast sur le mouvement réel de CE frame
            if (Physics.SphereCast(transform.position, currentClearance, moveDir,
                out RaycastHit hit, dist, obstacleMask, QueryTriggerInteraction.Ignore))
            {
                // Stop avant la surface + slide le long du mur
                float safeDist = Mathf.Max(0f, hit.distance - 0.01f);
                transform.position += moveDir * safeDist;

                Vector3 remaining = delta - moveDir * safeDist;
                Vector3 slide = Vector3.ProjectOnPlane(remaining, hit.normal);

                // On tente de glisser (optionnel : re-cast)
                if (slide.sqrMagnitude > 1e-6f)
                {
                    Vector3 slideDir = slide.normalized;
                    float slideDist = slide.magnitude;

                    if (!Physics.SphereCast(transform.position, currentClearance, slideDir,
                        out RaycastHit hit2, slideDist, obstacleMask, QueryTriggerInteraction.Ignore))
                    {
                        transform.position += slide;
                    }
                }

                // Option : amortir la vitesse en collision
                currentVelocity *= 0.2f;
            }
            else
            {
                transform.position += delta;
            }
        }

        // Orientation douce
        if (currentVelocity.sqrMagnitude > 0.01f)
        {
            Quaternion rot = Quaternion.LookRotation(currentVelocity.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 6f);
        }
    }

    void SetState(int mode)
    {
        // 0 = normal, 1 = risk
        rend.GetPropertyBlock(mpb);
        mpb.SetFloat("_Risk", mode);
        rend.SetPropertyBlock(mpb);
    }

    void Update()
    {
        Vector3 bestDir = lastDirection; // valeur safe par défaut
        Vector3 anchorPos = GetSafeAnchorPos();

        bool found = ComputeBestDirection(anchorPos, out bestDir);

        if (!found)
        {
            // Entrée en mode fallback
            inFallback = true;
            fallbackTimer += Time.deltaTime;

            // Phase 1 : ralentissement
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, maxAccel * Time.deltaTime);

            // Phase 2 : hover / stop temporaire
            if (fallbackTimer < 0.5f)
            {
                ApplyMovement(lastDirection * 0.2f); // micro drift
            }
            else
            {
                // Après 0.5s, on autorise un petit reset direction
                lastDirection = (anchorPos - transform.position).normalized;
                fallbackTimer = 0f;
                inFallback = false;
            }

            SetState(1); // risk visuel
            return;
        }

        // Anti jitter
        float maxRad = maxTurnRate * Mathf.Deg2Rad * Time.deltaTime;
        bestDir = Vector3.RotateTowards(lastDirection, bestDir, maxRad, 0f);

        float turn01 = Mathf.Clamp01(Vector3.Angle(lastDirection, bestDir) / 90f);
        currentClearance = Mathf.Lerp(clearanceRadius, clearanceRadius * 1.4f, turn01);

        // Speed control
        float targetSpeed = maxSpeed;
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, maxAccel * Time.deltaTime);

        ApplyMovement(bestDir);

        lastDirection = bestDir;

        SetState(0); // normal
    }
}
