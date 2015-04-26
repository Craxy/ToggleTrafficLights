﻿using System;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Utils.Ui
{
    public static class ValueParser
    {
        public static ValueParser<TResult> Create<TResult>(Func<string, IOption<TResult>> parse, string initialInput)
        {
            return new ValueParser<TResult>(parse, initialInput);
        }        
        public static ValueParser<TResult> Create<TResult>(Func<string, IOption<TResult>> parse, TResult initialInput)
        {
            return Create(parse, initialInput.ToString());
        }
    }
    public sealed class ValueParser<TResult>
    {
        private string _input;
        private IOption<TResult> _lastParseResult;

        public ValueParser(Func<string, IOption<TResult>> parse, string initialInput)
        {
            Parse = parse;
            
            Input = initialInput;

            if (LastParseResult.IsNone())
            {
                throw new ArgumentException("Initial Input must parse to a valid value.", "initialInput");
            }
        }

        public TResult Value { get; private set; }

        public string Input
        {
            get { return _input; }
            set
            {
                if (value != Input)
                {
                    _input = value;

                    LastParseResult = Parse(value);
                }
            }
        }

        public IOption<TResult> LastParseResult
        {
            get { return _lastParseResult; }
            private set
            {
                if (value == null)
                {
                    throw new ArgumentException("Option can not be null.", "value");
                }

                if (value != LastParseResult)
                {
                    _lastParseResult = value;

                    if (value.IsSome())
                    {
                        Value = value.GetValue();
                    }
                }
            }
        }

        public bool IsValidInput
        {
            get { return LastParseResult.IsSome(); }
        }

        public Func<string, IOption<TResult>> Parse { get; private set; }
    }

    public static class Parser
    {
        public static IOption<float> ParseFloat(string value)
        {
            float v;
            if (float.TryParse(value, out v))
            {
                return Option.Some(v);
            }
            else
            {
                return Option.None<float>();
            }
        }
        public static IOption<int> ParseInt(string value)
        {
            int v;
            if (int.TryParse(value, out v))
            {
                return Option.Some(v);
            }
            else
            {
                return Option.None<int>();
            }
        }         
        public static IOption<byte> ParseByte(string value)
        {
            byte v;
            if (byte.TryParse(value, out v))
            {
                return Option.Some(v);
            }
            else
            {
                return Option.None<byte>();
            }
        } 

    }
}