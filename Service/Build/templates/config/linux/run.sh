#! /bin/bash

cd /opt/thriot/apihost
echo "Starting APIs..."
mono Thriot.ApiHost.exe
cd ../websocketservice
echo "Starting Websocket service..."
mono-service Thriot.Platform.WebsocketService.exe
cd ..
echo "Starting Central website..."
dnx --appbase /opt/thriot/web/approot/packages/Thriot.Web/1.0.0/root Microsoft.Framework.ApplicationHost kestrel &

