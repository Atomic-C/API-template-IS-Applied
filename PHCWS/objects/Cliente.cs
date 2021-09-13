using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PHCWS.objects
{
	public class Cliente
	{
		//string _clstamp;
		string _nome;
		int _no;
		int _estab;
		string _morada;
		string _codpost;
		string _localidade;
		string _NIF;
		float _IVA;
		string _tipodesconto;
		string _cod_motivo_isencao;
		string _desc_motivo_isencao;

		public string desc_motivo_isencao
		{
			get { return _desc_motivo_isencao; }
			set { _desc_motivo_isencao = value; }
		}
		public string cod_motivo_isencao
		{
			get { return _cod_motivo_isencao; }
			set { _cod_motivo_isencao = value; }
		}


		public string tipodesconto
		{
			get { return _tipodesconto; }
			set { _tipodesconto = value; }
		}


		public float IVA
		{
			get { return _IVA; }
			set { _IVA = value; }
		}


		public string NIF
		{
			get { return _NIF; }
			set { _NIF = value; }
		}


		public string localidade
		{
			get { return _localidade; }
			set { _localidade = value; }
		}


		public string codpost
		{
			get { return _codpost; }
			set { _codpost = value; }
		}

		public string morada
		{
			get { return _morada; }
			set { _morada = value; }
		}

		public int estab
		{
			get { return _estab; }
			set { _estab = value; }
		}

		public int no
		{
			get { return _no; }
			set { _no = value; }
		}


		//public string clstamp
		//{
	//		get { return _clstamp; }
	//		set { _clstamp = value; }
	//	}

		

		public string nome
		{
			get { return _nome; }
			set { _nome = value; }
		}

	}
}