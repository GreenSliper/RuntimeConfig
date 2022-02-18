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

		List<string> GetSourceOrModifiedLinesList(List<string> raw)
		{
			List<string> result = new List<string>();
			Dictionary<string, string> tmpDct = new Dictionary<string, string>(dict);
			for(int i = 0; i < raw.Count; i++)
			{
				//if line contains valid key-value pair
				if (dictLines.TryGetValue(i, out var key))
					if (tmpDct.TryGetValue(key, out var val))
					{
						result.Add(key + '=' + val);
						tmpDct.Remove(key);
					}
					else //key removed
						continue;
				else//non-modified line leaves the same
					result.Add(raw[i]);
			}
			foreach (var kvp in tmpDct)
				result.Add(kvp.Key + '=' + kvp.Value);
			return result;
		}

		protected override void Save()
		{
			var raw = new List<string>();
			using (var sr = new StreamReader(fileName))
			{
				var ln="";
				while ((ln = sr.ReadLine()) != null)
					raw.Add(ln);
			}
			var lns = GetSourceOrModifiedLinesList(raw);
			
			using (var sw = new StreamWriter(fileName))
			{
				foreach (var ln in lns)
					sw.WriteLine(ln);
			}
		}
	}
}