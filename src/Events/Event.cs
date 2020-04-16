namespace SqlStreamStore.Demo
{
    public abstract class Event
    {
        protected Event(string type)
        {
            Type = type;
        }

        public int ExpectedVersion { get; set; }
        public string Type { get; }
    }
}