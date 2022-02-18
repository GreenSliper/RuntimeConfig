using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace RuntimeConfig
{
	/// <summary>
	/// Each line should contain one key-value pair with equality sign (=) between them
	/// Use double forward slash for comments
	/// Empty lines are ignored
	/// </summary>
	public class TextFileConfig : ConfigBase
	{
		string fileName;
		//for overriding changes
		Dictionary<int, string> dictLines = new Dictionary<int, string>();
		public TextFileConfig(string fileName = "config.conf", bool loadDataImmediately = false)
		{
			this.fileName = fileName;
			if (loadDataImmediately)
				LoadData();
		}
		protected override void Load()
		{
			using (var sr = new StreamReader(fileName))
			{
				string line = "";
				int cnt = -1;
				while ((line = sr.ReadLine()) != null)
				{
					cnt++;
					line = line.Trim();
					if (line.StartsWith("//") || !line.Contains('='))
						continue;
					var splt = line.Split('=');
					if (splt.Length != 2)
						continue;
					var key = splt[0].TrimEnd();
					dict.Add(key, splt[1].TrimStart());
					dictLines.Add(cnt, key);
				}
			}
		}

		Stream StreamFromString(string s)
		{
			var stream = new MemoryStream();
			var writer = new StreamWriter(stream);
			writer.Write(s);
			writer.Flush();
			stream.Position = 0;
			return stream;
		}

		IEnumerable<string> GetSourceOrModifiedLines(string raw)
		{
			using (var sr = new StreamReader(StreamFromString(raw)))
			{
				string line = "";
				int cnt = -1;
				while ((line = sr.ReadLine()) != null)
				{
					cnt++;
					//if line contains valid key-value pair
					if (dictLines.TryGetValue(cnt, out var key))
						if (dict.TryGetValue(key, out var val))
							yield return key + '=' + val;
						else //key removed
							continue;
					else //non-modified line
						yield return line;
			}
			}
		}

		protected override void Save()
		{
			string raw;
			using (var sr = new StreamReader(fileName))
				raw = sr.ReadToEnd();
			var lns = GetSourceOrModifiedLines(raw);
			using (var sw = new StreamWriter(fileName))
			{
				foreach (var ln in lns)
					sw.WriteLine(ln);
			}
		}
	}
}