using System;
using System.Diagnostics;

namespace RuntimeConfig
{
	class Program
	{
		static void Main(string[] args)
		{
			TextFileConfig tfc = new TextFileConfig(loadDataImmediately: true);
			Console.WriteLine(tfc["url"]);
			Console.WriteLine(tfc["url1"]);
			Console.WriteLine(tfc["id"]);
			Console.ReadKey();
		}
	}
}
