using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DependencyMultiChoiceSegment : MultiChoiceSegment
{
    DependentMultiChoiceSegment dependentSegment;

    public DependencyMultiChoiceSegment(string[] c, string s, DependentMultiChoiceSegment d) : base(c, s) {
        dependentSegment = d;
        Debug.Log("Is other segment null? " + (d == null));
    }

    public override void SelectChoice(int choice) {
        selectedChoice = choice;
        dependentSegment.ChangeState(selectedChoice);
    }
}
