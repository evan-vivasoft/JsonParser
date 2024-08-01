using System;

namespace Inspector.POService.InspectionProcedure.ScriptCommandFactory
{
    public static class ScriptCommanFactoryProvider
    {
        public static ScriptCommandEntityBase GetScriptCommandEntity(InspectionScriptCommand command, int maybeAddSequenceNumber = 0)
        {
            IScriptCommandFactory factory = null;

            switch (command.command_type)
            {
                case "Scriptcommand_1":
                    factory = new ScriptCommand1Factory();
                    break;
                case "Scriptcommand_3":
                    factory = new ScriptCommand3Factory();
                    break;
                case "Scriptcommand_4":
                    factory = new ScriptCommand4Factory();
                    break;
                case "Scriptcommand_41":
                    factory = new ScriptCommand41Facotry();
                    break;
                case "Scriptcommand_42":
                    factory = new ScritpCommand42Factory();
                    break;
                case "Scriptcommand_43":
                    factory = new ScriptCommand43Factory();
                    break;
                case "Scriptcommand_51":
                case "Scriptcommand_52":
                case "Scriptcommand_53":
                case "Scriptcommand_54":
                case "Scriptcommand_55":
                case "Scriptcommand_56":
                case "Scriptcommand_57":
                    factory = new ScriptCommand5xFactory();
                    break;
                default:
                    throw new ArgumentException($"No Script command found of type ${command.command_type}");
            }

            return factory.GetScriptCommandEntity(command, maybeAddSequenceNumber);
        }
    }
}
