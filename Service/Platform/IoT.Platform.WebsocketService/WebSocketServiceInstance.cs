using System.ServiceProcess;

namespace IoT.Platform.WebsocketService
{
    public partial class WebSocketServiceInstance : ServiceBase
    {
        private Brain _brain;

        public WebSocketServiceInstance()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            _brain = new Brain();
            _brain.Start();
        }

        protected override void OnStop()
        {
            _brain.Stop();
        }
    }
}
