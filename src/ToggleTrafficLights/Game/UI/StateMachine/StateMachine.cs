using System;
using System.Collections.Generic;
using System.Linq;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Game.UI.StateMachine
{
    //http://stackoverflow.com/questions/5923767/simple-state-machine-example-in-c
    //https://www.draw.io/

    public enum State
    {
        Hidden,   //!RoadsPanel.isVisible
        Deactivated,
        Activated,
        HiddenToActivated,
        ActivatedToHidden,
    }

    public enum Command
    {
        DisplayRoadsPanel,
        HideRoadsPanel,
        PressShortcut,
        ClickToolButton,
        ActivateOtherTool,
        ClickToolModeTab,
    }

    public sealed class Transition
    {
        public readonly State From;
        public readonly Command Command;
        public readonly State To;

        public Transition(State @from, Command command, State to)
        {
            From = @from;
            Command = command;
            To = to;
        }

        #region Equality members
        private bool Equals(Transition other)
        {
            return From == other.From && Command == other.Command && To == other.To;
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            return obj is Transition && Equals((Transition)obj);
        }
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int)From;
                hashCode = (hashCode * 397) ^ (int)Command;
                hashCode = (hashCode * 397) ^ (int)To;
                return hashCode;
            }
        }
        #endregion

        public override string ToString()
        {
            return string.Format("{0} -- {1} --> {2}", From, Command, To);
        }
    }

    public class StateMachine
    {
        public IList<Transition> Transitions { get; set; }
        public State CurrentState { get; private set; }

        public StateMachine(State initialState)
        {
            CurrentState = initialState;
            Transitions = new List<Transition>(0);
        }

        private bool TryFind(State @from, Command command, out Transition transition)
        {
            if (Transitions == null)
            {
                transition = null;
                return false;
            }

            transition = Transitions.SingleOrDefault(t => t.From == @from && t.Command == command);
            return transition != null;
        }
        
        public Transition GetNext(Command command)
        {
            Transition transition;
            if (!TryFind(CurrentState, command, out transition))
            {
                throw new InvalidOperationException(string.Format("Invalid transition: {0} -> {1}", CurrentState, command));
            }
            return transition;
        }

        public Transition MoveNext(Command command)
        {
            var currentTransition = GetNext(command);
            CurrentState = currentTransition.To;
            return currentTransition;
        }

        public StateMachine ThrowIfInvalidTransitions()
        {
            if (Transitions == null)
            {
                throw new InvalidOperationException("Property Transitions is null.");
            }

            //of course....in c# that's not possible....
            //var containsDuplicates = (xs) => xs.GroupBy(x => x).Any(g => g.Count() > 1);

            //no duplicated transitions
            if (Transitions.GroupBy(t => t).Any(g => g.Count() > 1))
            {
                throw new InvalidOperationException("Property Transitions is null.");
            }

            //no duplicate Commands from each State
            if (Transitions.GroupBy(t => t.From)
                           .Any(g => g.Select(t => t.Command)
                                      .GroupBy(c => c)
                                      .Any(gr => gr.Count() > 1)))
            {
                throw new InvalidOperationException("At least from one transition is one Command outgoing more than once.");
            }

            return this;
        }

        public string[] GetTransitionsStrings()
        {
            return Transitions.OrderBy(t => t.From)
                              .ThenBy(t => t.Command)
                              .Select(t => string.Format("{0} -- {1} --> {2}", t.From, t.Command, t.To))
                              .ToArray();

        }
    }


}