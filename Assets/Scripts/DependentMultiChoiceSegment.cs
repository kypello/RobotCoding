using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DependentMultiChoiceSegment : MultiChoiceSegment
{
    public string[][] altChoices;
    int state;

    public DependentMultiChoiceSegment(string[] c, string s, string[][] ac) : base(c, s) {
        altChoices = ac;
        choices = altChoices[0];
    }

    public void ChangeState(int newState) {
        state = newState;
        choices = altChoices[newState];
        if (selectedChoice >= choices.Length) {
            selectedChoice = choices.Length - 1;
        }
    }
}
