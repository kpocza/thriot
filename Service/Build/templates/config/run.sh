#! /bin/bash

echo "Starting Management API..."
nohup dnx --appbase /opt/thriot/api/approot/packages/Thriot.Management.WebApi/1.0.0/root Microsoft.Framework.ApplicationHost kestrel &

echo "Starting Platform API..."
nohup dnx --appbase /opt/thriot/papi/approot/packages/Thriot.Platform.WebApi/1.0.0/root Microsoft.Framework.ApplicationHost kestrel &

echo "Starting Reporting API..."
nohup dnx --appbase /opt/thriot/rapi/approot/packages/Thriot.Reporting.WebApi/1.0.0/root Microsoft.Framework.ApplicationHost kestrel &

echo "Starting Messaging Services..."
nohup dnx --appbase /opt/thriot/msvc/approot/packages/Thriot.Messaging.WebApi/1.0.0/root Microsoft.Framework.ApplicationHost kestrel &

echo "Starting Central website..."
nohup dnx --appbase /opt/thriot/web/approot/packages/Thriot.Web/1.0.0/root Microsoft.Framework.ApplicationHost kestrel &

echo "Starting Websocket service..."
pushd .
cd /opt/thriot/websocketservice
mono-service Thriot.Platform.WebsocketService.exe
popd
