namespace Inspector.POService.InspectionProcedure.ScriptCommandFactory
{
    public interface IScriptCommandFactory
    {
        ScriptCommandEntityBase GetScriptCommandEntity(InspectionScriptCommand command, int maybeAddSequenceNumber);
    }
}
