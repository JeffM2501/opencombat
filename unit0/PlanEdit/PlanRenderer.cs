using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Forms;
using GridWorld;

namespace PlanEdit
{
	public class PlanRenderer
	{
		public World TheWorld = null;
		PictureBox PictureFrame = null;
		string DataDir = string.Empty;

		public PlanRenderer(PictureBox frame, World world, Prefs prefs)
		{
		}
	}
}
