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

        public static implicit operator AnswerOrCounterquestion (SerializerState answer)
        {
            return new AnswerOrCounterquestion(answer);
        }

        public static implicit operator AnswerOrCounterquestion (PendingQuestion counterquestion)
        {
            return new AnswerOrCounterquestion(counterquestion);
        }

        public SerializerState Answer => _obj as SerializerState;
        public PendingQuestion Counterquestion => _obj as PendingQuestion;
    }
}
