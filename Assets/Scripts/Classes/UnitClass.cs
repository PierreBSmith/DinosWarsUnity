using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "UnitClass")]
public class UnitClass : ScriptableObject
{
    public string className;
    public Skill keystoneSkill1;
    public Skill keystoneSkill2;
    public List<Skill> skillTree1_1;
    public List<Skill> skillTree1_2;
    public List<Skill> skillTree2_1;
    public List<Skill> skillTree2_2;
}
