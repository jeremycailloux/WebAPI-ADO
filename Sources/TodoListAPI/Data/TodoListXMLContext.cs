using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;
using TodoList.Models;
using System.Linq;

namespace TodoListAPI.Data
{
   /// <summary>
   /// Contexte de données avec stockage dans un fichier XML
   /// NB/ En production, utiliser une vraie base de données
   /// </summary>
   public class TodoListXMLContext : ITodoListContext
   {
      private const string CHEMIN = "Data/Taches.xml";

      // Renvoie la liste de tâches du fichier xml
      public List<Tache> GetTaches()
      {
         XmlSerializer deserializer = new XmlSerializer(typeof(List<Tache>),
                                       new XmlRootAttribute("Taches"));
         using (var fs = new FileStream(CHEMIN, FileMode.Open))
         {
            var taches = (List<Tache>)deserializer.Deserialize(fs);
            return taches;
         }
      }

      // Enregistre la liste de tâches dans le fichier xml
      /*public static void EnregistrerTaches(List<Tache> taches)
      {
         XmlSerializer serializer = new XmlSerializer(typeof(List<Tache>),
                                    new XmlRootAttribute("Taches"));
         using (TextWriter writer = new StreamWriter("Data/Taches.xml"))
            serializer.Serialize(writer, taches);
      }*/

      // Ajoute une nouvelle tâche dans le fichier
      public void AjouterTache(Tache tache)
      {
         // On crée un elt pour la nouvelle tâche
         XElement elt = CréerTache(tache);

         // On ajout l'elt au doc et on enregistre
         XDocument doc = XDocument.Load(CHEMIN);
         doc.Root.Add(elt);
         doc.Save(CHEMIN);
      }

      // Modifie une tâche par remplacement complet
      public void ModifierTache(Tache tache)
      {
         XDocument doc = XDocument.Load(CHEMIN);
         
         // On recherche la tâche
         XElement elt = doc.Descendants("Tache").Where(t => t.Attribute("Id").Value == tache.Id.ToString()).FirstOrDefault();

         // Si elle existe, on la supprime
         if (elt != null) elt.Remove();

         // On crée un elt pour la tâche
         elt = CréerTache(tache);

         // On l'ajoute au doc et on enregistre
         doc.Root.Add(elt);
         doc.Save(CHEMIN);
      }

      // Supprime la tâche dont l'id est passé en paramètre
      public void SupprimerTache(int id)
      {
         XDocument doc = XDocument.Load(CHEMIN);
         
         // On recherche la tâche
         XElement elt = doc.Descendants("Tache").Where(t => t.Attribute("Id").Value == id.ToString()).FirstOrDefault();

         // Si elle existe, on la supprime et on renregistre
         if (elt != null)
         {
            elt.Remove();
            doc.Save(CHEMIN);
         }
      }

      // Crée et renvoie un elt xml représentant la tâche passée en paramètre
      private XElement CréerTache(Tache tache)
      {
         // Création d'un elt xml représentant la tâche
         XElement elt = new XElement("Tache");
         elt.Add(new XAttribute("Id", tache.Id));
         elt.Add(new XAttribute("Creation", tache.DateCreation));
         elt.Add(new XAttribute("Term", tache.DateEcheance));
         elt.Add(new XAttribute("Prio", tache.Priorite));
         elt.Add(new XAttribute("Fait", tache.Terminee));
         elt.Add(tache.Description);

         return elt;
      }
   }
}
