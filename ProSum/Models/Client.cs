using System;
using System.Net.Mail;


namespace ProSum.Models
{

    public class Client
    {
        public Guid Id { get; set; }


        private string name;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (value.Length > 0)
                {
                    name = value;
                }
            }
        }
        private string email;
        public string Email
        {
            get
            {
                return email;
            }
            set
            {

                if (value.Length > 0)
                {
                    try
                    {
                        email = new MailAddress(value).ToString();

                    }
                    catch (FormatException)
                    {
                        throw new FormatException("FormatException caught");

                    }
                }
            }
        }
        private string company;
        public string Company
        {
            get
            {
                return company;
            }
            set
            {

                if (value.Length > 0)
                {
                    company = value;
                }
            }

        }
        private string phoneNumber;

        public string PhoneNumber
        {
            get
            {
                return phoneNumber;
            }
            set
            {

                if (value.Length > 0)
                {

                    //if (Regex.Match(value, @"^(([+][(]?[0-9]{1,3}[)]?)|([(]?[0-9]{4}[)]?))\s*[)]?[-\s\.]?[(]?[0-9]{1,3}[)]?([-\s\.]?[0-9]{3})([-\s\.]?[0-9]{3,4})$/g").Success)
                    //{
                    phoneNumber = value;
                    //}
                    //else
                    //{
                    //    throw new ArgumentException("Invalid phoneNumber format");
                    //}
                }

            }
        }
        public Client(string name, string email, string company, string phoneNumber)
        {
            Id = Guid.NewGuid();
            Name = name;
            Email = email;
            Company = company;
            PhoneNumber = phoneNumber;
        }
        public Client()
        {

        }
    }
}
