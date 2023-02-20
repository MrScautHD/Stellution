namespace Easel.GUI.BBCode;

public class BBCodeInstruction
{
    public InstructionType InstructionType;

    public bool IsExiting;

    public BBCodeInstruction(InstructionType instructionType, bool isExiting)
    {
        InstructionType = instructionType;
        IsExiting = isExiting;
    }

    public override string ToString()
    {
        return InstructionType.ToString().ToUpper() + (IsExiting ? " EXITING" : "");
    }
}