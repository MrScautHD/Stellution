using Easel.Math;

namespace Easel.GUI.BBCode;

public class ColorInstruction : BBCodeInstruction
{
    public Color Color;
    
    public ColorInstruction(Color color, bool isExiting) : base(InstructionType.Color, isExiting)
    {
        Color = color;
    }

    public override string ToString()
    {
        return "COLOR" + (IsExiting ? " EXITING: " : ": ") + Color;
    }
}