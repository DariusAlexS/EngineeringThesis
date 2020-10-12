
public class AIAction 
{
    public ActionType aiActionType;
    public MoveToAIActionType moveToActionType;
    public Structure sourceStructure;
    public Structure destinationStructure;
    public int priority;

    public AIAction(ActionType actionType, Structure source, int priority)
    {
        this.aiActionType = actionType;
        this.sourceStructure = source;
        this.destinationStructure = null;
        this.moveToActionType = MoveToAIActionType.None;
        this.priority = priority;
    }
    public AIAction(ActionType actionType, Structure source, Structure dest, MoveToAIActionType moveToAIActionType, int priority)
    {
        this.aiActionType = actionType;
        this.sourceStructure = source;
        this.destinationStructure = dest;
        this.moveToActionType = moveToAIActionType;
        this.priority = priority;
    }


}
