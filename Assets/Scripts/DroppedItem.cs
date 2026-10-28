using UnityEngine;

public class DroppedItem : MonoBehaviour
{
    [Header("회전 설정")]
    public float rotationSpeed = 50f;
    public float floatSpeed = 2f;
    public float floatHeight = 0.2f;

    [Header("상호작용 설정")]
    public float interactableDistance = 2.5f;
    public ItemData itemData;

    private Vector3 startPos;
    private bool isEquipped = false;
    private float dropTimer = 0f;
    private Transform player;
    private bool playerNearby = false;

    void Start()
    {
        startPos = transform.position;
        GameObject p = GameObject.FindWithTag("Player");
        if (p != null) player = p.transform;
    }

    void Update()
    {
        if (isEquipped || player == null) return;

        // 1. 공중에서 회전하며 위아래로 살짝 움직이는 연출
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.World);
        float newY = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        // 2. 플레이어와의 거리 체크
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance <= interactableDistance)
        {
            playerNearby = true;
            // UI에 "F를 눌러 줍기" 표시 가능 (현재는 콘솔 및 키 입력 대기)
            if (Input.GetKeyDown(KeyCode.F))
            {
                PickUpItem();
            }
        }
        else
        {
            playerNearby = false;
        }

        // 3. 바닥에 떨어진 후 15초 지나면 소멸
        dropTimer += Time.deltaTime;
        if (dropTimer >= 15f)
        {
            Destroy(gameObject);
        }
    }

    void PickUpItem()
    {
        PlayerShooter shooter = player.GetComponent<PlayerShooter>();
        WeaponData itemWeaponData = GetComponent<WeaponData>();

        if (shooter != null && itemWeaponData != null)
        {
            // 1. 플레이어에게 이 무기 데이터와 내 메쉬/프리팹을 전달하여 장착!
            shooter.EquipWeapon(itemWeaponData, gameObject);

            // 2. 바닥에 있던 이 DroppedItem 오브젝트는 삭제 (플레이어 손에 새 모델이 생성되었으므로)
            Destroy(gameObject);
        }
    }
}