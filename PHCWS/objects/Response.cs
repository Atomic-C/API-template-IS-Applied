using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PHCWS.objects
{
	public class Response
	{
		string _content;

		public string content
		{
			get { return _content; }
			set { _content = value; }
		}

		string _ErrorMessage;


		public string ErrorMessage
		{
			get { return _ErrorMessage; }
			set { _ErrorMessage = value; }
		}

		int _codigo;

		public int codigo
		{
			get { return _codigo; }
			set { _codigo = value; }
		}

	}
}