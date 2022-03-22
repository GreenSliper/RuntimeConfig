using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace RuntimeConfig
{
	/// <summary>
	/// Each line should contain one key-value pair with equality sign (=) between them
	/// Use double forward slash for comments
	/// If the value contains equality sign or leading/trailing whitespaces, use double quotes
	/// Empty/invalid lines are ignored
	/// </summary>
	public class TextFileConfig : ConfigBase
	{
		string fileName;
		
		char splitChar = '=', quoteChar = '\"';
		string commentStringStart = "//";
		//for overriding changes
		Dictionary<int, string> dictLines = new Dictionary<int, string>();
		public TextFileConfig(string fileName = "config.conf", bool loadDataImmediately = false)
		{
			this.fileName = fileName;
			if (loadDataImmediately)
				LoadData();
		}

		bool SplitLine(string line, out string key, out string value)
		{
			key = value = null;
			line = line.Trim();
			int breakIndex = 0;
			if (line.StartsWith(commentStringStart) || (breakIndex = line.IndexOf(splitChar)) < 0)
				return false;
			key = line.Substring(0, breakIndex).TrimEnd();
			//value is null, valid
			if (breakIndex == line.Length - 1)
				return true;

			value = line.Substring(breakIndex + 1).TrimStart();
			bool isQuoted = value.StartsWith(quoteChar) && value.EndsWith(quoteChar);
			if(isQuoted)
				value = value.Trim(quoteChar);
			//double equality is not valid, except if the value is quoted 
			//and equality sign is a part of the value
			return !value.Contains(splitChar) || isQuoted;
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
					if (SplitLine(line, out var key, out var val))
					{
						dict.Add(key, val);
						dictLines.Add(cnt, key);
					}
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