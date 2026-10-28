using UnityEngine;

public enum WeaponType { Pistol, SMG, Rifle, SubmachineGun, Sniper, RayGun }
public enum WeaponGrade { Normal, Rare, Legendary }

public class WeaponData : MonoBehaviour
{
    public string weaponName;
    public WeaponType weaponType;
    public WeaponGrade grade;

    [Header("전투 스펙")]
    public int damage = 1;
    public float fireCooldown = 0.4f;
    public float bulletSpeed = 20f;
    public bool isContinuous = false; // 광선포처럼 꾹 누르는 무기 여부

    [Header("광선포 전용 설정")]
    public float rayExplosionInterval = 1.4f; // 1.4초마다 폭발
    public int explosionDamage = 5;
    public float rayRange = 30f;
}