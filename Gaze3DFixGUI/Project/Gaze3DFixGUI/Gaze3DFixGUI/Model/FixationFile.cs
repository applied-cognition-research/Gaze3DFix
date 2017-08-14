using System;
using System.Collections.Generic;

namespace Gaze3DFixGUI.Model
{
    class FixationFile
    {
        public String filename = "";

        public List<FixationData> list_FixationData = new List<FixationData>();

        public FixationFile ()
        {

        }

        public void addFixationData(FixationData fixationdata)
        {
            list_FixationData.Add(fixationdata);
        }
    }
}