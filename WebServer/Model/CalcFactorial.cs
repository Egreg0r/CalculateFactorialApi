using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WebServer.Models
{
    public class CalcFactorial
    {
        [DisplayName("�������� �����")]
        public long UserNomber { get; set; }

        [DisplayName("��������� �����")]
        public string Factorial { get; set; }
    }
}
