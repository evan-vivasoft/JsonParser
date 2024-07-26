namespace JSONParser.InspectionProcedure.ScriptCommandFactory
{
    public interface IScriptCommandFactory
    {
        ScriptCommandEntityBase GetScriptCommandEntity(InspectionScriptCommand command, int maybeAddSequenceNumber);
    }
}
