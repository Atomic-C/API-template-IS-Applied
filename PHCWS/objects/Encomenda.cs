using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PHCWS.objects
{
	public class Encomenda
	{
		float _shippingFees;
		float _shippingFeesWithTax;
		string _nif;
		string _subTotal;
		string _orderId;
		string _total;
		string _customerZipcode;
		string _customerAddress;
		string _customerId;
		string _customerCountry;
		string _customerCity;
		string _customerEmail;
		string _customerName;
		DateTime _dateCreated;
		int _customer_number_phc;
		int _customer_estab_phc;
		string _shipping_address_desc;
		string _shippingCountry;
		string _shippingZipcode;
		string _shippingCity;
		string _shippingAddress;

		public string shippingAddress
		{
			get { return _shippingAddress; }
			set { _shippingAddress = value; }
		}


		public string shippingCity
		{
			get { return _shippingCity; }
			set { _shippingCity = value; }
		}


		public string shippingZipcode
		{
			get { return _shippingZipcode; }
			set { _shippingZipcode = value; }
		}


		public string shippingCountry
		{
			get { return _shippingCountry; }
			set { _shippingCountry = value; }
		}
		public string shipping_address_desc
		{
			get { return _shipping_address_desc; }
			set { _shipping_address_desc = value; }
		}

		[Required]
		public int customer_estab_phc
		{
			get { return _customer_estab_phc; }
			set { _customer_estab_phc = value; }
		}
		[Required]
		public int customer_number_phc
		{
			get { return _customer_number_phc; }
			set { _customer_number_phc = value; }
		}

		public float shippingFeesWithTax
		{
			get { return _shippingFeesWithTax; }
			set { _shippingFeesWithTax = value; }
		}
		public float shippingFees
		{
			get { return _shippingFees; }
			set { _shippingFees = value; }
		}
		
		public string nif
		{
			get { return _nif; }
			set { _nif = value; }
		}
		[Required]
		public string subTotal
		{
			get { return _subTotal; }
			set { _subTotal = value; }
		}
		[Required]
		public string total
		{
			get { return _total; }
			set { _total = value; }
		}
		[Required]
		public string orderId
		{
			get { return _orderId; }
			set { _orderId = value; }
		}
		public string customerZipcode
		{
			get { return _customerZipcode; }
			set { _customerZipcode = value; }
		}
		public string customerAddress
		{
			get { return _customerAddress; }
			set { _customerAddress = value; }
		}
		public string customerId
		{
			get { return _customerId; }
			set { _customerId = value; }
		}
		public string customerCountry
		{
			get { return _customerCountry; }
			set { _customerCountry = value; }
		}
		public string customerCity
		{
			get { return _customerCity; }
			set { _customerCity = value; }
		}
		[Required]
		public string customerEmail
		{
			get { return _customerEmail; }
			set { _customerEmail = value; }
		}
		[Required]
		public string customerName
		{
			get { return _customerName; }
			set { _customerName = value; }
		}
		[Required]
		public DateTime dateCreated
		{
			get { return _dateCreated; }
			set { _dateCreated = value; }
		}
		[Required]
		public List<lines> lines { get; set; }

	}
	public class lines
	{
		string _reference;
		float _weight;
		float _quantity;
		float _percenttax;
		float _subTotalTax;
		float _total;
		float _subTotal;
		string _name;
		public float weight
		{
			get { return _weight; }
			set { _weight = value; }
		}
		public string reference
		{
			get { return _reference; }
			set { _reference = value; }
		}
		public float quantity
		{
			get { return _quantity; }
			set { _quantity = value; }
		}
		public float percenttax
		{
			get { return _percenttax; }
			set { _percenttax = value; }
		}
		public float subTotalTax
		{
			get { return _subTotalTax; }
			set { _subTotalTax = value; }
		}
		public float total
		{
			get { return _total; }
			set { _total = value; }
		}
		public float subTotal
		{
			get { return _subTotal; }
			set { _subTotal = value; }
		}
		public string name
		{
			get { return _name; }
			set { _name = value; }
		}
	}
	public class attributes
	{
		string _tinTamanho;

		public string tinTamanho
		{
			get { return _tinTamanho; }
			set { _tinTamanho = value; }
		}

		string _tinCor;

		public string tinCor
		{
			get { return _tinCor; }
			set { _tinCor = value; }
		}

	}
	public class cartCoupons
	{
		string _type;

		public string type
		{
			get { return _type; }
			set { _type = value; }
		}

		string _value;

		public string value
		{
			get { return _value; }
			set { _value = value; }
		}


		string _name;

		public string name
		{
			get { return _name; }
			set { _name = value; }
		}

	}
}