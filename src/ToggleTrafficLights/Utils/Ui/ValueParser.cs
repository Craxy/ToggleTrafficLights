using System;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Utils.Ui
{
  public static class ValueParser
  {
    public static ValueParser<TResult> Create<TResult>(Func<string, Option<TResult>> parse, string initialInput)
    {
      return new ValueParser<TResult>(parse, initialInput);
    }

    public static ValueParser<TResult> Create<TResult>(Func<string, Option<TResult>> parse, TResult initialInput)
    {
      return Create(parse, initialInput.ToString());
    }
  }

  public sealed class ValueParser<TResult>
  {
    private string _input;
    private Option<TResult> _lastParseResult;

    public ValueParser(Func<string, Option<TResult>> parse, string initialInput)
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
      get => _input;
      set
      {
        if (value != Input)
        {
          _input = value;

          LastParseResult = Parse(value);
        }
      }
    }

    public Option<TResult> LastParseResult
    {
      get => _lastParseResult;
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

    public bool IsValidInput => LastParseResult.IsSome();

    public Func<string, Option<TResult>> Parse { get; private set; }
  }

  public static class Parser
  {
    public static Option<float> ParseFloat(string value)
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

    public static Option<int> ParseInt(string value)
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

    public static Option<byte> ParseByte(string value)
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
