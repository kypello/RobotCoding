using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot : MonoBehaviour
{
    public CodeBlock codeBlock;

    void Awake() {
        codeBlock = new CodeBlock();
    }
}
