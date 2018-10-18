using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;
using System.Xml;

using Indy.Sockets.Core;

namespace Indy.Sockets.UnitTests.Core {
	public static class TestUtilities {
		private static string GetBasePath() {
			/*      string TempString = ConfigurationManager.AppSettings["SimulationsPath"];
						if (String.IsNullOrEmpty(TempString))
						{
							throw new Exception("Make sure you created a UnitTests.config file!");
						}
						return TempString;*/

			return Path.Combine(NUnit.Core.TestContext.CurrentDirectory, "SimulationsData");
		}

		public static ServerSocketSimulator GetSimulationServerSocket<TReplyCode, TReply, TContext>
		(TcpServerBase<TReplyCode, TReply, TContext> testedObject, string simulationName)
			where TContext: Context<TReplyCode, TReply, TContext>, new()
			where TReplyCode: IEquatable<TReplyCode>
			where TReply: Reply<TReplyCode>, new() {
			Type testedType = testedObject.GetType();
			int indexOfLastDot = testedObject.GetType().Assembly.GetName().Name.LastIndexOf(".");
			if (indexOfLastDot == -1) {
				throw new Exception("AssemblyName doesn't contain a dot");
			}
			string SimulationPath = Path.Combine(GetBasePath(), testedType.Assembly.GetName().Name.Substring(indexOfLastDot + 1));
			if (testedType.Name.Contains("`")) {
				SimulationPath = Path.Combine(SimulationPath, testedType.Name.Substring(0, testedType.Name.IndexOf("`")));
			} else {
				SimulationPath = Path.Combine(SimulationPath, testedType.Name);
			}
			SimulationPath = Path.Combine(SimulationPath, simulationName);
			string SimName = testedType.Assembly.GetName().Name.Substring(indexOfLastDot + 1) + ".";
			if (testedType.Name.Contains("`")) {
				SimName += testedType.Name.Substring(0, testedType.Name.IndexOf("`"));
			} else {
				SimName += testedType.Name;
			}
			SimName += "." + simulationName;
			ServerSocketSimulator sss = new ServerSocketSimulator();
			foreach (string s in Directory.GetFiles(SimulationPath, "*.xml")) {
				XmlDocument XmlDoc = new XmlDocument();
				XmlDoc.Load(s);
				sss.AddXmlDocument(XmlDoc, SimName + "." + Path.GetFileNameWithoutExtension(s));
			}
			testedObject.OnContextException += sss.DoContextException<TReplyCode, TReply, TContext>;
			testedObject.OnListenException += sss.DoListenException;

			return sss;
		}

		public static SocketSimulator GetSimulationSocket(Type testedType, string simulationName) {
			int indexOfLastDot = testedType.Assembly.GetName().Name.LastIndexOf(".");
			if (indexOfLastDot == -1) {
				throw new Exception("AssemblyName doesn't contain a dot");
			}
			string SimulationFileName = Path.Combine(GetBasePath(), testedType.Assembly.GetName().Name.Substring(indexOfLastDot + 1));
			if (testedType.Name.Contains("`")) {
				SimulationFileName = Path.Combine(SimulationFileName, testedType.Name.Substring(0, testedType.Name.IndexOf("`")));
			} else {
				SimulationFileName = Path.Combine(SimulationFileName, testedType.Name);
			}
			SimulationFileName = Path.Combine(SimulationFileName, simulationName + ".xml");
			string SimName = testedType.Assembly.GetName().Name.Substring(indexOfLastDot + 1) + ".";
			if (testedType.Name.Contains("`")) {
				SimName += testedType.Name.Substring(0, testedType.Name.IndexOf("`"));
			} else {
				SimName += testedType.Name;
			}
			SimName += "." + simulationName;
			return new SocketSimulator(new FileStream(SimulationFileName, FileMode.Open, FileAccess.Read), SimName);
		}
	}
}