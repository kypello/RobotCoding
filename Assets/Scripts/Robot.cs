using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot : MovingElement
{
    public static CodeBlock rootBlock = new CodeBlock();
    bool breakFlag = false;
    bool continueFlag = false;

    public float rotationSpeed = 90f;
    bool cancelExecution = false;

    //bool paused = false;

    bool runAttemptInQueue = false;

    public enum State {Running, Stopping, Stopped}

    public State state = State.Stopped;

    int currentLine;
    public AudioSource percussion;
    public AudioSource movingSound;

    public Transform cratePoint;
    MovingElement crate = null;

    void Awake() {
        //rootBlock = new CodeBlock();
    }

    public void StopRunningCode() {
        state = State.Stopping;
        percussion.volume = 0f;
    }

    public int GetCurrentLine() {
        return currentLine;
    }

    public static void ResetCode() {
        rootBlock = new CodeBlock();
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
        percussion.volume = 0.6f;
        breakFlag = false;
        yield return ExecuteCodeBlock(rootBlock, 0);
        state = State.Stopped;
        percussion.volume = 0f;
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
                    continueFlag = true;
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
                    continueFlag = false;
                    break;

                case "repeat":
                    int loops = System.Int32.Parse(statement.highlightableSegments[0].GetText());
                    for (int loop = 0; loop < loops && !breakFlag && state == State.Running; loop++) {
                        yield return ExecuteCodeBlock(statement.codeBlock, startingLine + i + 1);
                    }
                    breakFlag = false;
                    continueFlag = false;
                    break;

                case "while":
                    while (EvaluateCondition(statement) && !breakFlag && state == State.Running) {
                        yield return ExecuteCodeBlock(statement.codeBlock, startingLine + i + 1);
                    }
                    breakFlag = false;
                    continueFlag = false;
                    break;

                case "if":
                    if (EvaluateCondition(statement)) {
                        yield return ExecuteCodeBlock(statement.codeBlock, startingLine + i + 1);
                        if (breakFlag || continueFlag) {
                            i = codeBlock.statements.Count;
                        }
                    }
                    break;

                case "ifelse":
                    yield return ExecuteCodeBlock(statement.codeBlock, startingLine + i + 1, !EvaluateCondition(statement));
                    if (breakFlag || continueFlag) {
                        i = codeBlock.statements.Count;
                    }
                    break;

                case "pickup":
                    yield return PickUp();
                    break;

                case "drop":
                    yield return Drop();
                    break;
            }

            yield return null;
        }
        Debug.Log("End of executing code block");
        yield break;
    }

    IEnumerator PickUp() {
        if (crate != null) {
            yield break;
        }

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 2f, 1<<7) && hit.collider.gameObject.tag == "Crate") {
            crate = hit.collider.GetComponent<MovingElement>();

            int stacked = 0;
            while (Physics.Raycast(transform.position + transform.forward + transform.up * stacked * 2f, transform.up, out hit, 2f, 1<<7) && hit.collider.gameObject.tag == "Crate") {
                crate = hit.collider.GetComponent<MovingElement>();
                stacked++;
            }

            crate.transform.SetParent(cratePoint);
            crate.transform.localPosition = Vector3.zero;
            crate.transform.localEulerAngles = Vector3.zero;
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

        bool crateBlocked = Physics.Raycast(transform.position + Vector3.up * 2f, transform.forward * 2f, 2f, 1<<7);

        if (!crateBlocked) {
            crate.transform.SetParent(null);
        }

        yield return crate.MoveOneSpace(1f);

        if (!crateBlocked) {
            crate = null;
        }
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
        movingSound.Play();
        for (int i = 0; i < Mathf.Abs(spaces); i++) {
            if (state == State.Stopping) {
                break;
            }
            yield return MoveOneSpace(Mathf.Sign(spaces));
        }
        movingSound.Stop();
    }



    IEnumerator Rotate(float direction) {
        movingSound.Play();
        float amountRotated = 0f;
        Vector3 targetRotation = transform.eulerAngles + Vector3.up * 90f * direction;
        while (amountRotated < 90f) {
            yield return null;
            float rotation = rotationSpeed * Time.deltaTime * timeScale;
            transform.Rotate(Vector3.up * direction * rotation);
            amountRotated += rotation;
        }
        transform.eulerAngles = targetRotation;
        movingSound.Stop();
    }

    bool EvaluateCondition(Statement statement) {
        return EvaluateConditionSpecifics(StatementSlot.CleanString(statement.highlightableSegments[0].GetText()), StatementSlot.CleanString(statement.highlightableSegments[1].GetText()) == "== ", StatementSlot.CleanString(statement.highlightableSegments[2].GetText()));
    }

    bool EvaluateConditionSpecifics(string checkType, bool equality, string checkTarget) {
        Debug.Log("Checking condition");
        if (checkType == "checkBelow() ") {
            if (checkTarget == "button") {
                return CheckForButtonBelow(transform.position) == equality;
            }
            else if (checkTarget == "crate") {
                return CheckForCrateBelow(transform.position) == equality;
            }
            else if (checkTarget == "metal") {
                return (!CheckForButtonBelow(transform.position) && !CheckForCrateBelow(transform.position)) == equality;
            }
            return !equality;
        }
        else if (checkType == "checkInFront() ") {
            if (checkTarget == "button") {
                return CheckForButtonBelow(transform.position + transform.forward * 2f) == equality;
            }
            else if (checkTarget == "crate") {
                return CheckForCrateAhead() == equality;
            }
            else if (checkTarget == "metal") {
                return (!CheckForButtonBelow(transform.position + transform.forward * 2f) && !CheckForCrateBelow(transform.position + transform.forward * 2f) && !CheckForWallAhead() && CheckForFloorBelow(transform.position + transform.forward * 2f)) == equality;
            }
            else if (checkTarget == "wall") {
                return CheckForWallAhead() == equality;
            }
            else if (checkTarget == "pit") {
                return !CheckForFloorBelow(transform.position + transform.forward * 2f) == equality;
            }
        }

        return false;
    }

    bool CheckForButtonBelow(Vector3 position) {
        RaycastHit hit;
        return Physics.Raycast(position, Vector3.down, out hit, 2f, 1<<8, QueryTriggerInteraction.Collide);
    }

    bool CheckForCrateBelow(Vector3 position) {
        RaycastHit hit;
        return Physics.Raycast(position, Vector3.down, out hit, 2f, 1<<7) && hit.collider.gameObject.tag == "Crate";
    }

    bool CheckForFloorBelow(Vector3 position) {
        return Physics.Raycast(position, Vector3.down, 2f, 1<<7);
    }

    bool CheckForCrateAhead() {
        RaycastHit hit;
        return Physics.Raycast(transform.position, transform.forward, out hit, 2f, 1<<7) && hit.collider.gameObject.tag == "Crate";
    }

    bool CheckForWallAhead() {
        return Physics.Raycast(transform.position, transform.forward, 2f, 1<<7);
    }
}
