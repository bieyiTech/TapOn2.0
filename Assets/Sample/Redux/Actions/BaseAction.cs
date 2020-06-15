using System;

namespace Redux.Actions {
    [Serializable]
    public abstract class BaseAction {
    }

    [Serializable]
    public class RequestAction : BaseAction {
        public string url;
    }

    [Serializable]
    public class ResponseAction : BaseAction {
        public string url;
        public int statusCode;
        public string body;
    }
}