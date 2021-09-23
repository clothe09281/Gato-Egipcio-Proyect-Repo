using UnityEngine;
using System.Linq;

/// <summary>
/// �ĤH�����O
/// �\��G�H�����ʡB���ݡB�l�ܪ��a�B���˻P���`�B���A�ˬd��
/// ���A���G�C�| Enum�B�P�_�� switch (��¦�y�k)
/// </summary>
public class BaseEnemy : MonoBehaviour
{
    #region ���
    [Header("�򥻯�O")]
    [Range(50, 5000)]
    public float hp = 100;
    [Range(5, 1000)]
    public float attack = 20;
    [Range(1, 500)]
    public float speed = 100.0f;

    /// <summary>
    /// �H�����ݽd��
    /// </summary>
    public Vector2 v2RandomIdle = new Vector2(1, 5);
    /// <summary>
    /// �H�������d��
    /// </summary>
    public Vector2 v2RandomWalk = new Vector2(3, 6);

    // �N�p�H�����ܦb�ݩʭ��O�W
    [SerializeField]
    protected StateEnemy state;
    // public KeyCode key;

    private Rigidbody2D rig;
    private Animator ani;
    private AudioSource aud;
    /// <summary>
    /// ���ݮɶ��G�H��
    /// </summary>
    private float timeIdle;
    /// <summary>
    /// ���ݡG���ݥέp�ɾ�
    /// </summary>
    private float timerIdle;
    /// <summary>
    /// �����ɶ��G�H��
    /// </summary>
    private float timeWalk;
    /// <summary>
    /// �����έp�ɾ�
    /// </summary>
    private float timerWalk;
    #endregion

    /// <summary>
    /// ���a���O
    /// </summary>
    protected Player player;
    /// <summary>
    /// �����ϰ쪺�I��
    /// </summary>
    protected Collider2D hit;

    [Header("�����D���ơG�D��B���v")]
    public GameObject goProp;
    [Range(0, 1)]
    public float propProbability = 1;

    #region �ƥ�
    private void Start()
    {
        #region ���o����
        rig = GetComponent<Rigidbody2D>();
        ani = GetComponent<Animator>();
        aud = GetComponent<AudioSource>();

        player = GameObject.Find("�D����").GetComponent<Player>();
        #endregion

        #region ��l�ȳ]�w
        timeIdle = Random.Range(v2RandomIdle.x, v2RandomIdle.y);
        #endregion
    }
    protected virtual void Update()
    {
        CheckState();
        // Walk();
        WalkInFixedUpdate();
        checkForward();
        // Attack();
        // RandomDirection();
    }
    private void FixedUpdate()
    {

    }

    [Header("�ˬd�e��O�_����ê���Φa�O�y��")]
    public Vector3 checkForwardOffset;
    [Range(0, 1)]
    public float checkForwardRadius = 0.3f;

    // �����O�������p�G�Ʊ�l���O�Ƽg������`�G
    // 1. �׹��������O public �� protected - �O�@ ���\�l���O�s��
    // 2. �K�[����r virtual ���� - ���\�l���O�s��
    // 3. �l���O�ϥ� override �Ƽg
    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0.3f, 0.3f, 0.3f);
        // transform.right ��e���󪺥k�� (2D�Ҧ����e��A����b�Y)
        // transform.up ��e���󪺤W�� (���b�Y)
        Gizmos.DrawSphere(
            transform.position +
            transform.right * checkForwardOffset.x +
            transform.up * checkForwardOffset.y,
            checkForwardRadius);
    }
    #endregion

    // �{�Ѱ}�C
    // �y�k�G�������[�W���A���A�Ҧp�Gint[]�Bfloat[]�Bstring[]�BVector2[]
    public Collider2D[] hits;
    public Collider2D[] hitResult;

    #region ��k
    /// <summary>
    /// �ˬd���A
    /// </summary>
    private void CheckState()
    {
        switch (state)
        {
            case StateEnemy.idle:
                Idle();
                break;
            case StateEnemy.walk:
                Walk();
                break;
            case StateEnemy.track:
                break;
            case StateEnemy.attack:
                Attack();
                break;
            case StateEnemy.dead:
                Dead();
                break;
        }
    }

    /// <summary>
    /// ���ݡG�H����ƫ�i�J�쨫�����A
    /// �P�w������ܨ������A
    /// </summary>
    private void Idle()
    {
        if (timerIdle < timeIdle)           // �p�G �p�ɾ� < ���ݮɶ�
        {
            timerIdle += Time.deltaTime;    // �֥[�ɶ�
            ani.SetBool("�����}��", false);  // ���������ʵe
            // print("����");
        }
        else                                // �_�h
        {
            RandomDirection();              // �H����V
            state = StateEnemy.walk;        // �������A
            timeWalk = Random.Range(v2RandomWalk.x, v2RandomWalk.y);    // ���o�H�������ɶ�
            timerIdle = 0;
        }
    }

    /// <summary>
    /// �H������
    /// </summary>
    private void Walk()
    {
        // print("����");

        if (timerWalk < timeWalk)
        {
            timerWalk += Time.deltaTime;
            ani.SetBool("�����}��", true);  // �}�Ҩ����ʵe
            // rig.velocity = transform.right * speed * Time.deltaTime;
        }
        else
        {
            state = StateEnemy.idle;
            rig.velocity = Vector2.zero;
            timeIdle = Random.Range(v2RandomIdle.x, v2RandomIdle.y);
            timerWalk = 0;
        }
    }

    /// <summary>
    /// �N���z�欰��W�B�z�æb FixedUpdate �I�s
    /// </summary>
    private void WalkInFixedUpdate()
    {
        // �p�G �ثe���A �O���� �N ����.�[�t�� = �k�� * �t�� * 1/50 + �W�� * �a�ߤޤO
        if (state == StateEnemy.walk) rig.velocity = transform.right * speed * Time.deltaTime + Vector3.up * rig.velocity.y;
    }

    /// <summary>
    /// �H����V�G�H�����V�k��Υ���
    /// �Ȭ� 0 �ɡA����G0, 180, 0
    /// �Ȭ� 1 �ɡA�k��G0, 0, 0
    /// </summary>
    private void RandomDirection()
    {
        // �H��.�d��(�̤p, �̤j) - ��Ʈɤ��]�t�̤j�� (0, 2) - �H�����o 0 �� 1
        int random = Random.Range(0, 2);

        if (random == 0) transform.eulerAngles = Vector2.up * 180;
        else transform.eulerAngles = Vector2.zero;
    }

    private void checkForward()
    {
        hits = Physics2D.OverlapCircleAll(
            transform.position +
            transform.right * checkForwardOffset.x +
            transform.up * checkForwardOffset.y,
            checkForwardRadius);

        // print("�e��I�쪺����G" + hit.name);
        // ��ر��p�n��V�A�קK�����ê���H�α���
        // 1. �}�C���O�Ū� - �S���a��ԤO�|����
        // 2. �}�C�������O �a�O �åB ���O���x ������ - ����ê��
        // �d�߻y�� LinQ�G�i�H�d�߰}�C��ơA�Ҧp�G�O�_�]�t�a�O�B�O�_����Ƶ����K�K

        // �I��a�O�B���x�B�D���Ҥ��|��V
        hitResult = hits.Where(x => x.name != "�a�O" && x.name != "���x" && x.name != "�D����" && x.name != "�i��z�a�O").ToArray();

        // �}�C���ŭȡG�}�C�ƶq���s
        // �p�G �I���ƶq���s (�e��S���a�诸��) �Ϊ� �I�����G�j��s (�e�観��ê��) ���n��V
        if (hits.Length == 0)
        {
            // print("�e��S���a�O�|����");
            TurnDirection();
        }
    }

    /// <summary>
    /// ��V
    /// </summary>
    private void TurnDirection()
    {
        float y = transform.eulerAngles.y;
        if (y == 0) transform.eulerAngles = Vector3.up * 180;
        else transform.eulerAngles = Vector3.zero;
    }

    [Range(0.5f, 5)]
    /// <summary>
    /// �����N�o�ɶ�
    /// </summary>
    public float cdAttack = 3;
    // �}�C�G�O�s�ۦP��������ƪ��A�֦��s���P�Ȩ�����
    // �}�C�y�k�G����[]
    // �Ҧp�G int[]�Bstring[]�BGameObject[]�BVector3[]
    [Header("��������A�i�۳]�ƶq"), Range(0, 5)]
    public float[] attacksDelay;
    [Header("����������j�h�[��_�쥻���A"), Range(0, 5)]
    public float afterAttackRestoreOriginal = 1;

    private float timerAttack;

    /// <summary>
    /// �������A�G��������òK�[�N�o
    /// </summary>
    private void Attack()
    {
        if (timerAttack < cdAttack)
        {
            timerAttack += Time.deltaTime;
            ani.SetBool("�����}��", false);
        }
        else
        {
            AttackMethod();
        }
    }

    /// <summary>
    /// �l���O�i�H�M�w�Ӧp���������k
    /// </summary>
    protected virtual void AttackMethod()
    {
        timerAttack = 0;
        ani.SetTrigger("����Ĳ�o1");
        // print("����");
    }

    /// <summary>
    /// ����
    /// </summary>
    /// <param name="damage"></param>
    public void Hurt(float damage)
    {
        hp -= damage;
        ani.SetTrigger("����Ĳ�o");

        if (hp <= 0) Dead();
    }

    /// <summary>
    /// ���`�G���`�ʵe�B���A�B�����}���B�I�����B�[�t�ץH�έ���ᵲ
    /// </summary>
    private void Dead()
    {
        hp = 0;
        ani.SetBool("���`�}��", true);
        state = StateEnemy.dead;
        GetComponent<CapsuleCollider2D>().enabled = false;      // �����I����
        rig.velocity = Vector3.zero;                            // �[�t���k�s
        rig.constraints = RigidbodyConstraints2D.FreezeAll;     // ����ᵲ����
        DropProp();

        // TeleportManager.countAllEnemy--;        // �q���ǰe�޲z�N�ƶq -1

        enabled = false;
    }

    /// <summary>
    /// ���`��I�s�����D���k�A���v�ʱ���
    /// </summary>
    private void DropProp()
    {
        // �ͦ�(����,�y��,����)
        // Quaternion.identity �s���� = Vector3.zero
        if (Random.value <= propProbability)
            Instantiate(goProp, transform.position + Vector3.up * 1.5f, Quaternion.identity);
    }
    #endregion
}

// �w�q�C�|
// 1. �ϥ�����r enum �w�q�C�|�H�Υ]�t���ﶵ�A�i�H�b���O�~�B�~�w�q
// 2. �ݭn���@�����w�q�����C�|����
// �y�k�G�׹��� enum �C�|�W�� { �ﶵ1�A�ﶵ2�A...�A�ﶵN}
public enum StateEnemy
{
    idle, walk, track, attack, dead
}
