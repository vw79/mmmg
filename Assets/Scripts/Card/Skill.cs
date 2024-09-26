using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Skill
{
    public string skillName;
    public ActionType requiredActionType; // Action type required to activate the skill
    public string skillDescription;
    public int skillCost;

    public Skill(string skillName, ActionType requiredActionType, string skillDescription, int skillCost)
    {
        this.skillName = skillName;
        this.requiredActionType = requiredActionType;
        this.skillDescription = skillDescription;
        this.skillCost = skillCost;
    }
}

