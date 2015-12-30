#! /bin/bash

echo "Starting Management API..."
nohup dnx --project /opt/thriot/api/approot/packages/Thriot.Management.WebApi/1.0.0/root kestrel &

echo "Starting Platform API..."
nohup dnx --project /opt/thriot/papi/approot/packages/Thriot.Platform.WebApi/1.0.0/root kestrel &

echo "Starting Reporting API..."
nohup dnx --project /opt/thriot/rapi/approot/packages/Thriot.Reporting.WebApi/1.0.0/root kestrel &

echo "Starting Messaging Services..."
nohup dnx --project /opt/thriot/msvc/approot/packages/Thriot.Messaging.WebApi/1.0.0/root kestrel &

echo "Starting Central website..."
nohup dnx --project /opt/thriot/web/approot/packages/Thriot.Web/1.0.0/root kestrel &

echo "Waiting for the websites to fire up..."
sleep 5

echo "Starting Websocket service..."
pushd .
cd /opt/thriot/websocketservice
mono-service Thriot.Platform.WebsocketService.exe
popd
