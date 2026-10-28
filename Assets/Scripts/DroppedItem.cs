using UnityEngine;
using UnityEngine.InputSystem;

public class DroppedItem : MonoBehaviour
{
    [Header("회전 설정")]
    public float rotationSpeed = 50f;
    public float floatSpeed = 2f;
    public float floatHeight = 0.2f;

    [Header("상호작용 설정")]
    public float interactableDistance = 2.5f;

    [Header("장착할 무기 프리팹 (손에 쥘 모델)")]
    public GameObject weaponModelPrefab; // 이 칸에 실제 손에 들릴 무기 모델 프리팹을 넣으세요!

    private Vector3 startPos;
    private bool isEquipped = false;
    private float dropTimer = 0f;
    private Transform player;
    private bool playerNearby = false;
    private WeaponData itemWeaponData;

    void Start()
    {
        startPos = transform.position;
        GameObject p = GameObject.FindWithTag("Player");
        if (p != null) player = p.transform;

        itemWeaponData = GetComponent<WeaponData>();
    }

    void Update()
    {
        if (isEquipped || player == null) return;

        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.World);
        float newY = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        float distance = Vector3.Distance(transform.position, player.position);
        if (distance <= interactableDistance)
        {
            playerNearby = true;

            if (Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame)
            {
                PickUpItem();
            }
        }
        else
        {
            playerNearby = false;
        }

        dropTimer += Time.deltaTime;
        if (dropTimer >= 15f)
        {
            Destroy(gameObject);
        }
    }

    void PickUpItem()
    {
        PlayerShooter shooter = player.GetComponent<PlayerShooter>();

        if (shooter != null && itemWeaponData != null)
        {
            // 바닥용 오브젝트 대신 손에 쥘 실제 무기 프리팹 전달 (없으면 자기 자신 전달)
            GameObject prefabToEquip = weaponModelPrefab != null ? weaponModelPrefab : gameObject;

            shooter.EquipWeapon(itemWeaponData, prefabToEquip);
            isEquipped = true;
            Destroy(gameObject);
        }
    }
}