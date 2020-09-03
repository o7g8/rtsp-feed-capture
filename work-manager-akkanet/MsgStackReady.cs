namespace work_manager_akkanet
{
    internal class MsgStackReady
    {
        public string ModelName {get; set;}
        public object Stack {get; set;}
    }
}