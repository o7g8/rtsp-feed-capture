
namespace work_manager_akkanet
{
    class Feed {
        public string Url {get; set;}
        public string ModelName {get; set;}
        public int MaxFPS {get; set;}

        public int debugCaptureTimeMs {get; set; }
    };

    class Model {
        public string ModelName {get; set; }
        public int FramesInStack {get; set;}

        public int debugStackingTimeMs {get; set; }
        public int debugInferenceTimeMs {get; set; }
    }
    class Config {
        public Feed[] Feeds = new Feed[0];
        public Model[] Models = new Model[0];
    }
}