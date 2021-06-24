using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillController : GameBehaviour
{
    public class SkillTask
    {
        public Skill skill { get; private set; }
        public Task task { get; private set; }
        public float timer { get; private set; } = 0;
        public float progress { get { return Mathf.Clamp01(timer / skill.skillTime); } }

        public SkillTask(Skill s)
        {
            skill = s;
            task = new Task(CastSkill());
        }

        private IEnumerator CastSkill()
        {
            skill.CastSkill();

            while (timer < skill.skillTime)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            skill.UncastSkill();
            skill.Remove();
        }

        public void StopSkill()
        {
            skill.UncastSkill();
            skill.Remove();
            task.Stop();
        }
    }

    [SerializeField]
    private int maxSkillCount;
    private List<Skill> skills = new List<Skill>();
    public SkillTask currentSkillTask { get; private set; } = null;

    public ObjectEvent<List<Skill>> OnUpdateSkillList { get; } = new ObjectEvent<List<Skill>>();
    public GameEvent OnActiveSkill { get; } = new GameEvent();
    public GameEvent OnDeactiveSkill { get; } = new GameEvent();

    public void UpdateController()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
            CreateSkillTask();
        if (Input.GetKeyDown(KeyCode.E))
            SwitchSkill();
    }

    private void CreateSkillTask()
    {
        if (skills.Count == 0)
            return;

        if (skills.Count == 1 && currentSkillTask != null && currentSkillTask.task.Running)
            return;

        if (currentSkillTask != null && currentSkillTask.task.Running)
        {
            //為甚麼Finished沒有確保事件觸發的原子性...= =（簡單來說就是Task stop到事件觸發之間可能會發生其他事情）
            currentSkillTask.task.Finished -= OnSkillStop;
            currentSkillTask.StopSkill();
            OnSkillStop(true);
        }

        currentSkillTask = new SkillTask(skills[0]);
        currentSkillTask.task.Finished += OnSkillStop;
        //skills.RemoveAt(0);
        //TODO: 廣播技能開始的事件
        OnActiveSkill.Invoke();
    }

    private void SwitchSkill()
    {
        if (skills.Count <= 1)
            return;
        if (currentSkillTask != null && currentSkillTask.task.Running)
            return;

        Skill temp = null;
        for(int i = 0; i < skills.Count - 1; i++)
        {
            temp = skills[i];
            skills[i] = skills[i + 1];
            skills[i + 1] = temp;
        }
        //TODO: 廣播技能列變動的事件
        OnUpdateSkillList.Invoke(skills);
    }

    public void AddSkill(Skill s)
    {
        if (currentSkillTask != null && currentSkillTask.task.Running)
            skills.Insert(1, s);
        else
            skills.Insert(0, s);
        while (skills.Count > maxSkillCount)
        {
            skills[skills.Count - 1].Remove();
            skills.RemoveAt(skills.Count - 1);
        }
        //TODO: 廣播技能列變動的事件
        OnUpdateSkillList.Invoke(skills);
    }

    private void OnSkillStop(bool manual)
    {
        skills.RemoveAt(0);
        //TODO: 廣播技能列變動的事件
        OnDeactiveSkill.Invoke();
        OnUpdateSkillList.Invoke(skills);
    }
}
