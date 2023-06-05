using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotBulletController : MonoBehaviour
{
    //기관총의 총알과 박격포의 포, 모두 관리하는 스크립트

    [SerializeField] RobotBattleSceneController gameCtrl;
    [Header("총알 오브젝트 공용 데이터")]
    [SerializeField] Rigidbody2D rb;
    public Vector3 fireDir;   //포가 날아갈 방향
    [SerializeField] float firePower = 0f;
    public float fireForce = 500f;
    public int atkDamage = 0;
    public enum WeaponType {cannon, bullet };
    public WeaponType weaponType = WeaponType.cannon;

    [Header("박격포용 데이터")]
    [SerializeField] GameObject bombEffectPrefab;

    [Header("기관총용 데이터")]
    [SerializeField] GameObject bulletEffectPrefab;

    //[Header("음향")]
    //[SerializeField] AudioSource sfxFirePlayer;
    ////[SerializeField] AudioSource sfxEffectPlayer;
    //[SerializeField] AudioClip[] fireSounds;  //총알을 발사할 때
    ////[SerializeField] AudioClip[] effectSounds;//총알이 충돌할 때
    string[] soundName = { "RobotBulletFire", "RobotCannonFire" };

    private void Awake()
    {
        gameCtrl = FindObjectOfType<RobotBattleSceneController>();
        BulletSetup();
    }


    void BulletSetup()
    {
        rb = GetComponent<Rigidbody2D>();
        //sfxFirePlayer = GetComponent<AudioSource>();
        //sfxFirePlayer.volume = 1f;
        //sfxFirePlayer.loop = false;
        //sfxFirePlayer.playOnAwake = false;

        switch (weaponType)
        {
            case WeaponType.cannon: firePower = rb.mass * 300f; SoundManager.instance.PlayEffectSound(soundName[1], 1f); break;
            case WeaponType.bullet: firePower = rb.mass * 2000f; SoundManager.instance.PlayEffectSound(soundName[0], 1f); break;
        }

        Destroy(this.gameObject, 2.5f); //생성되면 2.5초 후 무조건 삭제
    }

    private void Start()
    {
        rb.AddForce(fireDir * firePower);
    }

    private void FixedUpdate()
    {
        float _angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
        transform.eulerAngles = new Vector3(0f, 0f, _angle);
    }

 
    int bulletHitCount = 0;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (gameCtrl.isGameOver) return;
        
        IDamage _damage = collision.transform.GetComponent<IDamage>();

        if (collision.gameObject.name == "Ground")
        {
            if (weaponType == WeaponType.cannon)
            {
                ContactPoint2D _contact = collision.contacts[0];
                CannonBombEffect(_contact); Destroy(this.gameObject);
            }

            if (weaponType == WeaponType.bullet && bulletHitCount == 0)
            {
                bulletHitCount++;
                ContactPoint2D _contact = collision.contacts[0];
                BulletHitEffect(_contact);
                Destroy(this.gameObject);
            }
        }

        if (_damage != null && collision.gameObject.layer != this.gameObject.layer)// LayerMask.NameToLayer("ENEMY"))
        {
            if (weaponType == WeaponType.bullet && bulletHitCount == 0)
            {
                bulletHitCount++;
                ContactPoint2D _contact = collision.contacts[0];
                BulletHitEffect(_contact);
               _damage.Damage(atkDamage);
                Destroy(this.gameObject); 
            }
            if (weaponType == WeaponType.cannon)
            {
                ContactPoint2D _contact = collision.contacts[0];
                CannonBombEffect(_contact); Destroy(this.gameObject);
            }
        }

        if(collision.gameObject.name == "CBullet(Clone)")
        {
            if (weaponType == WeaponType.cannon)
            {
                ContactPoint2D _contact = collision.contacts[0];
                CannonBombEffect(_contact); 
                Destroy(this.gameObject);
            }
        }
    }

    void BulletHitEffect(ContactPoint2D _contact)  //기관총 총알 명중 시 연출
    {
        bulletHitCount = 0;
        GameObject _effect = Instantiate(bulletEffectPrefab, _contact.point, Quaternion.identity);
        Destroy(_effect, 1f);
    }

    void CannonBombEffect(ContactPoint2D _contact)  //박격포 명중 시 연출
    {
        GameObject _bombEffect = Instantiate(bombEffectPrefab, _contact.point, Quaternion.identity);
        _bombEffect.GetComponent<RobotBombEffectController>().atkDamage = atkDamage;
        _bombEffect.GetComponent<RobotBombEffectController>().gameObject.layer = this.gameObject.layer;
    }
}
