using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public int maxHp = 3;
    public int currentHp;
    public Slider hpSlider;

    public static int killCount;

    [Header("무기 드랍 설정 (적 전용)")]
    public GameObject[] normalWeaponPrefabs;   // 일반 등급 무기 프리팹 리스트 (60%)
    public GameObject[] rareWeaponPrefabs;     // 희귀 등급 무기 프리팹 리스트 (35%)
    public GameObject[] legendaryWeaponPrefabs;// 전설 등급 무기 프리팹 리스트 (5%)

    void Start()
    {
        currentHp = maxHp;
        UpdateBar();
    }

    public void TakeDamage(int damage)
    {
        if (currentHp <= 0) return;

        currentHp -= damage;
        Debug.Log(name + " HP : " + currentHp);
        UpdateBar();

        if (currentHp <= 0) Die();
    }

    void UpdateBar()
    {
        if (hpSlider != null)
            hpSlider.value = (float)currentHp / maxHp;
    }

    void Die()
    {
        if (CompareTag("Player"))
        {
            GameManager.Instance.GameOver();
        }
        else
        {
            // 적일 경우에만 무기 드랍 확률 계산 실행
            DropWeapon();
            Destroy(gameObject);
            killCount++;
        }
    }

    void DropWeapon()
    {
        float roll = Random.Range(0f, 100f); // 0부터 100 사이의 난수 생성
        GameObject selectedPrefab = null;

        if (roll < 60f)
        {
            // 0 ~ 60 미만 (60% 확률): 일반 등급
            selectedPrefab = GetRandomPrefab(normalWeaponPrefabs);
        }
        else if (roll < 95f)
        {
            // 60 ~ 95 미만 (35% 확률): 희귀 등급
            selectedPrefab = GetRandomPrefab(rareWeaponPrefabs);
        }
        else
        {
            // 95 ~ 100 (5% 확률): 전설 등급
            selectedPrefab = GetRandomPrefab(legendaryWeaponPrefabs);
        }

        // 선택된 무기 프리팹이 존재한다면 적의 위치에서 바닥으로 드랍
        if (selectedPrefab != null)
        {
            Instantiate(selectedPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);
        }
    }

    // 배열에 등록된 무기 중 랜덤하게 하나를 골라주는 헬퍼 함수
    GameObject GetRandomPrefab(GameObject[] prefabs)
    {
        if (prefabs == null || prefabs.Length == 0) return null;
        int index = Random.Range(0, prefabs.Length);
        return prefabs[index];
    }
}