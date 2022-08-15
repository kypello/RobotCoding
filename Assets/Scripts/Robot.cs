using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot : MovingElement
{
    public CodeBlock rootBlock;
    bool breakFlag = false;

    public float rotationSpeed = 90f;
    bool cancelExecution = false;

    bool paused = false;

    bool runAttemptInQueue = false;

    public enum State {Running, Stopping, Stopped}

    public State state = State.Stopped;

    int currentLine;

    public Transform cratePoint;
    MovingElement crate = null;

    void Awake() {
        rootBlock = new CodeBlock();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.E)) {
            StartCoroutine(RunCode());
        }
    }

    public void StopRunningCode() {
        state = State.Stopping;
    }

    public int GetCurrentLine() {
        return currentLine;
    }

    public IEnumerator RunCode() {
        if (state == State.Running) {
            yield break;
        }

        if (runAttemptInQueue) {
            yield break;
        }

        while (state == State.Stopping) {
            runAttemptInQueue = true;
            yield return null;
        }
        runAttemptInQueue = false;

        Debug.Log("Starting Run");

        state = State.Running;
        breakFlag = false;
        yield return ExecuteCodeBlock(rootBlock, 0);
        state = State.Stopped;
        Debug.Log("Code done running");
    }

    IEnumerator ExecuteCodeBlock(CodeBlock codeBlock, int startingLine, bool skipToElse = false) {
        Debug.Log("Starting coroutine (is cancelExecution already true?) " + cancelExecution);
        int startPoint = 0;
        if (skipToElse) {
            while (startPoint < codeBlock.statements.Count && codeBlock.statements[startPoint].id != "else") {
                startPoint++;
            }
            startPoint++;
        }
        for (int i = startPoint; i < codeBlock.statements.Count; i++) {
            if (state == State.Stopping) {
                break;
            }

            Statement statement = codeBlock.statements[i];
            currentLine = startingLine + i;

            switch (statement.id) {
                case "break":
                    breakFlag = true;
                    i = codeBlock.statements.Count;
                    break;

                case "continue":
                    i = codeBlock.statements.Count;
                    break;

                case "dance":
                    yield return Dance();
                    break;

                case "else":
                    i = codeBlock.statements.Count;
                    break;

                case "move":
                    yield return Move(System.Int32.Parse(statement.highlightableSegments[0].GetText()));
                    break;

                case "turnright":
                    yield return Rotate(1f);
                    break;

                case "turnleft":
                    yield return Rotate(-1f);
                    break;

                case "wait":
                    yield return Wait(System.Int32.Parse(statement.highlightableSegments[0].GetText()));
                    break;

                case "forever":
                    while (!breakFlag && state == State.Running) {
                        yield return ExecuteCodeBlock(statement.codeBlock, startingLine + i + 1);
                    }
                    breakFlag = false;
                    break;

                case "repeat":
                    int loops = System.Int32.Parse(statement.highlightableSegments[0].GetText());
                    for (int loop = 0; loop < loops; loop++) {
                        yield return ExecuteCodeBlock(statement.codeBlock, startingLine + i + 1);
                        if (breakFlag || state == State.Stopping) {
                            loop = loops;
                            breakFlag = false;
                        }
                    }
                    break;

                case "if":
                    if (EvaluateCondition(statement)) {
                        yield return ExecuteCodeBlock(statement.codeBlock, startingLine + i + 1);
                        breakFlag = false;
                    }
                    break;

                case "ifelse":
                    yield return ExecuteCodeBlock(statement.codeBlock, startingLine + i + 1, !EvaluateCondition(statement));
                    breakFlag = false;
                    break;

                case "while":
                    while (EvaluateCondition(statement) && !breakFlag && state == State.Running) {
                        yield return ExecuteCodeBlock(statement.codeBlock, startingLine + i + 1);
                    }
                    breakFlag = false;
                    break;

                case "pickup":
                    yield return PickUp();
                    break;

                case "drop":
                    yield return Drop();
                    break;
            }
        }
        Debug.Log("End of executing code block");
        yield break;
    }

    IEnumerator PickUp() {
        if (crate != null) {
            yield break;
        }

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 2f, 1<<7)) {
            if (hit.collider.gameObject.tag == "Crate") {
                crate = hit.collider.GetComponent<MovingElement>();
                crate.transform.SetParent(cratePoint);
                crate.transform.localPosition = Vector3.zero;
                crate.transform.localEulerAngles = Vector3.zero;
            }
        }
        float timePassed = 0f;
        while (timePassed < 0.5f) {
            timePassed += Time.deltaTime * timeScale;
            yield return null;
        }
    }

    IEnumerator Drop() {
        if (crate == null) {
            yield break;
        }

        crate.transform.SetParent(null);
        yield return crate.MoveOneSpace(1f);
        crate = null;
    }

    IEnumerator Wait(int seconds) {
        float timePassed = 0f;
        while (timePassed < seconds) {
            yield return null;
            timePassed += Time.deltaTime * timeScale;
        }
    }

    IEnumerator Dance() {
        yield return Rotate(1f);
        yield return Rotate(1f);
        yield return Rotate(1f);
        yield return Rotate(1f);
    }

    IEnumerator Move(int spaces) {
        for (int i = 0; i < Mathf.Abs(spaces); i++) {
            if (state == State.Stopping) {
                break;
            }

            yield return MoveOneSpace(Mathf.Sign(spaces));
        }
    }



    IEnumerator Rotate(float direction) {
        float amountRotated = 0f;
        Vector3 targetRotation = transform.eulerAngles + Vector3.up * 90f * direction;
        while (amountRotated < 90f) {
            yield return null;
            float rotation = rotationSpeed * Time.deltaTime * timeScale;
            transform.Rotate(Vector3.up * direction * rotation);
            amountRotated += rotation;
        }
        transform.eulerAngles = targetRotation;
    }

    bool EvaluateCondition(Statement statement) {
        return EvaluateConditionSpecifics(StatementSlot.CleanString(statement.highlightableSegments[0].GetText()), StatementSlot.CleanString(statement.highlightableSegments[1].GetText()) == "== ", StatementSlot.CleanString(statement.highlightableSegments[2].GetText()));
    }

    bool EvaluateConditionSpecifics(string checkType, bool equality, string checkTarget) {
        Debug.Log("Checking condition");
        if (checkType == "checkBelow() ") {
            return !equality;
        }

        if (checkTarget == "wall") {
            return Physics.Raycast(transform.position, transform.forward, 2f, 1<<7) == equality;
        }
        else if (checkTarget == "pit") {
            return (!Physics.Raycast(transform.position, transform.forward, 2f, 1<<7) && !Physics.Raycast(transform.position + transform.forward * 2f, -transform.up, 2f, 1<<7)) == equality;
        }
        return false;
    }
}
