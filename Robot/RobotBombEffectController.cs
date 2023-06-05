using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotBombEffectController : MonoBehaviour
{
    public int atkDamage;
    [SerializeField] int attackCount = 0;

    private void Start()
    {
        attackCount = 0;
        StartCoroutine(BombEffectPlay());
    }

    IEnumerator BombEffectPlay()  //폭발 효과 (작은 상태에서 짧은 시간 안에 커지도록)
    {
        this.transform.localScale = Vector3.zero;
        this.transform.GetComponent<CircleCollider2D>().enabled = true;
        while (this.transform.localScale.x < 2f)
        {
            this.transform.localScale += Vector3.one;

            yield return new WaitForFixedUpdate();
        }

        if (this.transform.localScale.x >= 2f)
        {
            this.transform.GetComponent<CircleCollider2D>().enabled = false;
            Destroy(this.gameObject,1f);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        IDamage _damage = collision.transform.GetComponent<IDamage>();      

        if (_damage != null && this.gameObject.layer != collision.gameObject.layer && attackCount < 1)
        {
            /*if (attackCount < 1) */
            _damage.Damage(atkDamage);
            attackCount++;

        }
    }
}
