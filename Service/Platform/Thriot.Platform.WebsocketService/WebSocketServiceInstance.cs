using System.ServiceProcess;

namespace Thriot.Platform.WebsocketService
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
            var serviceSetup = new ServicesSetup();
            serviceSetup.Setup();

            _brain = new Brain(serviceSetup.GetServiceProvider());
            _brain.Start();
        }

        protected override void OnStop()
        {
            _brain.Stop();
        }
    }
}
