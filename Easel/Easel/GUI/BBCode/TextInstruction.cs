namespace Easel.GUI.BBCode;

public class TextInstruction : BBCodeInstruction
{
    public string Text;

    public TextInstruction(string text) : base(InstructionType.Text, false)
    {
        Text = text;
    }

    public override string ToString()
    {
        return $"TEXT: {Text}";
    }
}