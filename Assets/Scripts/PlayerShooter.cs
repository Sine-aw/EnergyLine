using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooter : MonoBehaviour
{
    [Header("현재 장착 중인 무기")]
    public WeaponData currentWeapon;

    [Header("무기 위치 세팅")]
    public Transform weaponSocket; // 카메라 하위의 WeaponSocket
    public Transform firePoint;     // 총알 나가는 위치

    [Header("기본 발사 프리팹")]
    public GameObject bulletPrefab;

    private GameObject currentWeaponModel; // 현재 손에 들고 있는 3D 모델 오브젝트
    private float lastFireTime = -999f;
    private bool isHoldingMouse = false;

    // ... (OnAttack, Update 등 기존 로직 유지) ...

    // 🔥 무기를 새로 주웠을 때 외형 및 총구 위치를 갱신해주는 함수
    public void EquipWeapon(WeaponData newWeaponData, GameObject weaponPrefab)
    {
        currentWeapon = newWeaponData;

        // 1. 기존에 들고 있던 무기 모델이 있다면 삭제
        if (currentWeaponModel != null)
        {
            Destroy(currentWeaponModel);
        }

        // 2. 새 무기 3D 모델을 WeaponSocket의 자식으로 생성
        if (weaponPrefab != null && weaponSocket != null)
        {
            currentWeaponModel = Instantiate(weaponPrefab, weaponSocket);
            currentWeaponModel.transform.localPosition = Vector3.zero;
            currentWeaponModel.transform.localRotation = Quaternion.identity;

            // 3. 만약 무기 프리팹 안에 자체 "FirePoint"라는 이름의 자식 오브젝트가 있다면 거기를 총구로 지정
            Transform customFirePoint = currentWeaponModel.transform.Find("FirePoint");
            if (customFirePoint != null)
            {
                firePoint = customFirePoint;
            }
        }
    }
}