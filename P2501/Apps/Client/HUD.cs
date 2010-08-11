using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Simulation;

using OpenTK;
using OpenTK.Graphics;

namespace P2501Client
{
    public class HUDRenderer
    {
        public Player ThePlayer = null;

        protected GameWindow Win;

        public HUDRenderer ( GameWindow window )
        {
            Win = window;
            Win.Resize += new EventHandler<EventArgs>(Win_Resize);
        }

        void Win_Resize(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        public void Update ( double time )
        {

        }
    }
}
