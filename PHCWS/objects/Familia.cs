using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PHCWS.objects
{
	public class Familia
	{
        string _familiadesc;
        string _design;


        public string familia
        {
            get { return _familiadesc; }
            set { _familiadesc = value; }
        }

        public string design
        {
            get { return _design; }
            set { _design = value; }
        }
       
    }
}
