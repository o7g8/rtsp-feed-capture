namespace work_manager_akkanet
{
    internal class MsgRequestInferenceJob
    {
        public string ModelName {get; private set;}

        public MsgRequestInferenceJob(string modelName)
        {
            ModelName = modelName;
        }
    }
}