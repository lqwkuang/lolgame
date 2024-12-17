using ProtoMsg;
using UnityEngine;


public enum PlayerAnimationClip
{
    None,
    Idle,
    Run,
    Dance,
    Dead,
    SkillQ,
    SkillW,
    SkillE,
    SkillR,
    NormalAttack,
}
public class AnimatorManager : MonoBehaviour
{
    PlayerCtrl playerCtrl;
    PlayerInfo playerInfo;
    Animator animator;
    public void Init(PlayerCtrl playerCtrl)
    {
        this.playerCtrl = playerCtrl;
        playerInfo = playerCtrl.playerInfo;
        animator = transform.GetComponent<Animator>();
    }
    //���Ŷ���
    public void Play(PlayerAnimationClip clip)
    {
        ReserState();
        animator.SetBool(clip.ToString(), true);
    }
    public string[] clips = new string[] { "None", "Idle", "Run", "Dead", "SkillQ", "SkillW", "SkillE", "SkillR", "NormalAttack", "Dance" };
    //����״̬
    void ReserState()
    {
        for (int i = 0; i < clips.Length; i++)
        {
            animator.SetBool(clips[i], false);
        }
    }
    //�¼�����
    //Q
    public void DoSkillQEvent()
    {
        SpawnEffect("Q");
    }
    //W
    public void DoSkillWEvent()
    {
        SpawnEffect("W");
    }

    //E
    public void DoSkillEEvent()
    {
        SpawnEffect("E");
    }
    //R
    public void DoSkillREvent()
    {
        SpawnEffect("R");
    }
    //A
    public void DoSkillAEvent()
    {
        SpawnEffect("A");
    }

    //������Ч
    public void SpawnEffect(string key)
    {
        GameObject effect = GamePool.Instance.GetGameobject($"{playerCtrl.HeroID}" + $"{key}");
        effect.transform.position = transform.position;
        effect.transform.eulerAngles = transform.eulerAngles;
        effect.gameObject.SetActive(true);
        EConfig eConfig = effect.GetComponent<EConfig>();
        eConfig.moveRoot.position = transform.position + new Vector3(0, 1, 0);
        eConfig.moveRoot.eulerAngles = transform.eulerAngles;
        BattleUserInputC2S skillCMD = playerCtrl.playerFSM.skillCMD.CMD;
        //playerCtrl.OnSkillTrriger ���ܴ����ص�
        switch (key)
        {
            case "Q":
                eConfig.Init(skillCMD.RolesID, skillCMD.LockTag, skillCMD.LockID, transform.forward,
            transform.position, playerCtrl.OnSkillQTrriger);
                GamePool.Instance.Recely($"{playerCtrl.HeroID}Q", eConfig.gameObject, eConfig.destroyTime);
                break;
            case "W":
                eConfig.Init(skillCMD.RolesID, skillCMD.LockTag, skillCMD.LockID, transform.forward,
            transform.position, playerCtrl.OnSkillWTrriger);
                GamePool.Instance.Recely($"{playerCtrl.HeroID}W", eConfig.gameObject, eConfig.destroyTime);
                break;
            case "E":
                eConfig.Init(skillCMD.RolesID, skillCMD.LockTag, skillCMD.LockID, transform.forward,
            transform.position, playerCtrl.OnSkillETrriger);
                GamePool.Instance.Recely($"{playerCtrl.HeroID}E", eConfig.gameObject, eConfig.destroyTime);
                break;
            case "R":
                eConfig.Init(skillCMD.RolesID, skillCMD.LockTag, skillCMD.LockID, transform.forward,
            transform.position, playerCtrl.OnSkillRTrriger);
                GamePool.Instance.Recely($"{playerCtrl.HeroID}R", eConfig.gameObject, eConfig.destroyTime);
                break;
            case "A":
                eConfig.Init(skillCMD.RolesID, skillCMD.LockTag, skillCMD.LockID, transform.forward,
            transform.position, playerCtrl.OnSkillATrriger);
                GamePool.Instance.Recely($"{playerCtrl.HeroID}A", eConfig.gameObject, eConfig.destroyTime);
                break;
            default:
                break;
        }
    }

    //�����¼�
    public void EndSkill()
    {
        Debug.Log("�����ͷŽ���");
        playerCtrl.playerFSM.ToNext(FSMState.Idle);
    }
    // Update is called once per frame
    void Update()
    {

    }
}
