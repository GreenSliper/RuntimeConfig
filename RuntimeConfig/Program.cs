using System;

namespace RuntimeConfig
{
	class Program
	{
		static void Main(string[] args)
		{
			TextFileConfig tfc = new TextFileConfig(loadDataImmediately: true);
			Console.WriteLine(tfc["id"]);
			tfc["id"] = "1";
			tfc.SaveChanges();
		}
	}
}
