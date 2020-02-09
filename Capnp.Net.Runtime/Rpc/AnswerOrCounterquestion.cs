namespace Capnp.Rpc
{
    /// <summary>
    /// Helper struct to support tail calls
    /// </summary>
    public struct AnswerOrCounterquestion
    {
        readonly object _obj;

        AnswerOrCounterquestion(object obj)
        {
            _obj = obj;
        }

        /// <summary>
        /// Wraps a SerializerState
        /// </summary>
        /// <param name="answer">object to wrap</param>
        public static implicit operator AnswerOrCounterquestion (SerializerState answer)
        {
            return new AnswerOrCounterquestion(answer);
        }

        /// <summary>
        /// Wraps a PendingQuestion
        /// </summary>
        /// <param name="counterquestion">object to wrap</param>
        public static implicit operator AnswerOrCounterquestion (PendingQuestion counterquestion)
        {
            return new AnswerOrCounterquestion(counterquestion);
        }

        /// <summary>
        /// SerializerState, if applicable
        /// </summary>
        public SerializerState? Answer => _obj as SerializerState;

        /// <summary>
        /// PendingQuestion, if applicable
        /// </summary>
        public PendingQuestion? Counterquestion => _obj as PendingQuestion;
    }
}