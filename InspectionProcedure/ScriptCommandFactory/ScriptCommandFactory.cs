using System;
using System.Collections.Generic;
using System.Linq;

namespace Inspector.POService.InspectionProcedure.ScriptCommandFactory
{
    internal class ScriptCommand1Factory: IScriptCommandFactory
    {
        public ScriptCommandEntityBase GetScriptCommandEntity(InspectionScriptCommand command, int maybeAddSequenceNumber = 0)
        {
            var scriptCommand = (command.data);
            return (
                new ScriptCommand1Entity()
                {
                    SequenceNumber = command.sequence + maybeAddSequenceNumber,
                    Text = scriptCommand.instruction
                }
            );
        }
    }

    internal class ScriptCommand3Factory : IScriptCommandFactory
    {
        public ScriptCommandEntityBase GetScriptCommandEntity(InspectionScriptCommand command, int maybeAddSequenceNumber = 0)
        {
            var scriptCommand = (command.data);
            return new ScriptCommand3Entity
            {
                SequenceNumber = command.sequence + maybeAddSequenceNumber,
                Text = scriptCommand.instruction,
                Duration = scriptCommand.duration
            };
        }
    }

    internal class ScriptCommand4Factory : IScriptCommandFactory
    {
       public ScriptCommandEntityBase GetScriptCommandEntity(InspectionScriptCommand command, int maybeAddSequenceNumber = 0)
        {
            var scriptCommand4Json = (command.data);

            var scriptCommand =
                new ScriptCommand4Entity
                {
                    SequenceNumber = command.sequence + maybeAddSequenceNumber,
                    ObjectName = scriptCommand4Json.object_name,
                    MeasurePoint = scriptCommand4Json.measure_point,
                    Question = scriptCommand4Json.question,
                    TypeQuestion = Helper.GetEnumValueFromDescription<TypeQuestion>(scriptCommand4Json.type_of_question),
                    TextOptions = new List<string>(),
                    InspectionPointId = scriptCommand4Json.inspection_point_id,
                    Required = scriptCommand4Json.required
                };
            if (!string.IsNullOrEmpty(scriptCommand4Json.option_1))
            {
                scriptCommand.TextOptions.Add(scriptCommand4Json.option_1);
            }

            if (!string.IsNullOrEmpty(scriptCommand4Json.option_2))
            {
                scriptCommand.TextOptions.Add(scriptCommand4Json.option_2);
            }

            if (!string.IsNullOrEmpty(scriptCommand4Json.option_3))
            {
                scriptCommand.TextOptions.Add(scriptCommand4Json.option_3);
            }

            return scriptCommand;
        }
    }

    internal class ScriptCommand41Facotry: IScriptCommandFactory
    {
        public ScriptCommandEntityBase GetScriptCommandEntity(InspectionScriptCommand command, int maybeAddSequenceNumber = 0)
        {
            var scriptCommand41Json = (command.data);
            var scriptCommand =
                new ScriptCommand41Entity
                {
                    SequenceNumber = command.sequence + maybeAddSequenceNumber,
                    ObjectName = scriptCommand41Json.object_name,
                    MeasurePoint = scriptCommand41Json.measure_point,
                    ShowNextListImmediatly = scriptCommand41Json.show_next_list_immediately,
                    InspectionPointId = scriptCommand41Json.inspection_point_id,
                    ScriptCommandList = new List<ScriptCommand41ListEntity>()
                };

            var listType = scriptCommand41Json.list_type;
            var index = 1;

            foreach (var listItem in scriptCommand41Json.lists)
            {
                List<ListConditionCodeEntity> listConditionCode = new List<ListConditionCodeEntity>();

                foreach (var conditionCode in listItem.condition_codes)
                {
                    var conditionCodeEntity = new ListConditionCodeEntity
                    {
                        ConditionCode = conditionCode.condition_code,
                        ConditionCodeDescription = conditionCode.condition_code_description,
                        DisplayNextList = !conditionCode.do_not_display_next_list
                    };
                    listConditionCode.Add(conditionCodeEntity);
                }

                ScriptCommand41ListEntity listEntity = new ScriptCommand41ListEntity
                {
                    SequenceNumberList = index++,
                    ListType = listType,
                    SelectionRequired = listItem.selection_required,
                    ListQuestion = listItem.list_question,
                    OneSelectionAllowed = listItem.one_selection,
                    CheckListResult = listItem.saving_all_condition_codes,
                    ListConditionCodes = listConditionCode
                };
                scriptCommand.ScriptCommandList.Add(listEntity);
            }

            return scriptCommand;
        }
    }

    internal class ScritpCommand42Factory: IScriptCommandFactory
    {
        public ScriptCommandEntityBase GetScriptCommandEntity(InspectionScriptCommand command, int maybeAddSequenceNumber = 0)
        {

            var scriptCommand42Json = (command.data);

            return (
                new ScriptCommand42Entity
                {
                    ObjectName = scriptCommand42Json.object_name,
                    MeasurePoint = scriptCommand42Json.measure_point,
                    InspectionPointId = scriptCommand42Json.inspection_point_id,
                    SequenceNumber = command.sequence + maybeAddSequenceNumber
                }
            );
        }
    }

    internal class ScriptCommand43Factory: IScriptCommandFactory
    {
        public ScriptCommandEntityBase GetScriptCommandEntity(InspectionScriptCommand command, int maybeAddSequenceNumber = 0)
        {
            var scriptCommand43Json = command.data;

            return (
                new ScriptCommand43Entity
                {
                    SequenceNumber = command.sequence + maybeAddSequenceNumber,
                    ObjectName = scriptCommand43Json.object_name,
                    MeasurePoint = scriptCommand43Json.measure_point,
                    Instruction = scriptCommand43Json.instruction,
                    ListItems = scriptCommand43Json.items.ToList(),
                    InspectionPointId = scriptCommand43Json.inspection_point_id,
                    Required = scriptCommand43Json.required
                }
            );
        }
    }

    internal class ScriptCommand5xFactory: IScriptCommandFactory
    {
        public ScriptCommandEntityBase GetScriptCommandEntity(InspectionScriptCommand command, int maybeAddSequenceNumber = 0)
        {
            var scriptCommand5xJson = (command.data);
            return (
                new ScriptCommand5XEntity
                {
                    SequenceNumber = command.sequence + maybeAddSequenceNumber,
                    ObjectName = scriptCommand5xJson.object_name,
                    MeasurePoint = scriptCommand5xJson.measure_point,
                    ScriptCommand5X = Helper.GetEnumValueFromDescription<ScriptCommand5XType>(command.command_type.Replace("Scriptcommand_", "")),
                    Instruction = scriptCommand5xJson.instruction,
                    DigitalManometer = Helper.GetEnumValueFromDescription<DigitalManometer>(scriptCommand5xJson.digital_manometer),
                    MeasurementFrequency = Helper.GetEnumValueFromDescription<TypeMeasurementFrequency>(scriptCommand5xJson.measuring_frequency.ToString()),
                    MeasurementPeriod = (int)(scriptCommand5xJson.measuring_period),
                    ExtraMeasurementPeriod = (int)scriptCommand5xJson.extra_measuring_period,
                    InspectionPointId = scriptCommand5xJson.inspection_point_id,
                    Leakage = Helper.GetEnumValueFromDescription<Leakage>(!string.IsNullOrEmpty(scriptCommand5xJson.leakage_amount) ? scriptCommand5xJson.leakage_amount : "-")
                }
            );
        }
    }

}
