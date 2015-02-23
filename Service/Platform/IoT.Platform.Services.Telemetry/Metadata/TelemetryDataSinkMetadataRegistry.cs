﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using IoT.Platform.Services.Telemetry.Configuration;
using IoT.Plugins.Core;

namespace IoT.Platform.Services.Telemetry.Metadata
{
    public class TelemetryDataSinkMetadataRegistry : ITelemetryDataSinkMetadataRegistry
    {
        public void Build(TelemetryDataSection telemetryDataSinksSection)
        {
            Incoming = RecognizeTelemetryDataSinkMetadatas<ITelemetryDataSink>(telemetryDataSinksSection.Incoming);
        }

        private IEnumerable<TelemetryDataSinkMetadata> RecognizeTelemetryDataSinkMetadatas<TIncomingTelemetryDataSinks>(
            TelemetryDataSinkCollection elements)
            where TIncomingTelemetryDataSinks : ITelemetryDataSink
        {
            var telemetryDataSinkMetadatas = new List<TelemetryDataSinkMetadata>();

            foreach (TelemetryDataSinkElement telemetryDataSinkElement in elements)
            {
                if(telemetryDataSinkMetadatas.Any(sink => sink.Name == telemetryDataSinkElement.Name.ToLowerInvariant()))
                    throw new InvalidOperationException("Ambigous sink name: " + telemetryDataSinkElement.Name);

                var type = Type.GetType(telemetryDataSinkElement.Type);
                if(type == null)
                    throw new NullReferenceException(telemetryDataSinkElement.Type + " type cannot be resolved");

                var telemetryDataSinkInstance = (TIncomingTelemetryDataSinks) Activator.CreateInstance(type);

                var parameterPresets = new Dictionary<string, string>();
                foreach (KeyValueConfigurationElement parameterPresetConfigurationElement in telemetryDataSinkElement.ParameterPresets)
                {
                    parameterPresets[parameterPresetConfigurationElement.Key] = parameterPresetConfigurationElement.Value;
                }

                var parameterInputs = telemetryDataSinkInstance.ParametersNames.ToList();
                var parameterSubstitutes = telemetryDataSinkInstance.ParameterSubstitutes;

                foreach (var param in parameterPresets)
                {
                    if (!parameterInputs.Remove(param.Key))
                    {
                        if (parameterSubstitutes.ContainsKey(param.Key))
                        {
                            parameterInputs.Remove(parameterSubstitutes[param.Key]);
                        }
                    }
                }

                var telemetryDataSinkMetadata = new TelemetryDataSinkMetadata(telemetryDataSinkElement.Name.ToLowerInvariant(), telemetryDataSinkElement.Description, type, parameterInputs,
                    parameterPresets);

                telemetryDataSinkMetadatas.Add(telemetryDataSinkMetadata);
            }
            return telemetryDataSinkMetadatas;
        }

        public IEnumerable<TelemetryDataSinkMetadata> Incoming { get; private set; }
    }
}
