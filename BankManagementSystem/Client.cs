﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankManagementSystem
{
    class Client
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string DOB { get; set; }
        public string Nationality { get; set; }
        public string Gender { get; set; }
        public string NID { get; set; }
        public string Occupation { get; set; }
        public string AccountType { get; set; }
        private Account account;
    }
}
