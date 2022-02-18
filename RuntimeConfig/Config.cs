using System;
using System.Collections.Generic;
using System.Text;

namespace RuntimeConfig
{
	public interface IConfig
	{
		void SaveChanges();
		string this[string key] { get; set; }
	}
	public abstract class ConfigBase : IConfig
	{
		/// <param name="loadDataImmediately">If set to <see langword="false"/>, load logics is invoked only after any value is requested</param>
		public ConfigBase(bool loadDataImmediately = false)
		{
			if (loadDataImmediately)
				LoadData();
		}

		protected bool isDirty, loaded;
		protected Dictionary<string, string> dict;
		public string this[string key] {
			get
			{
				if (!loaded)
					LoadData();
				return dict[key];
			}
			set {
				if (!dict.TryGetValue(key, out var prev) || prev != value)
				{
					isDirty = true;
					dict[key] = value;
				}
			}
		}
		public void SaveChanges()
		{
			if (!isDirty)
				return;
			Save();
			isDirty = false;
		}

		protected void LoadData()
		{
			dict = new Dictionary<string, string>();
			isDirty = false;
			Load();
			loaded = true;
		}

		/// <summary>
		/// Do NOT call directly, use LoadData
		/// </summary>
		protected abstract void Load();
		protected abstract void Save();
	}
}
