using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GridWorld;
using FileLocations;

namespace PlanEdit
{
	public class Prefs
	{
		public string DataPath = string.Empty;
	}

	public class ClusterEditor
	{
		public World TheWorld = null;
		public Prefs ThePrefs = null;

		public ClusterEditor(Prefs prefs)
		{
			ThePrefs = prefs;
			TheWorld = WorldBuilder.NewWorld("default", new string[0]);
		}
	}
}
