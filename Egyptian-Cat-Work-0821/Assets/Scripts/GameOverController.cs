using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// �C����������G
/// 1. �����Ҧ��Ǫ���Ĳ�o�ǰe��
/// 2. ���a���`
/// </summary>
public class GameOverController : MonoBehaviour
{
    [Header("�����e���ʵe����")]
    public Animator aniFinal;
    [Header("�������D")]
    public Text textFinalTitle;
    [Header("�C���ӧQ�P���Ѥ�r")]
    // �r�ꤺ������ \n
    [TextArea(1, 3)]
    public string stringWin = "�A�w�g���\�����Ҧ��Ǫ�...\n�i�H�~�򩹫e�i�F...";
    [TextArea(1, 3)]
    public string stringLose = "�D�ԥ���...\n�A�D�Ԥ@���a...";
    [Header("���s�P���}���s")]
    public KeyCode kcReplay = KeyCode.R;
    public KeyCode kcQuit = KeyCode.Q;

    /// <summary>
    /// �O�_�C������
    /// </summary>
    private bool isGameOver;

    private void Update()
    {
        Replay();
        Quit();
    }

    private void Replay()
    {
        if (isGameOver && Input.GetKeyDown(kcReplay)) SceneManager.LoadScene("GameScene");
    }

    private void Quit()
    {
        if (isGameOver && Input.GetKeyDown(kcQuit)) Application.Quit();
    }

    /// <summary>
    /// ��ܹC�������e��
    /// 1. �]�w���C������
    /// 2. �Ұʰʵe - �H�J
    /// 3. �P�_�ӧQ�Υ��Ѩç�s���D
    /// </summary>
    /// <param name="win"></param>
    public void ShowGameOverView(bool win)
    {
        isGameOver = true;
        aniFinal.enabled = true;

        if (win) textFinalTitle.text = stringWin;
        else textFinalTitle.text = stringLose;
    }
}
