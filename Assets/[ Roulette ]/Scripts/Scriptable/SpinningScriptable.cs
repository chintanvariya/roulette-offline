using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


[CreateAssetMenu(fileName = "New Roullete", menuName = "Scriptable/Game Roullete")]
public class SpinningScriptable : ScriptableObject
{
    public enum state
    {
        NumberPleno,
        NumberMiddle,
        NumberCorner,
        NumberRow,
        Dozen,
        Column,
        Even,
        Eighteenth1,
        Black,
        ButtomCorner,
        Three,
        Red,
        Odd,
        Eighteenth2
    }

    public state stateSet;
    [FormerlySerializedAs("pieceValue")] public List<string> pieceNumber;

    public int winningMuliply;
}
