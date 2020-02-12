using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace TodoList.Models
{
   public class Tache
   {
      [XmlAttribute("Id")]
      public int Id { get; set; }

      [XmlText]
      [DataType(DataType.MultilineText)]
      [Required, MaxLength(250)]
      public string Description { get; set; }

      [XmlAttribute("Creation")]
      [DataType(DataType.Date)]
      public DateTime DateCreation { get; set; }

      [XmlAttribute("Term")]
      [DataType(DataType.Date)]
      public DateTime DateEcheance { get; set; }

      [XmlAttribute("Prio")]
      public int Priorite { get; set; }

      [XmlAttribute("Fait")]
      public bool Terminee { get; set; }
   }
}
