using System;
using System.Diagnostics;

namespace RuntimeConfig
{
	class Program
	{
		static void Main(string[] args)
		{
			Stopwatch sw = new Stopwatch();
			int N = 1000;
			TextFileConfig tfc = new TextFileConfig(loadDataImmediately: true);

			sw.Start();
			for (int i = 0; i < N; i++)
			{
				tfc["id"] = (i % 2).ToString();
				tfc.SaveChanges();
			}
			sw.Stop();
			Console.WriteLine("Elapsed={0}", sw.Elapsed);
			Console.ReadKey();
		}
	}
}
