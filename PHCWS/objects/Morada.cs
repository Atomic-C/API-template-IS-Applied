using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PHCWS.objects
{
	public class Morada
	{
		string _descricao;
		string _morada;
		string _localidade;
		string _codpostal;
		string _codpais;
		string _descpais;
		string _nome;
		[Required]
		public string nome
		{
			get { return _nome; }
			set { _nome = value; }
		}

		int _no;
		int _estab;
		[Required]
		public int estab
		{
			get { return _estab; }
			set { _estab = value; }
		}

		[Required]
		public int no
		{
			get { return _no; }
			set { _no = value; }
		}

		[Required]
		public string descpais
		{
			get { return _descpais; }
			set { _descpais = value; }
		}
		[Required]
		public string codpais
		{
			get { return _codpais; }
			set { _codpais = value; }
		}

		[Required]
		public string codpostal
		{
			get { return _codpostal; }
			set { _codpostal = value; }
		}

		[Required]
		public string localidade
		{
			get { return _localidade; }
			set { _localidade = value; }
		}

		[Required]
		public string morada
		{
			get { return _morada; }
			set { _morada = value; }
		}
		[Required]
		public string descricao
		{
			get { return _descricao; }
			set { _descricao = value; }
		}

	}
}