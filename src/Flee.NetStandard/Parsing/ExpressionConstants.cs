using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Flee.Parsing
{
    ///<remarks>
    /// An enumeration with token and production node
    ///constants.</remarks>
    internal enum ExpressionConstants
    {
        ADD = 1001,
        SUB = 1002,
        MUL = 1003,
        DIV = 1004,
        POWER = 1005,
        MOD = 1006,
        LEFT_PAREN = 1007,
        RIGHT_PAREN = 1008,
        LEFT_BRACE = 1009,
        RIGHT_BRACE = 1010,
        EQ = 1011,
        LT = 1012,
        GT = 1013,
        LTE = 1014,
        GTE = 1015,
        NE = 1016,
        AND = 1017,
        OR = 1018,
        XOR = 1019,
        NOT = 1020,
        IN = 1021,
        BETWEEN = 1022,
        DOT = 1023,
        ARGUMENT_SEPARATOR = 1024,
        LEFT_SHIFT = 1025,
        RIGHT_SHIFT = 1026,
        WHITESPACE = 1027,
        INTEGER = 1028,
        REAL = 1029,
        STRING_LITERAL = 1030,
        CHAR_LITERAL = 1031,
        TRUE = 1032,
        FALSE = 1033,
        IDENTIFIER = 1034,
        HEX_LITERAL = 1035,
        NULL_LITERAL = 1036,
        TIMESPAN = 1037,
        DATETIME = 1038,
        EXPRESSION = 2001,
        XOR_EXPRESSION = 2002,
        OR_EXPRESSION = 2003,
        AND_EXPRESSION = 2004,
        NOT_EXPRESSION = 2005,
        BETWEEN_EXPRESSION = 2006,
        IN_EXPRESSION = 2007,
        IN_TARGET_EXPRESSION = 2008,
        IN_LIST_TARGET_EXPRESSION = 2009,
        COMPARE_EXPRESSION = 2010,
        SHIFT_EXPRESSION = 2011,
        ADDITIVE_EXPRESSION = 2012,
        MULTIPLICATIVE_EXPRESSION = 2013,
        POWER_EXPRESSION = 2014,
        NEGATE_EXPRESSION = 2015,
        MEMBER_EXPRESSION = 2016,
        MEMBER_ACCESS_EXPRESSION = 2017,
        BASIC_EXPRESSION = 2018,
        MEMBER_FUNCTION_EXPRESSION = 2019,
        FIELD_PROPERTY_EXPRESSION = 2020,
        INDEX_EXPRESSION = 2021,
        FUNCTION_CALL_EXPRESSION = 2022,
        ARGUMENT_LIST = 2023,
        LITERAL_EXPRESSION = 2024,
        BOOLEAN_LITERAL_EXPRESSION = 2025,
        EXPRESSION_GROUP = 2026
    }
}
