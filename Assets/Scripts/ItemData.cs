using UnityEngine;

public enum ItemGrade { Normal, Rare, Legendary }

public class ItemData : MonoBehaviour
{
    public string itemName;
    public ItemGrade grade;
    public int bonusDamage = 1; // 장착 시 추가 공격력 등 활용 가능
}