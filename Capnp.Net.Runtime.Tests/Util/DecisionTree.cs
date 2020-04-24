using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Capnp.Net.Runtime.Tests
{
    public class DecisionTree
    {
        readonly ILogger Logger = Logging.CreateLogger<DecisionTree>();

        readonly List<bool> _decisions = new List<bool>();
        readonly Stack<int> _freezeStack = new Stack<int>();
        int _pos, _freezeCounter;

        public DecisionTree()
        {
            _freezeStack.Push(0);
        }

        public DecisionTree(params bool[] initialDecisions): this()
        {
            _decisions.AddRange(initialDecisions);
        }

        public bool MakeDecision()
        {
            if (_pos >= _decisions.Count)
            {
                _decisions.Add(false);
            }

            try
            {
                return _decisions[_pos++];
            }
            catch (ArgumentOutOfRangeException)
            {
                Logger.LogError($"WTF?! {_pos - 1}, {_decisions.Count}");
                throw;
            }
        }

        public void Freeze()
        {
            _freezeStack.Push(_pos);
        }

        public bool NextRound()
        {
            _pos = 0;

            for (int i = 0; i < _freezeCounter; i++)
                _freezeStack.Pop();

            while (_freezeStack.Count > 1)
            {
                int end = _freezeStack.Pop();
                int begin = _freezeStack.Peek();

                for (int i = end - 1; i >= begin; i--)
                {
                    if (_decisions[i] == false)
                    {
                        _decisions[i] = true;
                        _freezeStack.Clear();
                        _freezeStack.Push(0);
                        return true;
                    }
                    //else
                    //{
                    //    _decisions.RemoveAt(i);
                    //}
                }

                ++_freezeCounter;
            }

            return false;
        }

        public override string ToString()
        {
            return "[" + string.Join("|", _decisions) + "]";
        }

        public void Iterate(Action testMethod)
        {
            Logger.LogInformation("Starting decision-tree based combinatorial test");
            int iter = 0;
            do
            {
                Logger.LogInformation($"Iteration {iter}: pattern {ToString()}");
                testMethod();
                ++iter;
            } while (NextRound());
        }
    }
}
