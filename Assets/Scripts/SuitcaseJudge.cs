using UnityEngine;

public class SuitcaseJudge : MonoBehaviour
{
    public enum Decision
    {
        Legal,
        Bedenklich,
        Illegal
    }

    [SerializeField]
    private DuplicateCurrentSuitcase duplicateSystem;

    private bool alreadyRated = false;

    public void ResetForNewSuitcase()
    {
        alreadyRated = false;
        Debug.Log("Neuer Koffer → Bewertung wieder möglich");
    }

    public void PressGreen()
    {
        Evaluate(Decision.Legal);
    }

    public void PressYellow()
    {
        Evaluate(Decision.Bedenklich);
    }

    public void PressRed()
    {
        Evaluate(Decision.Illegal);
    }

    private void Evaluate(Decision playerChoice)
    {
        // Tutorial Sperre
        if (!TutorialManager.Instance.TutorialFinished)
        {
            Debug.Log("Tutorial noch nicht beendet");
            return;
        }

        if (alreadyRated)
        {
            Debug.Log("Dieser Koffer wurde bereits bewertet");
            return;
        }

        if (duplicateSystem == null)
        {
            Debug.LogError("Duplicate System fehlt!");
            return;
        }

        GameObject suitcase =
            duplicateSystem.GetCurrentCopySuitcase();

        if (suitcase == null)
        {
            Debug.Log("Kein kopierter Koffer vorhanden");
            return;
        }

        bool hasIllegal = false;
        bool hasBedenklich = false;

        Transform[] all =
            suitcase.GetComponentsInChildren<Transform>(true);

        foreach (Transform t in all)
        {
            if (t.CompareTag("Illegal"))
            {
                hasIllegal = true;
            }

            if (t.CompareTag("Bedenklich"))
            {
                hasBedenklich = true;
            }
        }

        Decision correctDecision;

        if (hasIllegal)
        {
            correctDecision = Decision.Illegal;
        }
        else if (hasBedenklich)
        {
            correctDecision = Decision.Bedenklich;
        }
        else
        {
            correctDecision = Decision.Legal;
        }

        alreadyRated = true;

        if (playerChoice == correctDecision)
        {
            Debug.Log("RICHTIG");
            ScoreManager.Instance.AddPoint();
        }
        else
        {
            Debug.Log("FALSCH");
            ScoreManager.Instance.RemovePoint();
        }
    }
}