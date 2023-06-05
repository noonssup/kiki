using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotCrWeaponController : MonoBehaviour
{
    //근거리 무기용 스크립트 (2022.11.10 현재 전기톱 1종)

    [SerializeField] RobotWeaponController weaponCtrl;
    [SerializeField] GameObject hitEffect;
    public int atkDamage = 0;
    [SerializeField] float fireDelay = 0f;
    [SerializeField] float coolTime = 0f;
    [SerializeField] string weaponName = "140001";
    string soundName = "RobotChainsawEffect";

    private void Awake()
    {
        weaponCtrl = this.transform.GetComponent<RobotWeaponController>();
        weaponName = "140001"; ///전기톱 string_id
    }

    private void Start()
    {
        DataSetup();
    }

    void DataSetup()
    {
        if (weaponCtrl != null)
        {
            atkDamage = weaponCtrl.DamageSetup(weaponName);
            coolTime = weaponCtrl.CoolTimeSetup(weaponName);
        }
    }

    float hitDelay = 0.01f;

    private void OnCollisionStay2D(Collision2D collision)
    {
        IDamage _damage = collision.transform.GetComponent<IDamage>();
        if (hitDelay > 0f)
        {
            hitDelay -= Time.deltaTime;
            if (hitDelay <= 0f) hitDelay = 0f;
        }

        if(_damage != null && hitDelay <= 0f)
        {
            //피격 효과 (이펙트 이미지, 사운드)
            SoundManager.instance.PlayEffectSound(soundName, 1f);
            ContactPoint2D _contact = collision.contacts[0];
            GameObject _effect = Instantiate(hitEffect, _contact.point, Quaternion.identity);
            Destroy(_effect, 1.5f);
            _damage.Damage(atkDamage);
            hitDelay = 0.2f;
        }
    }
}
