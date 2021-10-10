using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WebServer.Models
{
    public class CalcFactorial
    {
        [DisplayName("Исходное число")]
        public long UserNomber { get; set; }

        [DisplayName("Факториал числа")]
        public string Factorial { get; set; }
    }
}
