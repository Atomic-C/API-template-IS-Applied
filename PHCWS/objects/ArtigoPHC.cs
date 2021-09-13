using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PHCWS.objects
{
	public class ArtigoPHC
	{
		string _referencia;
		string _description;
		string _big_description;
		float _iva;
		float _price;
		float _minim_lot;
		float _weight;

		public float weight
		{
			get { return _weight; }
			set { _weight = value; }
		}
		public float minim_qtt
		{
			get { return _minim_lot; }
			set { _minim_lot = value; }
		}


		public string big_description
		{
			get { return _big_description; }
			set { _big_description = value; }
		}

		public string referencia
		{
			get { return _referencia; }
			set { _referencia = value; }
		}
		

		public string description
		{
			get { return _description; }
			set { _description = value; }
		}
		

		public float iva
		{
			get { return _iva; }
			set { _iva = value; }
		}
		

		public float price
		{
			get { return _price; }
			set { _price = value; }
		}


	}
}
