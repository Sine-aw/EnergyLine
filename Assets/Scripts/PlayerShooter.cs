using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooter : MonoBehaviour
{
    [Header("현재 장착 중인 무기")]
    public WeaponData currentWeapon;

    [Header("무기 위치 세팅")]
    public Transform weaponSocket;
    public Transform firePoint;

    [Header("기본 장착 설정")]
    public WeaponData defaultWeaponData;
    public GameObject defaultWeaponPrefab;

    [Header("총알 설정 (일반 무기용)")]
    public GameObject bulletPrefab;

    [Header("광선포(레이저) 전용 설정")]
    public LineRenderer laserLineRenderer;

    private float laserTickTimer = 0f;
    private float explosionTimer = 0f;

    private GameObject currentWeaponModel;
    private float lastFireTime = -999f;

    void Start()
    {
        if (defaultWeaponData != null && defaultWeaponPrefab != null)
        {
            EquipWeapon(defaultWeaponData, defaultWeaponPrefab);
        }
    }

    void Update()
    {
        if (currentWeapon == null) return;

        // 뉴 인풋 시스템을 통해 현재 마우스 좌클릭을 꾹 누르고 있는지 실시간으로 직접 체크
        bool isLeftMousePressed = Mouse.current != null && Mouse.current.leftButton.isPressed;

        if (currentWeapon.weaponType == WeaponType.RayGun)
        {
            // 광선포일 때: 마우스를 누르고 있는 동안만 발사, 떼면 즉시 정지
            if (isLeftMousePressed)
            {
                ProcessRayGun();
            }
            else
            {
                StopRayGun();
            }
        }
        else
        {
            // 일반 연사 무기일 때
            if (isLeftMousePressed && currentWeapon.isContinuous)
            {
                if (Time.time >= lastFireTime + currentWeapon.fireCooldown)
                {
                    ShootBullet();
                }
            }
        }
    }

    public void OnAttack(InputValue value)
    {
        // 일반 단발 무기 처리를 위한 입력 이벤트
        if (value.isPressed)
        {
            if (currentWeapon != null && currentWeapon.weaponType != WeaponType.RayGun && !currentWeapon.isContinuous)
            {
                if (Time.time >= lastFireTime + currentWeapon.fireCooldown)
                {
                    ShootBullet();
                }
            }
        }
    }

    void ShootBullet()
    {
        if (bulletPrefab == null) return;

        lastFireTime = Time.time;
        Transform currentFirePoint = firePoint != null ? firePoint : transform;
        Vector3 spawnPos = currentFirePoint.position + currentFirePoint.forward * 0.5f;

        GameObject bullet = Instantiate(bulletPrefab, spawnPos, Quaternion.LookRotation(currentFirePoint.forward));

        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.damage = currentWeapon.damage;
        }

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.linearVelocity = currentFirePoint.forward * currentWeapon.bulletSpeed;
        }
    }

    void ProcessRayGun()
    {
        Transform currentFirePoint = firePoint != null ? firePoint : transform;
        RaycastHit hit;
        float range = currentWeapon.rayRange > 0 ? currentWeapon.rayRange : 30f;
        Vector3 rayEnd = currentFirePoint.position + currentFirePoint.forward * range;

        // 1. 레이저 렌더러 시각 효과 켜기
        if (laserLineRenderer != null)
        {
            laserLineRenderer.enabled = true;
            laserLineRenderer.SetPosition(0, currentFirePoint.position);

            if (Physics.Raycast(currentFirePoint.position, currentFirePoint.forward, out hit, range))
            {
                rayEnd = hit.point;
            }
            laserLineRenderer.SetPosition(1, rayEnd);
        }

        // 2. 0.3초마다 틱 데미지
        if (Physics.Raycast(currentFirePoint.position, currentFirePoint.forward, out hit, range))
        {
            laserTickTimer += Time.deltaTime;
            if (laserTickTimer >= 0.3f)
            {
                laserTickTimer = 0f;
                Health enemyHp = hit.collider.GetComponent<Health>();
                if (enemyHp != null && hit.collider.CompareTag("Enemy"))
                {
                    enemyHp.TakeDamage(currentWeapon.damage);
                }
            }
        }

        // 3. 1.4초마다 폭발
        explosionTimer += Time.deltaTime;
        float interval = currentWeapon.rayExplosionInterval > 0 ? currentWeapon.rayExplosionInterval : 1.4f;
        if (explosionTimer >= interval)
        {
            explosionTimer = 0f;
            TriggerRayExplosion(rayEnd);
        }
    }

    void TriggerRayExplosion(Vector3 explosionCenter)
    {
        Collider[] colliders = Physics.OverlapSphere(explosionCenter, 3f);
        foreach (var col in colliders)
        {
            if (col.CompareTag("Enemy"))
            {
                Health enemyHp = col.GetComponent<Health>();
                if (enemyHp != null)
                {
                    int expDamage = currentWeapon.explosionDamage > 0 ? currentWeapon.explosionDamage : 5;
                    enemyHp.TakeDamage(expDamage);
                }
            }
        }
        Debug.DrawRay(explosionCenter, Vector3.up * 3f, Color.yellow, 1.0f);
    }

    void StopRayGun()
    {
        laserTickTimer = 0f;
        explosionTimer = 0f;

        if (laserLineRenderer != null)
        {
            laserLineRenderer.enabled = false;
        }
    }

    public void EquipWeapon(WeaponData newWeaponData, GameObject weaponPrefab)
    {
        if (currentWeaponModel != null)
        {
            Destroy(currentWeaponModel);
        }

        if (weaponPrefab != null && weaponSocket != null)
        {
            currentWeaponModel = Instantiate(weaponPrefab, weaponSocket);

            DroppedItem dropScript = currentWeaponModel.GetComponent<DroppedItem>();
            if (dropScript != null) Destroy(dropScript);

            currentWeaponModel.transform.localPosition = Vector3.zero;
            currentWeaponModel.transform.localRotation = Quaternion.identity;
            currentWeaponModel.transform.localScale = Vector3.one;

            WeaponData spawnedWeaponData = currentWeaponModel.GetComponent<WeaponData>();
            if (spawnedWeaponData != null)
            {
                currentWeapon = spawnedWeaponData;
            }
            else
            {
                currentWeapon = newWeaponData;
            }

            Transform customFirePoint = currentWeaponModel.transform.Find("FirePoint");
            if (customFirePoint != null)
            {
                firePoint = customFirePoint;
            }
            else
            {
                firePoint = currentWeaponModel.transform;
            }
        }
    }
}