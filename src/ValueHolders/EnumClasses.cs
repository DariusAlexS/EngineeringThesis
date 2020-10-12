public enum PlayerType {AI, Human, Neutral}

public enum ActionType {None, ProduceGold, ProduceSoldiers, UpgradeStructure, MoveToCommand}
public enum MoveToAIActionType {None, Reinforcements, Attack}
public enum StructureType {Castle, GoldMine};
public class Word
{
    public string typingString;
    public int stringLength;

    public Word(string toType, int length)
    {
        this.typingString = toType;
        this.stringLength = length;
    }
}