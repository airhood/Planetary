using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FOROperator
{
    increase, decrease
}

public class NodeCommand
{
    public static void PRINT(object message)
    {
        Debug.Log(message.ToString());
    }

    /*
    public static bool IF(NodeParameter parameter)
    {
        Node content = parameter.value as Node;
        if (content == null)
        {
            if (parameter.dataType == DataType._bool && parameter.type == typeof(bool))
            {
                return (bool)parameter.value;
            }
        }
        else
        {
            NodeParameter nodeParameterA;
            NodeParameter nodeParameterB;
            try
            {
                nodeParameterA = (NodeParameter)content.parameters[0].value;
                nodeParameterB = (NodeParameter)content.parameters[0].value;
            }
            catch (Exception e)
            {
                Log.LogError("NodeCommand.IF: NodeParameter casting error { " + e.Message + " }");
                return false;
            }

            if (content.type == NodeType.ANDOperator)
            {
                bool a = IF(nodeParameterA);
                bool b = IF(nodeParameterB);
                return a && b;
            }
            else if (content.type == NodeType.OROperator)
            {
                bool a = IF(nodeParameterA);
                bool b = IF(nodeParameterB);
                return a || b;
            }
            else
            {
                return checkCondition(content.type, content);
            }
        }

        Log.LogError("NodeCommand.IF: no matching node interpreter");
        return false;

        bool checkCondition(NodeType type, Node condition)
        {
            if (condition.parameters[0].value.GetType() != condition.parameters[1].value.GetType()) return false;
            if (type == NodeType.EqualOperator) return (condition.parameters[0].value == condition.parameters[1].value);
            else if (type == NodeType.NotEqualOperater) return (condition.parameters[0].value != condition.parameters[1].value);

            Log.LogError($"NodeCommands.checkCondition: conidtion check error");
            return false;
        }
    }
    */

    public static void IF(Node node, bool condition)
    {
        if (condition) NodeSystem.RunNodeBracket(node.nodeBrackets[0]);
    }

    public static void IF_ELSE(Node node, bool condition)
    {
        if (condition) NodeSystem.RunNodeBracket(node.nodeBrackets[0]);
        else NodeSystem.RunNodeBracket(node.nodeBrackets[1]);
    }

    public static bool Equals(Node node)
    {
        if (node.parameters[0].value.GetType() != node.parameters[1].value.GetType())
        {
            Log.LogError($"NodeCommands.checkCondition: conidtion check error");
            return false;
        }
        return (node.parameters[0].value == node.parameters[1].value);
    }

    public static bool NotEquals(Node node)
    {
        if (node.parameters[0].value.GetType() != node.parameters[1].value.GetType())
        {
            Log.LogError($"NodeCommands.checkCondition: conidtion check error");
            return false;
        }
        return (node.parameters[0].value != node.parameters[1].value);
    }

    public static void FOR(Node node)
    {
        Node variableDeclaration;
        Node condition;
        FOROperator forOperator;
    }

    public static void ADD(Node node)
    {

    }

    public static void DECLEAR_VARIABLE(string variableName, DataType dataType)
    {
        VariableManager.DeclearVariable(variableName, dataType, null);
    }

    public static void DECLEAR_VARIABLE(string variableName, DataType dataType, object value)
    {
        VariableManager.DeclearVariable(variableName, dataType, value);
    }
}
