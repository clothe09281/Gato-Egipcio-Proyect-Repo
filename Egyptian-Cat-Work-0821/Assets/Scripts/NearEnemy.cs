using UnityEngine;
using System.Collections;   // �ޥ� �t��.���X - ��P�{��

/// <summary>
/// ��Z�������ĤH�����G��Z������
/// </summary>
// ���O�G�����O
// �G�_���᭱�Ĥ@�ӥN���O�n�~�Ӫ����O
public class NearEnemy : BaseEnemy
{
    #region ���
    [Header("�����ϰ쪺�첾�P�j�p")]
    public Vector2 checkAttackOffset;
    public Vector2 checkAttackSize;
    #endregion

    #region �ƥ�
    protected override void OnDrawGizmos()
    {
        // �����O�쥻���{�����e
        base.OnDrawGizmos();

        Gizmos.color = new Color(0.5f, 0.3f, 0.1f, 0.3f);
        Gizmos.DrawCube(
            transform.position +
            transform.right * checkAttackOffset.x +
            transform.up * checkAttackOffset.y,
            checkAttackSize);
    }

    protected override void Update()
    {
        base.Update();

        CheckPlayerInAttackArea();
    }

    #endregion

    #region ��k
    /// <summary>
    /// �ˬd���a�O�_�i�J�����ϰ�
    /// </summary>
    private void CheckPlayerInAttackArea()
    {
        hit = Physics2D.OverlapBox(
            transform.position +
            transform.right * checkAttackOffset.x +
            transform.up * checkAttackOffset.y,
            checkAttackSize, 0, 1 << 7);

        // �p�G �I�쪫�󬰪��a �N�N���A�אּ ����
        if (hit) state = StateEnemy.attack;
        // print(hit.name);
    }

    protected override void AttackMethod()
    {
        base.AttackMethod();

        StartCoroutine(DelaySendDamageToPlayer());      // �Ұʨ�P�{��
    }

    // ��P�{�ǥΪk�G
    // 1. �ޥ� System.Collections API
    // 2. �Ǧ^��k�A�Ǧ^������ IEnumerator
    // 3. �ϥ� StartCoroutine() �Ұʨ�P�{��

    /// <summary>
    /// ����N�ˮ`�ǰe�����a
    /// </summary>
    private IEnumerator DelaySendDamageToPlayer()
    {
        // ���o�}�C�ƶq�y�k�G�}�C.Length
        for (int i = 0; i < attacksDelay.Length; i++)
        {
            // �h���{���ֱ���GAlt + �W�ΤU
            // �榡�Ʊƪ��ֱ���GCtrl + K D

            // ���o�}�C�y�k�G�}�C���W��[]
            yield return new WaitForSeconds(attacksDelay[0]);          // ����ɶ�
            // print("�Ĥ@������");
            if (hit) player.Hurt(attack);                               // �p�G�I����T�s�b�A�缾�a�y���ˮ`
        }
        // ���ݧ������_�쥻���A�ɶ� - �����ʵe�̫᪺�ɶ�
        yield return new WaitForSeconds(afterAttackRestoreOriginal);
        // �p�G ���a�٦b�����ϰ줺 �N���� �_�h �N����
        if (hit) state = StateEnemy.attack;
        else state = StateEnemy.walk;
    }
    #endregion
}
