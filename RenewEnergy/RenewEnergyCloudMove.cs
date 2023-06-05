using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;
using UnityEngine.UI;

public class RenewEnergyCloudMove : MonoBehaviour
{
    public RenewEnergyGameController gameCtrl;
    public ParticleSystem particle;
    public GameObject thunder;
    Rigidbody2D rb;
    Image[] cloudImage = new Image[2];

    public enum MoveDirection
    {
        left=0,
        right
    }
    public MoveDirection dir = MoveDirection.right;
    public MoveDirection Direction
    {
        get { return dir; }
        set
        {
            if (value == (MoveDirection)2) value = 0;
            dir = value;
            switch ((int)dir)
            {
                case 0: moveDir = Vector3.left; break;
                case 1: moveDir = Vector3.right; break;
            }
        }
    }
    float maxMovePosX = 12f;
    public Vector3 moveDir;
    Vector3 currentPos;
    Vector3 nextPos;
    public float moveSpeed;
    float dirChangeTimer = 10f;
    float leaveTimer = 0f;   //������ ����� �ð�
    public float velocityValue = 0f;  //�̵� �ӵ� (+����)
    public bool isThunder = false;
    bool isMove = false;
    bool isChange = false;

    //[Header("����")]
    //public AudioSource rainPlayer;
    //public AudioSource thunderPlayer;
    //public AudioClip rainSound;
    //public AudioClip thunderSound;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        thunder.SetActive(false);
        GetComponent<BoxCollider2D>().isTrigger = true;
        cloudImage[1] = transform.GetChild(0).GetChild(0).GetComponent<Image>();
        cloudImage[0] = transform.GetChild(1).GetComponent<Image>();
        Color[] _color = new Color[cloudImage.Length];
        _color[0] = cloudImage[0].color;
        _color[1] = cloudImage[1].color;
        _color[0].a = 0f;
        _color[1].a = 0f;
        cloudImage[0].color = _color[0];
        cloudImage[1].color = _color[1];
        //nameTag = "cloud";

        //thunderTimer = 3f;
        if (isThunder)
        {
            particle.gameObject.SetActive(true);
            StartCoroutine(ThunderEffect());
        }
        else particle.gameObject.SetActive(false);

        moveSpeed = Random.Range(0.5f, 0.8f);

        if (this.transform.position.x > 0f) Direction = (MoveDirection)0;
        else if (this.transform.position.x < 0f) Direction = (MoveDirection)1;

        StartCoroutine(CloudFadeEffect(_color[0]));
    }

    IEnumerator CloudFadeEffect(Color _color)   //������ ������ ��Ÿ���� ȿ��
    {
        Color _color2 = cloudImage[1].color;

        while(_color.a < 1f)
        {
            _color.a += Time.deltaTime;
            _color2.a += Time.deltaTime * 0.5f;
            cloudImage[0].color = _color;
            cloudImage[1].color = _color2;

            if(_color.a > 0.1f)
            {
                if (!isMove)
                {
                    isMove = true;
                    gameCtrl.cloudCount++;
                }
                GetComponent<BoxCollider2D>().isTrigger = false;

            }
            yield return null;
        }
    }

    IEnumerator ThunderEffect()
    { 
        WaitForSeconds _35sec = new WaitForSeconds(3.5f);
        WaitForSeconds _1sec = new WaitForSeconds(0.1f);
        while (isThunder)
        {
            yield return _1sec;
            SoundManager.instance.PlayEffectSound("eff_Common_thunder", 1f);
            thunder.SetActive(true);
            yield return _1sec;
            thunder.SetActive(false);
            int index = Random.Range(0, 3);
            switch (index)
            {
                case 0: yield return _35sec; break;
                case 1: yield return _35sec; yield return _35sec; break;
                case 2: yield return _35sec; yield return _35sec; yield return _35sec; break;
            }
        }
    }

    void SwitchMoveDir()  //���� �̵� ���� ����
    {
        Direction++;
    }

    private void Update()
    {
        thunder.transform.localPosition = Vector3.zero;

        CloudMovingAction();  //���� �̵� �Լ�

        if (!isChange)   //���� �ð� �̵� �� ���� ����
        {
            dirChangeTimer -= Time.deltaTime;
            if(dirChangeTimer <= 0f)
            {
                SwitchMoveDir();
                dirChangeTimer = 10f;
            }
        }

        leaveTimer += Time.deltaTime;
        if (leaveTimer > 20f || !RenewEnergyGameManager.instance.isGamePlaying)  //���� 20�� �� �Ǵ� ���� ���� �� ����
        {
            StartCoroutine(DestroyEffect());
        }
    }

    IEnumerator DestroyEffect()  //������ ������� ȿ�� �� ����
    {
        Color[] _color = new Color[cloudImage.Length];
        _color[0] = cloudImage[0].color;
        _color[1] = cloudImage[1].color;
        cloudImage[0].color = _color[0];
        cloudImage[1].color = _color[1];

        if(isThunder) particle.gameObject.SetActive(false);

        while (_color[0].a > 0f)
        {
            _color[0].a -= Time.deltaTime;
            _color[1].a -= Time.deltaTime;
            cloudImage[0].color = _color[0];
            cloudImage[1].color = _color[1];
            yield return null;
        }

        CloudDestroy();
    }


    void CloudMovingAction()  //���� �̵� �Լ�
    {
        currentPos = this.transform.position;
        if(dir == MoveDirection.right)
        {
            rb.velocity = moveDir * moveSpeed;
            
            if (this.transform.position.x >= maxMovePosX)
            {
                CloudDestroy();
            }
            nextPos = this.transform.position;
        }
        else if (dir == MoveDirection.left)
        {
            rb.velocity = moveDir * moveSpeed;
            if (this.transform.position.x <= -maxMovePosX)
            {
                CloudDestroy();
            }
            nextPos = this.transform.position;
        }
        velocityValue = rb.velocity.x;
    }

    void CloudDestroy()  //���� ����
    {
        gameCtrl.cloudCount--;
        if (isThunder)
        {
            gameCtrl.isRain= false;
        }
        Destroy(this.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Cloud02(Clone)" || collision.gameObject.name == "Cloud01(Clone)")  //�ٸ� ������ ������,
        {
            if ((float)Mathf.Abs(collision.transform.GetComponent<Rigidbody2D>().velocity.x) > (float)Mathf.Abs(this.rb.velocity.x))
            {
                this.SwitchMoveDir();
                this.isChange = true;
                this.moveSpeed = collision.transform.GetComponent<RenewEnergyCloudMove>().moveSpeed;
                collision.transform.GetComponent<RenewEnergyCloudMove>().isChange = true;
            }
            else
            {
                isChange = false;
                return;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)  //������ ������ �� �� �ڸ��� �̹� �ٸ� ������ �ִٸ� �����ϰ� �ٽ� ����
    {
        if (collision.name == "Cloud02(Clone)"|| collision.name == "Cloud01(Clone)")
        {
            if (isMove) return;

            gameCtrl.cloudTimer = 0.5f;
            if (isThunder)
            {
                gameCtrl.isRain = false;
            }
            Destroy(this.gameObject);
        }
    }

}
