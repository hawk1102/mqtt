﻿using System.Collections.Generic;
using System.Linq;
using Hermes.Packets;
using Hermes.Properties;

namespace Hermes
{
	public class ConnectionProvider : IConnectionProvider
	{
		//TODO: We should control concurrency in this list (ConcurrentDictionary is not available on PCL's)
		readonly IDictionary<string, IChannel<IPacket>> connections = new Dictionary<string, IChannel<IPacket>> ();

		public int Connections { get { return this.connections.Count; } }

		public void AddConnection(string clientId, IChannel<IPacket> connection)
        {
			var existingConnection = this.connections.FirstOrDefault (c => c.Key == clientId);

			if (!existingConnection.Equals(default(KeyValuePair<string, IChannel<IPacket>>))) {
				this.RemoveConnection (clientId);
				existingConnection.Value.Close ();
			}

			this.connections.Add (clientId, connection);
        }

		/// <exception cref="ProtocolException">ProtocolException</exception>
		public IChannel<IPacket> GetConnection (string clientId)
		{
			var connection = this.connections.FirstOrDefault (c => c.Key == clientId);

			if (connection.Equals (default (KeyValuePair<string, IChannel<IPacket>>))) {
				var error = string.Format (Resources.ConnectionProvider_ClientIdNotFound, clientId);
				
				throw new ProtocolException (error);
			}

			var clientConnection = this.connections.First (c => c.Key == clientId);

			return clientConnection.Value;

		}

		/// <exception cref="ProtocolException">ProtocolException</exception>
        public void RemoveConnection(string clientId)
        {
			var connection = this.connections.FirstOrDefault (c => c.Key == clientId);

			if (connection.Equals (default (KeyValuePair<string, IChannel<IPacket>>))){
				var error = string.Format (Resources.ConnectionProvider_ClientIdNotFound, clientId);
				
				throw new ProtocolException (error);
			}

			this.connections.Remove (clientId);
        }
	}
}